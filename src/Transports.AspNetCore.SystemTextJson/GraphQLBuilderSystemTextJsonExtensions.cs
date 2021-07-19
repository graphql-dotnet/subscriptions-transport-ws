using System;
using System.Linq;
using System.Text.Json;
using GraphQL.Execution;
using GraphQL.Server.Transports.AspNetCore;
using GraphQL.Server.Transports.AspNetCore.SystemTextJson;
using GraphQL.SystemTextJson;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace GraphQL.Server
{
    public static class GraphQLBuilderSystemTextJsonExtensions
    {
        /// <summary>
        /// Adds a <see cref="IGraphQLRequestDeserializer"/> and a <see cref="IDocumentWriter"/>
        /// to the service collection with the provided configuration/settings.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configureDeserializerSettings">
        /// Action to further configure the request deserializer's settings.
        /// Affects reading of the JSON from the HTTP request the middleware processes.
        /// </param>
        /// <param name="configureSerializerSettings">
        /// Action to further configure the response serializer's settings.
        /// Affects JSON returned by the middleware.
        /// </param>
        /// <returns>GraphQL Builder.</returns>
        [Obsolete]
        public static IGraphQLBuilder AddSystemTextJson(this IGraphQLBuilder builder,
            Action<JsonSerializerOptions> configureDeserializerSettings = null,
            Action<JsonSerializerOptions> configureSerializerSettings = null)
        {
            builder.Services.AddSingleton<IGraphQLRequestDeserializer>(p => new GraphQLRequestDeserializer(configureDeserializerSettings ?? (_ => { })));
            builder.Services.Replace(ServiceDescriptor.Singleton<IDocumentWriter>(p => new DocumentWriter(opt =>
            {
                opt.Converters.Add(new OperationMessageConverter());
                configureSerializerSettings?.Invoke(opt);
            }, p.GetService<IErrorInfoProvider>() ?? new ErrorInfoProvider())));

            return builder;
        }

        /// <summary>
        /// Adds a <see cref="IGraphQLRequestDeserializer"/> and a <see cref="IDocumentWriter"/>
        /// to the service collection with the provided configuration/settings.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configureDeserializerSettings">
        /// Action to further configure the request deserializer's settings.
        /// Affects reading of the JSON from the HTTP request the middleware processes.
        /// </param>
        /// <param name="configureSerializerSettings">
        /// Action to further configure the response serializer's settings.
        /// Affects JSON returned by the middleware.
        /// </param>
        /// <returns>GraphQL Builder.</returns>
        public static DI.IGraphQLBuilder AddSystemTextJson(this DI.IGraphQLBuilder builder,
            Action<JsonSerializerOptions> configureDeserializerSettings = null,
            Action<JsonSerializerOptions> configureSerializerSettings = null)
        {
            builder.Register<IGraphQLRequestDeserializer>(services => new GraphQLRequestDeserializer(configureDeserializerSettings ?? (_ => { })), DI.ServiceLifetime.Singleton);
            SystemTextJson.GraphQLBuilderExtensions.AddSystemTextJson(builder, configureSerializerSettings);
            builder.Configure<JsonSerializerOptions>(opt =>
            {
                if (!opt.Converters.Any(y => y.GetType() == typeof(OperationMessageConverter)))
                {
                    opt.Converters.Add(new OperationMessageConverter());
                }
            });

            return builder;
        }
    }
}
