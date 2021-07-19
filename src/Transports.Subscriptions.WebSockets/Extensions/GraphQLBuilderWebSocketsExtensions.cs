using System;
using GraphQL.Server.Transports.Subscriptions.Abstractions;
using GraphQL.Server.Transports.WebSockets;
using Microsoft.Extensions.DependencyInjection;

namespace GraphQL.Server
{
    public static class GraphQLBuilderWebSocketsExtensions
    {
        /// <summary>
        /// Add required services for GraphQL web sockets
        /// </summary>
        [Obsolete]
        public static IGraphQLBuilder AddWebSockets(this IGraphQLBuilder builder)
        {
            builder.Services
                .AddTransient(typeof(IWebSocketConnectionFactory<>), typeof(WebSocketConnectionFactory<>))
                .AddTransient<IOperationMessageListener, LogMessagesListener>()
                .AddTransient<IOperationMessageListener, ProtocolMessageListener>();

            return builder;
        }

        /// <summary>
        /// Add required services for GraphQL web sockets
        /// </summary>
        public static DI.IGraphQLBuilder AddWebSockets(this DI.IGraphQLBuilder builder)
        {
            builder
                .Register(typeof(IWebSocketConnectionFactory<>), typeof(WebSocketConnectionFactory<>), DI.ServiceLifetime.Transient)
                .Register<IOperationMessageListener, LogMessagesListener>(DI.ServiceLifetime.Transient)
                .Register<IOperationMessageListener, ProtocolMessageListener>(DI.ServiceLifetime.Transient);

            return builder;
        }
    }
}
