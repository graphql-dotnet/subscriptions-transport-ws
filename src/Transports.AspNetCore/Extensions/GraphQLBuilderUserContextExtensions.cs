using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GraphQL.Server.Transports.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace GraphQL.Server
{
    public static class GraphQLBuilderUserContextExtensions
    {
        /// <summary>
        /// Adds an <see cref="IUserContextBuilder"/> as a singleton.
        /// </summary>
        /// <typeparam name="TUserContextBuilder">The type of the <see cref="IUserContextBuilder"/> implementation.</typeparam>
        /// <param name="builder">The GraphQL builder.</param>
        /// <returns>The GraphQL builder.</returns>
        [Obsolete]
        public static IGraphQLBuilder AddUserContextBuilder<TUserContextBuilder>(this IGraphQLBuilder builder)
            where TUserContextBuilder : class, IUserContextBuilder
        {
            builder.Services.AddSingleton<IUserContextBuilder, TUserContextBuilder>();

            return builder;
        }

        /// <summary>
        /// Set up a delegate to create the UserContext for each GraphQL request
        /// </summary>
        /// <typeparam name="TUserContext"></typeparam>
        /// <param name="builder">The GraphQL builder.</param>
        /// <param name="creator">A delegate used to create the user context from the <see cref="HttpContext"/>.</param>
        /// <returns>The GraphQL builder.</returns>
        [Obsolete]
        public static IGraphQLBuilder AddUserContextBuilder<TUserContext>(this IGraphQLBuilder builder, Func<HttpContext, TUserContext> creator)
            where TUserContext : class, IDictionary<string, object>
        {
            builder.Services.AddSingleton<IUserContextBuilder>(new UserContextBuilder<TUserContext>(creator));

            return builder;
        }

        /// <summary>
        /// Set up a delegate to create the UserContext for each GraphQL request
        /// </summary>
        /// <typeparam name="TUserContext"></typeparam>
        /// <param name="builder">The GraphQL builder.</param>
        /// <param name="creator">A delegate used to create the user context from the <see cref="HttpContext"/>.</param>
        /// <returns>The GraphQL builder.</returns>
        [Obsolete]
        public static IGraphQLBuilder AddUserContextBuilder<TUserContext>(this IGraphQLBuilder builder, Func<HttpContext, Task<TUserContext>> creator)
            where TUserContext : class, IDictionary<string, object>
        {
            builder.Services.AddSingleton<IUserContextBuilder>(new UserContextBuilder<TUserContext>(creator));

            return builder;
        }

        /// <summary>
        /// Set up default policy for matching endpoints. It is required when both GraphQL HTTP and
        /// GraphQL WebSockets middlewares are mapped to the same endpoint (by default 'graphql').
        /// </summary>
        /// <param name="builder">The GraphQL builder.</param>
        /// <returns>The GraphQL builder.</returns>
        [Obsolete]
        public static IGraphQLBuilder AddDefaultEndpointSelectorPolicy(this IGraphQLBuilder builder)
        {
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<MatcherPolicy, GraphQLDefaultEndpointSelectorPolicy>());

            return builder;
        }

        /// <summary>
        /// Adds an <see cref="IUserContextBuilder"/> as a singleton.
        /// </summary>
        /// <typeparam name="TUserContextBuilder">The type of the <see cref="IUserContextBuilder"/> implementation.</typeparam>
        /// <param name="builder">The GraphQL builder.</param>
        /// <returns>The GraphQL builder.</returns>
        public static DI.IGraphQLBuilder AddUserContextBuilder<TUserContextBuilder>(this DI.IGraphQLBuilder builder)
            where TUserContextBuilder : class, IUserContextBuilder
        {
            builder.Register<IUserContextBuilder, TUserContextBuilder>(DI.ServiceLifetime.Singleton);
            builder.ConfigureExecution(async options =>
            {
                if (options.UserContext == null || options.UserContext.Count == 0 && options.UserContext.GetType() == typeof(Dictionary<string, object>))
                {
                    var httpContext = options.RequestServices.GetRequiredService<IHttpContextAccessor>().HttpContext;
                    var contextBuilder = options.RequestServices.GetRequiredService<IUserContextBuilder>();
                    options.UserContext = await contextBuilder.BuildUserContext(httpContext);
                }
            });

            return builder;
        }

        /// <summary>
        /// Set up a delegate to create the UserContext for each GraphQL request
        /// </summary>
        /// <typeparam name="TUserContext"></typeparam>
        /// <param name="builder">The GraphQL builder.</param>
        /// <param name="creator">A delegate used to create the user context from the <see cref="HttpContext"/>.</param>
        /// <returns>The GraphQL builder.</returns>
        public static DI.IGraphQLBuilder AddUserContextBuilder<TUserContext>(this DI.IGraphQLBuilder builder, Func<HttpContext, TUserContext> creator)
            where TUserContext : class, IDictionary<string, object>
        {
            builder.Register<IUserContextBuilder>(new UserContextBuilder<TUserContext>(creator));
            builder.ConfigureExecution(options =>
            {
                if (options.UserContext == null || options.UserContext.Count == 0 && options.UserContext.GetType() == typeof(Dictionary<string, object>))
                {
                    var httpContext = options.RequestServices.GetRequiredService<IHttpContextAccessor>().HttpContext;
                    options.UserContext = creator(httpContext);
                }
            });

            return builder;
        }

        /// <summary>
        /// Set up a delegate to create the UserContext for each GraphQL request
        /// </summary>
        /// <typeparam name="TUserContext"></typeparam>
        /// <param name="builder">The GraphQL builder.</param>
        /// <param name="creator">A delegate used to create the user context from the <see cref="HttpContext"/>.</param>
        /// <returns>The GraphQL builder.</returns>
        public static DI.IGraphQLBuilder AddUserContextBuilder<TUserContext>(this DI.IGraphQLBuilder builder, Func<HttpContext, Task<TUserContext>> creator)
            where TUserContext : class, IDictionary<string, object>
        {
            builder.Register<IUserContextBuilder>(new UserContextBuilder<TUserContext>(creator));
            builder.ConfigureExecution(async options =>
            {
                if (options.UserContext == null || options.UserContext.Count == 0 && options.UserContext.GetType() == typeof(Dictionary<string, object>))
                {
                    var httpContext = options.RequestServices.GetRequiredService<IHttpContextAccessor>().HttpContext;
                    options.UserContext = await creator(httpContext);
                }
            });

            return builder;
        }

        /// <summary>
        /// Set up default policy for matching endpoints. It is required when both GraphQL HTTP and
        /// GraphQL WebSockets middlewares are mapped to the same endpoint (by default 'graphql').
        /// </summary>
        /// <param name="builder">The GraphQL builder.</param>
        /// <returns>The GraphQL builder.</returns>
        public static DI.IGraphQLBuilder AddDefaultEndpointSelectorPolicy(this DI.IGraphQLBuilder builder)
        {
            builder.TryRegister<MatcherPolicy, GraphQLDefaultEndpointSelectorPolicy>(DI.ServiceLifetime.Singleton);

            return builder;
        }
    }
}
