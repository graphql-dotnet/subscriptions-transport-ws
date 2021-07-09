using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
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
        public PathString GraphQLEndPoint { get; set; } = "/graphql";

        /// <summary>
        /// Subscriptions EndPoint.
        /// </summary>
        public PathString SubscriptionsEndPoint { get; set; } = "/graphql";

        /// <summary>
        /// HTTP headers with which the GraphiQL will be initialized.
        /// </summary>
        public Dictionary<string, string> Headers { get; set; }

        /// <summary>
        /// Function to get index.html page with GraphiQL setup.
        /// </summary>
        public Func<Stream> IndexStream { get; set; } = () => typeof(GraphiQLOptions).GetTypeInfo().Assembly
            .GetManifestResourceStream("GraphQL.Server.Ui.GraphiQL.Internal.graphiql.cshtml");

        /// <summary>
        /// Additional data object for custom index.html page, that is serialized into `@Model.AdditionalData` placeholder.
        /// </summary>
        public object AdditionalData { get; set; }
    }
}
