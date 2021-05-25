using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GraphQL.NewtonsoftJson;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace GraphQL.Server.Transports.AspNetCore.NewtonsoftJson
{
    /// <summary>
    /// Implementation of an <see cref="IGraphQLRequestDeserializer"/> that uses Newtonsoft.Json.
    /// </summary>
    public class GraphQLRequestDeserializer : IGraphQLRequestDeserializer
    {
        private readonly JsonSerializer _serializer;

        public GraphQLRequestDeserializer(Action<JsonSerializerSettings> configure)
        {
            var settings = new JsonSerializerSettings();
            configure?.Invoke(settings);
            _serializer = JsonSerializer.Create(settings); // it's thread safe https://stackoverflow.com/questions/36186276/is-the-json-net-jsonserializer-threadsafe
        }

        public Task<GraphQLRequestDeserializationResult> DeserializeFromJsonBodyAsync(HttpRequest httpRequest, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Do not explicitly or implicitly (via using, etc.) call dispose because StreamReader will dispose inner stream.
            // This leads to the inability to use the stream further by other consumers/middlewares of the request processing
            // pipeline. In fact, it is absolutely not dangerous not to dispose StreamReader as it does not perform any useful
            // work except for the disposing inner stream.
            var reader = new StreamReader(httpRequest.Body);

            var result = new GraphQLRequestDeserializationResult { IsSuccessful = true };

            using (var jsonReader = new JsonTextReader(reader) { CloseInput = false })
            {
                int firstChar = reader.Peek();

                cancellationToken.ThrowIfCancellationRequested();

                switch (firstChar)
                {
                    case '{':
                        try
                        {
                            result.Single = ToGraphQLRequest(_serializer.Deserialize<InternalGraphQLRequest>(jsonReader));
                        }
                        catch (JsonException e)
                        {
                            result.IsSuccessful = false;
                            result.Exception = e;
                        }
                        break;
                    case '[':
                        try
                        {
                            result.Batch = _serializer.Deserialize<InternalGraphQLRequest[]>(jsonReader)
                                .Select(ToGraphQLRequest)
                                .ToArray();
                        }
                        catch (JsonException e)
                        {
                            result.IsSuccessful = false;
                            result.Exception = e;
                        }
                        break;
                    default:
                        result.IsSuccessful = false;
                        break;
                }
            }

            return Task.FromResult(result);
        }

        public Inputs DeserializeInputsFromJson(string json) => json?.ToInputs();

        private static GraphQLRequest ToGraphQLRequest(InternalGraphQLRequest internalGraphQLRequest)
            => new GraphQLRequest
            {
                OperationName = internalGraphQLRequest.OperationName,
                Query = internalGraphQLRequest.Query,
                Inputs = internalGraphQLRequest.Variables?.ToInputs(), // must return null if not provided, not an empty dictionary
                Extensions = internalGraphQLRequest.Extensions?.ToInputs(), // must return null if not provided, not an empty dictionary
            };
    }
}
