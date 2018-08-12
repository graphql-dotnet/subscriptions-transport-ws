using GraphQL.Server.Transports.AspNetCore;
using GraphQL.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace GraphQL.Server
{
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Add the GraphQL middleware to the request pipeline
        /// </summary>
        /// <typeparam name="TSchema">The implementation of <see cref="ISchema"/> to use</typeparam>
        /// <param name="builder"></param>
        /// <param name="path"></param>
        /// <returns>The <see cref="IApplicationBuilder"/> received as parameter</returns>
        public static IApplicationBuilder UseGraphQL<TSchema>(this IApplicationBuilder builder,
            PathString path)
            where TSchema : ISchema
        {
            return builder.UseMiddleware<GraphQLHttpMiddleware<TSchema>>(path);
        }
    }
}
