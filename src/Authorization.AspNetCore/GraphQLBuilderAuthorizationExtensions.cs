using System;
using GraphQL.Server.Authorization.AspNetCore;
using GraphQL.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace GraphQL.Server
{
    public static class GraphQLBuilderAuthorizationExtensions
    {
        /// <summary>
        /// Adds the GraphQL authorization.
        /// </summary>
        /// <param name="builder">The GraphQL builder.</param>
        /// <returns>Reference to the passed <paramref name="builder"/>.</returns>
        [Obsolete]
        public static IGraphQLBuilder AddGraphQLAuthorization(this IGraphQLBuilder builder)
            => builder.AddGraphQLAuthorization(options => { });

        /// <summary>
        /// Adds the GraphQL authorization.
        /// </summary>
        /// <param name="builder">The GraphQL builder.</param>
        /// <param name="options">An action delegate to configure the provided <see cref="AuthorizationOptions"/>.</param>
        /// <returns>Reference to the passed <paramref name="builder"/>.</returns>
        [Obsolete]
        public static IGraphQLBuilder AddGraphQLAuthorization(this IGraphQLBuilder builder, Action<AuthorizationOptions> options)
        {
            builder.Services.TryAddTransient<IClaimsPrincipalAccessor, DefaultClaimsPrincipalAccessor>();
            builder.Services
                .AddHttpContextAccessor()
                .AddTransient<IValidationRule, AuthorizationValidationRule>()
                .AddAuthorizationCore(options);

            return builder;
        }

        /// <summary>
        /// Adds the GraphQL authorization.
        /// </summary>
        /// <param name="builder">The GraphQL builder.</param>
        /// <returns>Reference to the passed <paramref name="builder"/>.</returns>
        public static DI.IGraphQLBuilder AddGraphQLAuthorization(this DI.IGraphQLBuilder builder)
            => builder.AddGraphQLAuthorization(_ => { });

        /// <summary>
        /// Adds the GraphQL authorization.
        /// </summary>
        /// <param name="builder">The GraphQL builder.</param>
        /// <param name="configure">An action delegate to configure the provided <see cref="AuthorizationOptions"/>.</param>
        /// <returns>Reference to the passed <paramref name="builder"/>.</returns>
        public static DI.IGraphQLBuilder AddGraphQLAuthorization(this DI.IGraphQLBuilder builder, Action<AuthorizationOptions> configure)
        {
            var services = (builder as MicrosoftDI.GraphQLBuilder)?.Services ?? throw new NotSupportedException("This method only supports the MicrosoftDI implementation of IGraphQLBuilder.");
            services.TryAddTransient<IClaimsPrincipalAccessor, DefaultClaimsPrincipalAccessor>();
            services.AddHttpContextAccessor();
            services.AddAuthorizationCore(configure);
            builder.AddValidationRule<AuthorizationValidationRule>();

            return builder;
        }
    }
}
