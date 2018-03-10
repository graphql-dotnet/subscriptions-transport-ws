using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading.Tasks;
using GraphQL.Server.Transports.Subscriptions.Abstractions;
using GraphQL.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GraphQL.Server.Transports.WebSockets
{
    public class GraphQLWebSocketsMiddleware<TSchema> where TSchema : ISchema
    {
        private readonly IGraphQLExecuter _executer;
        private readonly RequestDelegate _next;
        private readonly IEnumerable<IOperationMessageListener> _messageListeners;
        private readonly ILoggerFactory _loggerFactory;
        private readonly GraphQLWebSocketsOptions _options;
        private readonly ILogger _logger;

        public GraphQLWebSocketsMiddleware(
            RequestDelegate next,
            IGraphQLExecuterFactory executerFactory,
            IOptions<GraphQLWebSocketsOptions> options,
            IEnumerable<IOperationMessageListener> messageListeners,
            ILoggerFactory loggerFactory)
        {
            _next = next;
            _messageListeners = messageListeners;
            _loggerFactory = loggerFactory;
            _executer = executerFactory.Create<TSchema>();
            _options = options.Value;
            _logger = loggerFactory.CreateLogger<GraphQLWebSocketsMiddleware<TSchema>>();
        }

        public async Task Invoke(HttpContext context)
        {
            using (_logger.BeginScope(new Dictionary<string, object>() {
                ["ConnectionId"] = context.Connection.Id,
                ["Request"] = context.Request
            }))
            {
                if (!IsGraphQLRequest(context))
                {
                    _logger.LogInformation("Request is not a valid  websocket request");
                    await _next(context);
                    return;
                }

                _logger.LogInformation("Connection is a valid websocket request");
                await ExecuteAsync(context);
            }
        }

        private bool IsGraphQLRequest(HttpContext context)
        {
            var path = context.Request.Path;
            if (!path.StartsWithSegments(_options.Path))
                return false;

            if (!context.WebSockets.IsWebSocketRequest)
                return false;

            return true;
        }

        private async Task ExecuteAsync(HttpContext context)
        {
            var socket = await context.WebSockets
                .AcceptWebSocketAsync("graphql-ws").ConfigureAwait(false);

            if (!context.WebSockets.WebSocketRequestedProtocols
                .Contains(socket.SubProtocol))
            {
                _logger.LogError(
                    "Websocket connection does not have correct protocol: graphql-ws. Request protocols: {protocols}",
                    context.WebSockets.WebSocketRequestedProtocols);
                await socket.CloseAsync(
                    WebSocketCloseStatus.ProtocolError,
                    $"Server only supports graphql-ws protocol",
                    context.RequestAborted).ConfigureAwait(false);

                return;
            }

            using (_logger.BeginScope($"GraphQL websocket connection: {context.Connection.Id}"))
            {
                var connection = new WebSocketConnection(
                    socket,
                    context.Connection.Id,
                    new SubscriptionManager(_executer, _loggerFactory),
                    _messageListeners,
                    _loggerFactory);

                await connection.Connect();
            }
        }
    }
}