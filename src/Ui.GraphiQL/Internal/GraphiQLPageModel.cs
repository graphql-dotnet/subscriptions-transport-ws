using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace GraphQL.Server.Ui.GraphiQL.Internal
{
    // https://docs.microsoft.com/en-us/aspnet/core/mvc/razor-pages/?tabs=netcore-cli
    internal sealed class GraphiQLPageModel
    {
        private string _graphiQLCSHtml;

        private readonly GraphiQLOptions _options;

        public GraphiQLPageModel(GraphiQLOptions options)
        {
            _options = options;
        }

        public string Render()
        {
            if (_graphiQLCSHtml == null)
            {
                using var manifestResourceStream = _options.IndexStream();
                using var streamReader = new StreamReader(manifestResourceStream);

                var headers = new Dictionary<string, object>
                {
                    ["Accept"] = "application/json",
                    ["Content-Type"] = "application/json",
                };

                if (_options.Headers?.Count > 0)
                {
                    foreach (var item in _options.Headers)
                        headers[item.Key] = item.Value;
                }

                var builder = new StringBuilder(streamReader.ReadToEnd())
                    .Replace("@Model.GraphQLEndPoint", _options.GraphQLEndPoint)
                    .Replace("@Model.SubscriptionsEndPoint", _options.SubscriptionsEndPoint)
                    .Replace("@Model.Headers", JsonSerializer.Serialize<object>(headers))
                    .Replace("@Model.AdditionalData", JsonSerializer.Serialize(_options.AdditionalData, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    }));

                _graphiQLCSHtml = builder.ToString();
            }

            return _graphiQLCSHtml;
        }
    }
}
