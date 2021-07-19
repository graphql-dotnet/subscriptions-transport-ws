using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GraphQL.Server.Transports.WebSockets.Tests
{
    public class TestStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<TestSchema>();
#pragma warning disable CS0612 // Type or member is obsolete
            services.AddGraphQL()
                .AddWebSockets();
#pragma warning restore CS0612 // Type or member is obsolete
            services.AddLogging(builder =>
            {
                // prevent writing errors to Console.Error during tests (required for testing on ubuntu)
                builder.ClearProviders();
                builder.AddDebug();
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseWebSockets();
            app.UseGraphQLWebSockets<TestSchema>("/graphql");
        }
    }
}
