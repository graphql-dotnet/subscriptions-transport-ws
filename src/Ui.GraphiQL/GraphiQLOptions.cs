using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace GraphQL.Server.Ui.GraphiQL
{
    /// <summary>
    /// Options to customize the <see cref="GraphiQLMiddleware"/>.
    /// </summary>
    public class GraphiQLOptions
    {
        /// <summary>
        /// The GraphQL EndPoint.
        /// </summary>
        public string GraphQLEndPoint { get; set; } = "/graphql";

        /// <summary>
        /// Subscriptions EndPoint.
        /// </summary>
        public string SubscriptionsEndPoint { get; set; } = "/graphql";

        /// <summary>
        /// HTTP headers with which the GraphiQL will be initialized.
        /// </summary>
        public Dictionary<string, string> Headers { get; set; }
    }
}
