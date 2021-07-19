using System;
using Microsoft.Extensions.DependencyInjection;

namespace GraphQL.Server
{
    /// <summary>
    /// GraphQL builder used for GraphQL specific extension methods as 'this' argument.
    /// </summary>
    [Obsolete]
    public interface IGraphQLBuilder
    {
        /// <summary>
        /// Underlying <see cref="IServiceCollection"/> collection used (filled) by extension methods.
        /// </summary>
        IServiceCollection Services { get; }
    }
}
