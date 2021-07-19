using System;
using GraphQL;
using GraphQL.Instrumentation;
using GraphQL.Server;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods for <see cref="IServiceCollection"/> to add GraphQL execution engine.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add required services for GraphQL.
        /// </summary>
        /// <param name="services">Collection of registered services.</param>
        /// <returns>GraphQL builder used for GraphQL specific extension chaining.</returns>
        [Obsolete]
        public static IGraphQLBuilder AddGraphQL(this IServiceCollection services) => services.AddGraphQL(_ => { });

        /// <summary>
        /// Add required services for GraphQL.
        /// </summary>
        /// <param name="services">Collection of registered services.</param>
        /// <param name="configureOptions">An action delegate to configure the provided <see cref="GraphQLOptions"/>.</param>
        /// <returns>GraphQL builder used for GraphQL specific extension chaining.</returns>
        [Obsolete]
        public static IGraphQLBuilder AddGraphQL(this IServiceCollection services, Action<GraphQLOptions> configureOptions)
        {
            if (configureOptions == null)
                throw new ArgumentNullException(nameof(configureOptions));

            return services.AddGraphQL((opt, _) => configureOptions(opt));
        }

        /// <summary>
        /// Add required services for GraphQL.
        /// </summary>
        /// <param name="services">Collection of registered services.</param>
        /// <param name="configureOptions">
        /// An action delegate to configure the provided <see cref="GraphQLOptions"/>.
        /// This delegate provides additional <see cref="IServiceProvider"/> parameter to resolve all necessary dependencies.
        /// </param>
        /// <returns>GraphQL builder used for GraphQL specific extension chaining.</returns>
        [Obsolete]
        public static IGraphQLBuilder AddGraphQL(this IServiceCollection services, Action<GraphQLOptions, IServiceProvider> configureOptions)
        {
            if (configureOptions == null)
                throw new ArgumentNullException(nameof(configureOptions));

            // This is used instead of "normal" services.Configure(configureOptions) to pass IServiceProvider to user code.
            services.AddSingleton<IConfigureOptions<GraphQLOptions>>(x => new ConfigureNamedOptions<GraphQLOptions>(Options.Options.DefaultName, opt => configureOptions(opt, x)));
            services.TryAddSingleton<InstrumentFieldsMiddleware>();
            services.TryAddSingleton<IDocumentExecuter, DocumentExecuter>();
            services.TryAddTransient(typeof(IGraphQLExecuter<>), typeof(DefaultGraphQLExecuter<>));

            services.TryAddSingleton<IDocumentWriter>(x =>
            {
                throw new InvalidOperationException(
                    "IDocumentWriter not set in DI container. " +
                    "Add a IDocumentWriter implementation, for example " +
                    "GraphQL.SystemTextJson.DocumentWriter or GraphQL.NewtonsoftJson.DocumentWriter." +
                    "For more information, see: https://github.com/graphql-dotnet/graphql-dotnet/blob/master/README.md and https://github.com/graphql-dotnet/server/blob/develop/README.md.");
            });

            return new GraphQLBuilder(services);
        }
    }
}
