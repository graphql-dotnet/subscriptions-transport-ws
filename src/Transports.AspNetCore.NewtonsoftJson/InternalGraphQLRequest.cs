using Newtonsoft.Json;

namespace GraphQL.Server.Transports.AspNetCore.NewtonsoftJson
{
    /// <summary>
    /// A type for deserializing directly into that suits the NewtonsoftJson serializer.
    /// </summary>
    internal sealed class InternalGraphQLRequest
    {
        [JsonProperty(GraphQLRequest.OPERATION_NAME_KEY)]
        public string OperationName { get; set; }

        [JsonProperty(GraphQLRequest.QUERY_KEY)]
        public string Query { get; set; }

        [JsonProperty(GraphQLRequest.VARIABLES_KEY)]
        public Inputs Variables { get; set; }

        [JsonProperty(GraphQLRequest.EXTENSIONS_KEY)]
        public Inputs Extensions { get; set; }
    }
}
