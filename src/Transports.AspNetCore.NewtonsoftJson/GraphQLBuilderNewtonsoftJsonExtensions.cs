using System;
using GraphQL.Execution;
using GraphQL.NewtonsoftJson;
using GraphQL.Server.Transports.AspNetCore;
using GraphQL.Server.Transports.AspNetCore.NewtonsoftJson;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;

namespace GraphQL.Server
{
    public static class GraphQLBuilderNewtonsoftJsonExtensions
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
        public static IGraphQLBuilder AddNewtonsoftJson(this IGraphQLBuilder builder,
            Action<JsonSerializerSettings> configureDeserializerSettings = null,
            Action<JsonSerializerSettings> configureSerializerSettings = null)
        {
            builder.Services.AddSingleton<IGraphQLRequestDeserializer>(p => new GraphQLRequestDeserializer(configureDeserializerSettings ?? (_ => { })));
            builder.Services.Replace(ServiceDescriptor.Singleton<IDocumentWriter>(p => new DocumentWriter(configureSerializerSettings ?? (_ => { }), p.GetService<IErrorInfoProvider>() ?? new ErrorInfoProvider())));

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
        public static DI.IGraphQLBuilder AddNewtonsoftJson(this DI.IGraphQLBuilder builder,
            Action<JsonSerializerSettings> configureDeserializerSettings = null,
            Action<JsonSerializerSettings> configureSerializerSettings = null)
        {
            builder.Register<IGraphQLRequestDeserializer>(p => new GraphQLRequestDeserializer(configureDeserializerSettings ?? (_ => { })), DI.ServiceLifetime.Singleton);
            NewtonsoftJson.GraphQLBuilderExtensions.AddNewtonsoftJson(builder, configureSerializerSettings);

            return builder;
        }
    }
}
