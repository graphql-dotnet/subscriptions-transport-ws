using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using GraphQL.SystemTextJson;
using Microsoft.AspNetCore.Http;

namespace GraphQL.Server.Transports.AspNetCore.SystemTextJson
{
    /// <summary>
    /// Implementation of an <see cref="IGraphQLRequestDeserializer"/> that uses System.Text.Json;
    /// reading the request body asynchronously and optimally.
    /// </summary>
    /// <remarks>
    /// With thanks to David Fowler (@davidfowl) for his help getting this right.
    /// </remarks>
    public class GraphQLRequestDeserializer : IGraphQLRequestDeserializer
    {
        private readonly JsonSerializerOptions _serializerOptions = new JsonSerializerOptions();

        public GraphQLRequestDeserializer(Action<JsonSerializerOptions> configure)
        {
            // Add converter that deserializes Variables property
            _serializerOptions.Converters.Add(new InputsConverter());

            configure?.Invoke(_serializerOptions);
        }

        public async Task<GraphQLRequestDeserializationResult> DeserializeFromJsonBodyAsync(HttpRequest httpRequest, CancellationToken cancellationToken = default)
        {
            var bodyReader = httpRequest.BodyReader;

            JsonTokenType jsonTokenType;
            try
            {
                jsonTokenType = await PeekJsonTokenTypeAsync(bodyReader, cancellationToken);
            }
            catch (JsonException)
            {
                // Invalid request content, assign None so it falls through to IsSuccessful = false
                jsonTokenType = JsonTokenType.None;
            }

            cancellationToken.ThrowIfCancellationRequested();

            var result = new GraphQLRequestDeserializationResult { IsSuccessful = true };
            switch (jsonTokenType)
            {
                case JsonTokenType.StartObject:
                    result.Single = ToGraphQLRequest(
                        await JsonSerializer.DeserializeAsync<InternalGraphQLRequest>(bodyReader.AsStream(), _serializerOptions, cancellationToken));
                    return result;
                case JsonTokenType.StartArray:
                    result.Batch = (await JsonSerializer.DeserializeAsync<InternalGraphQLRequest[]>(bodyReader.AsStream(), _serializerOptions, cancellationToken))
                        .Select(ToGraphQLRequest)
                        .ToArray();
                    return result;
                default:
                    result.IsSuccessful = false;
                    return result;
            }
        }

        private static async ValueTask<JsonTokenType> PeekJsonTokenTypeAsync(PipeReader reader, CancellationToken cancellationToken)
        {
            // Separate method so that we can use the ref struct
            static bool DetermineTokenType(in ReadOnlySequence<byte> buffer, out JsonTokenType jsonToken)
            {
                var jsonReader = new Utf8JsonReader(buffer);
                if (jsonReader.Read())
                {
                    jsonToken = jsonReader.TokenType;
                    return true;
                }
                jsonToken = JsonTokenType.None;
                return false;
            }

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var result = await reader.ReadAsync(cancellationToken);

                cancellationToken.ThrowIfCancellationRequested();

                var buffer = result.Buffer;

                if (DetermineTokenType(buffer, out var tokenType))
                {
                    // Don't consume any of the buffer so we can re-parse it with the
                    // serializer
                    reader.AdvanceTo(buffer.Start, buffer.Start);
                    return tokenType;
                }
                else
                {
                    // We don't have enough to read a token, keep buffering
                    reader.AdvanceTo(buffer.Start, buffer.End);
                }

                // If there's no more data coming, then bail
                if (result.IsCompleted)
                {
                    return JsonTokenType.None;
                }
            }
        }

        public Inputs DeserializeInputsFromJson(string json) => json?.ToInputs();

        private static GraphQLRequest ToGraphQLRequest(InternalGraphQLRequest internalGraphQLRequest)
            => new GraphQLRequest
            {
                OperationName = internalGraphQLRequest.OperationName,
                Query = internalGraphQLRequest.Query,
                Inputs = internalGraphQLRequest.Variables, // must return null if not provided, not an empty dictionary
                Extensions = internalGraphQLRequest.Extensions, // must return null if not provided, not an empty dictionary
            };
    }
}
