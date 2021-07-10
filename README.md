# GraphQL for .NET - Subscription Transport WebSockets

![License](https://img.shields.io/github/license/graphql-dotnet/server)

[![Join the chat at https://gitter.im/graphql-dotnet/graphql-dotnet](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/graphql-dotnet/graphql-dotnet?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

[![Run code tests](https://github.com/graphql-dotnet/server/actions/workflows/test.yml/badge.svg)](https://github.com/graphql-dotnet/server/actions/workflows/test.yml)
[![Build artifacts](https://github.com/graphql-dotnet/server/actions/workflows/build.yml/badge.svg)](https://github.com/graphql-dotnet/server/actions/workflows/build.yml)
[![Publish release](https://github.com/graphql-dotnet/server/actions/workflows/publish.yml/badge.svg)](https://github.com/graphql-dotnet/server/actions/workflows/publish.yml)
[![CodeQL analysis](https://github.com/graphql-dotnet/server/actions/workflows/codeql-analysis.yml/badge.svg)](https://github.com/graphql-dotnet/server/actions/workflows/codeql-analysis.yml)

[![codecov](https://codecov.io/gh/graphql-dotnet/server/branch/master/graph/badge.svg?token=ZBcVYq7hz4)](https://codecov.io/gh/graphql-dotnet/server)
[![Total alerts](https://img.shields.io/lgtm/alerts/g/graphql-dotnet/server.svg?logo=lgtm&logoWidth=18)](https://lgtm.com/projects/g/graphql-dotnet/server/alerts/)
[![Language grade: C#](https://img.shields.io/lgtm/grade/csharp/g/graphql-dotnet/server.svg?logo=lgtm&logoWidth=18)](https://lgtm.com/projects/g/graphql-dotnet/server/context:csharp)

![Activity](https://img.shields.io/github/commit-activity/w/graphql-dotnet/server)
![Activity](https://img.shields.io/github/commit-activity/m/graphql-dotnet/server)
![Activity](https://img.shields.io/github/commit-activity/y/graphql-dotnet/server)

![Size](https://img.shields.io/github/repo-size/graphql-dotnet/server)

GraphQL ASP.NET Core server on top of [GraphQL.NET](https://github.com/graphql-dotnet/graphql-dotnet).
Transport compatible with [Apollo](https://github.com/apollographql/subscriptions-transport-ws) subscription protocol.

Provides the following packages:

| Package                                              | Downloads                                                                                                                                                                             | NuGet Latest                                                                                                                                                                         |
|------------------------------------------------------|---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| GraphQL.Server.All                                   | [![Nuget](https://img.shields.io/nuget/dt/GraphQL.Server.All)](https://www.nuget.org/packages/GraphQL.Server.All)                                                                     | [![Nuget](https://img.shields.io/nuget/v/GraphQL.Server.All)](https://www.nuget.org/packages/GraphQL.Server.All)                                                                     |
| GraphQL.Server.Core                                  | [![Nuget](https://img.shields.io/nuget/dt/GraphQL.Server.Core)](https://www.nuget.org/packages/GraphQL.Server.Core)                                                                   | [![Nuget](https://img.shields.io/nuget/v/GraphQL.Server.Core)](https://www.nuget.org/packages/GraphQL.Server.Core)                                                                   |
| GraphQL.Server.Transports.AspNetCore                 | [![Nuget](https://img.shields.io/nuget/dt/GraphQL.Server.Transports.AspNetCore)](https://www.nuget.org/packages/GraphQL.Server.Transports.AspNetCore)                                 | [![Nuget](https://img.shields.io/nuget/v/GraphQL.Server.Transports.AspNetCore)](https://www.nuget.org/packages/GraphQL.Server.Transports.AspNetCore)                                 |
| GraphQL.Server.Transports.AspNetCore.NewtonsoftJson  | [![Nuget](https://img.shields.io/nuget/dt/GraphQL.Server.Transports.AspNetCore.NewtonsoftJson)](https://www.nuget.org/packages/GraphQL.Server.Transports.AspNetCore.NewtonsoftJson)   | [![Nuget](https://img.shields.io/nuget/v/GraphQL.Server.Transports.AspNetCore.NewtonsoftJson)](https://www.nuget.org/packages/GraphQL.Server.Transports.AspNetCore.NewtonsoftJson)   |
| GraphQL.Server.Transports.AspNetCore.SystemTextJson  | [![Nuget](https://img.shields.io/nuget/dt/GraphQL.Server.Transports.AspNetCore.SystemTextJson)](https://www.nuget.org/packages/GraphQL.Server.Transports.AspNetCore.SystemTextJson)   | [![Nuget](https://img.shields.io/nuget/v/GraphQL.Server.Transports.AspNetCore.SystemTextJson)](https://www.nuget.org/packages/GraphQL.Server.Transports.AspNetCore.SystemTextJson)   |
| GraphQL.Server.Transports.Subscriptions.Abstractions | [![Nuget](https://img.shields.io/nuget/dt/GraphQL.Server.Transports.Subscriptions.Abstractions)](https://www.nuget.org/packages/GraphQL.Server.Transports.Subscriptions.Abstractions) | [![Nuget](https://img.shields.io/nuget/v/GraphQL.Server.Transports.Subscriptions.Abstractions)](https://www.nuget.org/packages/GraphQL.Server.Transports.Subscriptions.Abstractions) |
| GraphQL.Server.Transports.Subscriptions.WebSockets <br/> *formerly  known as `GraphQL.Server.Transports.WebSockets`* | [![Nuget](https://img.shields.io/nuget/dt/GraphQL.Server.Transports.Subscriptions.WebSockets)](https://www.nuget.org/packages/GraphQL.Server.Transports.Subscriptions.WebSockets)     | [![Nuget](https://img.shields.io/nuget/v/GraphQL.Server.Transports.Subscriptions.WebSockets)](https://www.nuget.org/packages/GraphQL.Server.Transports.Subscriptions.WebSockets)     |
| GraphQL.Server.Ui.Altair                             | [![Nuget](https://img.shields.io/nuget/dt/GraphQL.Server.Ui.Altair)](https://www.nuget.org/packages/GraphQL.Server.Ui.Altair)                                                         | [![Nuget](https://img.shields.io/nuget/v/GraphQL.Server.Ui.Altair)](https://www.nuget.org/packages/GraphQL.Server.Ui.Altair)                                                         |
| GraphQL.Server.Ui.Playground                         | [![Nuget](https://img.shields.io/nuget/dt/GraphQL.Server.Ui.Playground)](https://www.nuget.org/packages/GraphQL.Server.Ui.Playground)                                                 | [![Nuget](https://img.shields.io/nuget/v/GraphQL.Server.Ui.Playground)](https://www.nuget.org/packages/GraphQL.Server.Ui.Playground)                                                 |
| GraphQL.Server.Ui.GraphiQL                           | [![Nuget](https://img.shields.io/nuget/dt/GraphQL.Server.Ui.GraphiQL)](https://www.nuget.org/packages/GraphQL.Server.Ui.GraphiQL)                                                     | [![Nuget](https://img.shields.io/nuget/v/GraphQL.Server.Ui.GraphiQL)](https://www.nuget.org/packages/GraphQL.Server.Ui.GraphiQL)                                                     |
| GraphQL.Server.Ui.Voyager                            | [![Nuget](https://img.shields.io/nuget/dt/GraphQL.Server.Ui.Voyager)](https://www.nuget.org/packages/GraphQL.Server.Ui.Voyager)                                                       | [![Nuget](https://img.shields.io/nuget/v/GraphQL.Server.Ui.Voyager)](https://www.nuget.org/packages/GraphQL.Server.Ui.Voyager)                                                       |
| GraphQL.Server.Authorization.AspNetCore              | [![Nuget](https://img.shields.io/nuget/dt/GraphQL.Server.Authorization.AspNetCore)](https://www.nuget.org/packages/GraphQL.Server.Authorization.AspNetCore)                           | [![Nuget](https://img.shields.io/nuget/v/GraphQL.Server.Authorization.AspNetCore)](https://www.nuget.org/packages/GraphQL.Server.Authorization.AspNetCore)                           |

You can install the latest stable versions via [NuGet](https://www.nuget.org/packages/GraphQL.Server.Transports.AspNetCore/).
Also you can get all preview versions from [GitHub Packages](https://github.com/orgs/graphql-dotnet/packages?repo_name=server).
Note that GitHub requires authentication to consume the feed. See more information [here](https://docs.github.com/en/free-pro-team@latest/packages/publishing-and-managing-packages/about-github-packages#authenticating-to-github-packages).

## Getting started

> **TL;DR**
> Install [GraphQL.Server.All](https://www.nuget.org/packages/GraphQL.Server.All) meta package with all the packages you need to get started.

#### 1. HTTP middleware for GraphQL

For just the HTTP middleware:

```
> dotnet add package GraphQL.Server.Transports.AspNetCore
```

The HTTP middleware needs an `IGraphQLRequestDeserializer` implementation.

.NET Core 3+:

```
> dotnet add package GraphQL.Server.Transports.AspNetCore.SystemTextJson
```

Legacy (prior to .NET Core 3):

```
> dotnet add package GraphQL.Server.Transports.AspNetCore.NewtonsoftJson
```

Or you can use your own `IGraphQLRequestDeserializer` implementation.

For more information on how to migrate from `Newtonsoft.Json` to `System.Text.Json` see
[this article](https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-migrate-from-newtonsoft-how-to).

#### 2. WebSockets transport for subscriptions

For the WebSocket subscription protocol (depends on above) middleware:

```
> dotnet add package GraphQL.Server.Transports.Subscriptions.WebSockets
```

#### 3. Authorization

For integration of GraphQL.NET validation subsystem into ASP.NET Core:

```
> dotnet add package Authorization.AspNetCore
```

#### 4. UI integration

For the UI middleware/s:

```
> dotnet add package GraphQL.Server.Ui.Altair
> dotnet add package GraphQL.Server.Ui.GraphiQL
> dotnet add package GraphQL.Server.Ui.Playground
> dotnet add package GraphQL.Server.Ui.Voyager
```

## Configure

See the sample project's [Startup.cs](samples/Samples.Server/Startup.cs) or [StartupWithRouting.cs](samples/Samples.Server/StartupWithRouting.cs) for full details.
More information about ASP.NET Core routing [here](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/routing?view=aspnetcore-3.1).

1. Without routing:

```c#
public void ConfigureServices(IServiceCollection services)
{
    // Add GraphQL services and configure options
    services
        .AddSingleton<IChat, Chat>()
        .AddSingleton<ChatSchema>()
        .AddGraphQL((options, provider) =>
        {
            options.EnableMetrics = Environment.IsDevelopment();
            var logger = provider.GetRequiredService<ILogger<Startup>>();
            options.UnhandledExceptionDelegate = ctx => logger.LogError("{Error} occurred", ctx.OriginalException.Message);
        })
        // Add required services for GraphQL request/response de/serialization
        .AddSystemTextJson() // For .NET Core 3+
        .AddNewtonsoftJson() // For everything else
        .AddErrorInfoProvider(opt => opt.ExposeExceptionStackTrace = Environment.IsDevelopment())
        .AddWebSockets() // Add required services for web socket support
        .AddDataLoader() // Add required services for DataLoader support
        .AddGraphTypes(typeof(ChatSchema)) // Add all IGraphType implementors in assembly which ChatSchema exists 
}

public void Configure(IApplicationBuilder app)
{
    // this is required for websockets support
    app.UseWebSockets();

    // use websocket middleware for ChatSchema at default path /graphql
    app.UseGraphQLWebSockets<ChatSchema>();

    // use HTTP middleware for ChatSchema at default path /graphql
    app.UseGraphQL<ChatSchema>();

    // use GraphiQL middleware at default path /ui/graphiql with default options
    app.UseGraphQLGraphiQL();

    // use GraphQL Playground middleware at default path /ui/playground with default options
    app.UseGraphQLPlayground();

    // use Altair middleware at default path /ui/altair with default options
    app.UseGraphQLAltair();
    
    // use Voyager middleware at default path /ui/voyager with default options
    app.UseGraphQLVoyager();
}
```

2. With routing:

```c#
public void ConfigureServices(IServiceCollection services)
{
    // Add GraphQL services and configure options
    services
        .AddRouting()
        .AddSingleton<IChat, Chat>()
        .AddSingleton<ChatSchema>()
        .AddGraphQL((options, provider) =>
        {
            options.EnableMetrics = Environment.IsDevelopment();
            var logger = provider.GetRequiredService<ILogger<Startup>>();
            options.UnhandledExceptionDelegate = ctx => logger.LogError("{Error} occurred", ctx.OriginalException.Message);
        })
        // It is required when both GraphQL HTTP and GraphQL WebSockets middlewares are mapped to the same endpoint (by default 'graphql').
        .AddDefaultEndpointSelectorPolicy()
        // Add required services for GraphQL request/response de/serialization
        .AddSystemTextJson() // For .NET Core 3+
        .AddNewtonsoftJson() // For everything else
        .AddErrorInfoProvider(opt => opt.ExposeExceptionStackTrace = Environment.IsDevelopment())
        .AddWebSockets() // Add required services for web socket support
        .AddDataLoader() // Add required services for DataLoader support
        .AddGraphTypes(typeof(ChatSchema)); // Add all IGraphType implementors in assembly which ChatSchema exists 
}

public void Configure(IApplicationBuilder app)
{
    // this is required for websockets support
    app.UseWebSockets();

    // this is required for ASP.NET Core routing
    app.UseRouting();
            
    app.UseEndpoints(endpoints =>
    {
        // map websocket middleware for ChatSchema at default path /graphql
        endpoints.MapGraphQLWebSockets<ChatSchema>();

        // map HTTP middleware for ChatSchema at default path /graphql
        endpoints.MapGraphQL<ChatSchema, GraphQLHttpMiddlewareWithLogs<ChatSchema>>();

        // map GraphQL Playground middleware at default path /ui/playground with default options
        endpoints.MapGraphQLPlayground();

        // map GraphiQL middleware at default path /ui/graphiql with default options
        endpoints.MapGraphQLGraphiQL();

        // map Altair middleware at default path /ui/altair with default options
        endpoints.MapGraphQLAltair();

        // map Voyager middleware at default path /ui/voyager with default options
        endpoints.MapGraphQLVoyager();
}
```

### UserContext and resolvers

`UserContext` of your resolver will be type of `MessageHandlingContext`. You can
access the properties including your actual `UserContext` by using the
`Get<YourContextType>("UserContext")` method. This will read the context from the properties of
`MessageHandlingContext`. You can add any other properties as to the context in
`IOperationMessageListeners`. See the sample for example of injecting `ClaimsPrincipal`.

## Sample

[Samples.Server](samples/Samples.Server/Startup.cs) shows a simple Chat example demonstrating the subscription transport.
It supports various GraphQL client IDEs (by default opening GraphQL Playground).

Here are some example queries to get started. Use three browser tabs or better yet windows 
to view the changes.

### Subscription 1

Query:

```
subscription MessageAddedByUser($id:String!) {
  messageAddedByUser(id: $id) {
    from { id displayName }
    content
  }
}
```

Variables:

```
{
  "id": "1"
}
```

### Subscription 2

```
subscription MessageAdded {
  messageAdded {
    from { id displayName }
    content
  }
}
```

### Mutation

Query:

```
mutation AddMessage($message: MessageInputType!) {
  addMessage(message: $message) {
    from {
      id
      displayName
    }
    content
  }
}
```

Variables: 

```
{
  "message": {
    "content": "Message",
    "fromId": "1"
  }
}
```

## API changes

You can see the changes in public APIs using [fuget.org](https://www.fuget.org/packages/GraphQL.Server.Transports.AspNetCore/4.3.0/lib/netstandard2.0/diff/3.4.1/).
