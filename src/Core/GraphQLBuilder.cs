using System;
using Microsoft.Extensions.DependencyInjection;

namespace GraphQL.Server
{
    [Obsolete]
    internal sealed class GraphQLBuilder : IGraphQLBuilder
    {
        public IServiceCollection Services { get; }

        public GraphQLBuilder(IServiceCollection services)
        {
            Services = services;
        }
    }
}
