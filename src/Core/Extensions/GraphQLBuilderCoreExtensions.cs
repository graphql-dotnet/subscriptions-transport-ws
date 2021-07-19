using System;
using System.Linq;
using System.Reflection;
using GraphQL.DataLoader;
using GraphQL.Execution;
using GraphQL.Types;
using GraphQL.Types.Relay;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace GraphQL.Server
{
    /// <summary>
    /// GraphQL specific extension methods for <see cref="IGraphQLBuilder"/>.
    /// </summary>
    public static class GraphQLBuilderCoreExtensions
    {
        /// <summary>
        /// Registers an instance of <see cref="BasicGraphQLExecuter{TSchema}"/> as <see cref="IGraphQLExecuter{TSchema}"/> for
        /// use with <see cref="GraphQLOptions"/>. It is recommended to use <see cref="IDocumentExecuter"/> directly rather than
        /// use the <see cref="IGraphQLExecuter{TSchema}"/> interface.
        /// </summary>
        public static DI.IGraphQLBuilder AddServer(this DI.IGraphQLBuilder builder, Action<GraphQLOptions> configureOptions)
        {
            builder.TryRegister(typeof(IGraphQLExecuter<>), typeof(BasicGraphQLExecuter<>), DI.ServiceLifetime.Transient);
            builder.TryRegister(typeof(IGraphQLExecuter), typeof(BasicGraphQLExecuter<ISchema>), DI.ServiceLifetime.Transient);
            builder.Configure(configureOptions);
            return builder;
        }

        /// <summary>
        /// Registers an instance of <see cref="BasicGraphQLExecuter{TSchema}"/> as <see cref="IGraphQLExecuter{TSchema}"/> for
        /// use with <see cref="GraphQLOptions"/>. It is recommended to use <see cref="IDocumentExecuter"/> directly rather than
        /// use the <see cref="IGraphQLExecuter{TSchema}"/> interface.
        /// </summary>
        public static DI.IGraphQLBuilder AddServer(this DI.IGraphQLBuilder builder, Action<GraphQLOptions, IServiceProvider> configureOptions = null)
        {
            builder.TryRegister(typeof(IGraphQLExecuter<>), typeof(BasicGraphQLExecuter<>), DI.ServiceLifetime.Transient);
            builder.TryRegister(typeof(IGraphQLExecuter), typeof(BasicGraphQLExecuter<ISchema>), DI.ServiceLifetime.Transient);
            builder.Configure(configureOptions);
            return builder;
        }

        /// <summary>
        /// Provides the ability to configure <see cref="ErrorInfoProviderOptions"/> for <see cref="DefaultErrorInfoProvider"/>.
        /// </summary>
        /// <param name="builder">GraphQL builder used for GraphQL specific extension methods as 'this' argument.</param>
        /// <param name="configureOptions">Action to configure the <see cref="ErrorInfoProviderOptions"/>.</param>
        /// <returns>Reference to <paramref name="builder"/>.</returns>
        [Obsolete]
        public static IGraphQLBuilder AddErrorInfoProvider(this IGraphQLBuilder builder, Action<ErrorInfoProviderOptions> configureOptions)
            => AddErrorInfoProvider<DefaultErrorInfoProvider>(builder, configureOptions);

        /// <summary>
        /// Provides the ability to configure <see cref="ErrorInfoProviderOptions"/> for the specified <typeparamref name="TErrorInfoProvider"/>.
        /// </summary>
        /// <typeparam name="TErrorInfoProvider">The <see cref="IErrorInfoProvider"/> implementation.</typeparam>
        /// <param name="builder">GraphQL builder used for GraphQL specific extension methods as 'this' argument.</param>
        /// <param name="configureOptions">Action to configure the <see cref="ErrorInfoProviderOptions"/>.</param>
        /// <returns>Reference to <paramref name="builder"/>.</returns>
        [Obsolete]
        public static IGraphQLBuilder AddErrorInfoProvider<TErrorInfoProvider>(this IGraphQLBuilder builder, Action<ErrorInfoProviderOptions> configureOptions)
            where TErrorInfoProvider : DefaultErrorInfoProvider
        {
            if (configureOptions == null)
                throw new ArgumentNullException(nameof(configureOptions));

            return builder.AddErrorInfoProvider<TErrorInfoProvider>((opt, _) => configureOptions(opt));
        }

        /// <summary>
        /// Provides the ability to configure <see cref="ErrorInfoProviderOptions"/> for <see cref="DefaultErrorInfoProvider"/>.
        /// </summary>
        /// <param name="builder">GraphQL builder used for GraphQL specific extension methods as 'this' argument.</param>
        /// <param name="configureOptions">Action to configure the <see cref="ErrorInfoProviderOptions"/>.</param>
        /// <returns>Reference to <paramref name="builder"/>.</returns>
        [Obsolete]
        public static IGraphQLBuilder AddErrorInfoProvider(this IGraphQLBuilder builder, Action<ErrorInfoProviderOptions, IServiceProvider> configureOptions)
            => AddErrorInfoProvider<DefaultErrorInfoProvider>(builder, configureOptions);

        /// <summary>
        /// Provides the ability to configure <see cref="ErrorInfoProviderOptions"/> for the specified <typeparamref name="TErrorInfoProvider"/>.
        /// </summary>
        /// <typeparam name="TErrorInfoProvider">The <see cref="IErrorInfoProvider"/> implementation.</typeparam>
        /// <param name="builder">GraphQL builder used for GraphQL specific extension methods as 'this' argument.</param>
        /// <param name="configureOptions">Action to configure the <see cref="ErrorInfoProviderOptions"/>.</param>
        /// <returns>Reference to <paramref name="builder"/>.</returns>
        [Obsolete]
        public static IGraphQLBuilder AddErrorInfoProvider<TErrorInfoProvider>(this IGraphQLBuilder builder, Action<ErrorInfoProviderOptions, IServiceProvider> configureOptions)
            where TErrorInfoProvider : DefaultErrorInfoProvider
        {
            if (configureOptions == null)
                throw new ArgumentNullException(nameof(configureOptions));

            // This is used instead of "normal" services.Configure(configureOptions) to pass IServiceProvider to user code.
            builder.Services.AddSingleton<IConfigureOptions<ErrorInfoProviderOptions>>(x => new ConfigureNamedOptions<ErrorInfoProviderOptions>(Options.DefaultName, opt => configureOptions(opt, x)));
            builder.Services.TryAddSingleton<IErrorInfoProvider, TErrorInfoProvider>();
            builder.Services.AddOptions();

            return builder;
        }

        /// <summary>
        /// Add required services for GraphQL data loader support
        /// </summary>
        /// <param name="builder">GraphQL builder used for GraphQL specific extension methods as 'this' argument.</param>
        /// <returns>Reference to <paramref name="builder"/>.</returns>
        [Obsolete]
        public static IGraphQLBuilder AddDataLoader(this IGraphQLBuilder builder)
        {
            builder.Services.TryAddSingleton<IDataLoaderContextAccessor, DataLoaderContextAccessor>();
            builder.Services.AddSingleton<IDocumentExecutionListener, DataLoaderDocumentListener>();

            return builder;
        }

        /// <summary>
        /// Add all types that implement <seealso cref="IGraphType"/> in the calling assembly
        /// </summary>
        /// <param name="builder">GraphQL builder used for GraphQL specific extension methods as 'this' argument.</param>
        /// <param name="serviceLifetime">The service lifetime to register the GraphQL types with</param>
        /// <returns>Reference to <paramref name="builder"/>.</returns>
        [Obsolete]
        public static IGraphQLBuilder AddGraphTypes(
            this IGraphQLBuilder builder,
            ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            => builder.AddGraphTypes(Assembly.GetCallingAssembly(), serviceLifetime);

        /// <summary>
        /// Add all types that implement <seealso cref="IGraphType"/> in the assembly which <paramref name="typeFromAssembly"/> belongs to
        /// </summary>
        /// <param name="builder">GraphQL builder used for GraphQL specific extension methods as 'this' argument.</param>
        /// <param name="typeFromAssembly">The type from assembly to register all GraphQL types from</param>
        /// <param name="serviceLifetime">The service lifetime to register the GraphQL types with</param>
        /// <returns>Reference to <paramref name="builder"/>.</returns>
        [Obsolete]
        public static IGraphQLBuilder AddGraphTypes(
            this IGraphQLBuilder builder,
            Type typeFromAssembly,
            ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            => builder.AddGraphTypes(typeFromAssembly.Assembly, serviceLifetime);

        /// <summary>
        /// Add all types that implement <seealso cref="IGraphType"/> in the specified assembly
        /// </summary>
        /// <param name="builder">GraphQL builder used for GraphQL specific extension methods as 'this' argument.</param>
        /// <param name="assembly">The assembly to register all GraphQL types from</param>
        /// <param name="serviceLifetime">The service lifetime to register the GraphQL types with</param>
        /// <returns>Reference to <paramref name="builder"/>.</returns>
        [Obsolete]
        public static IGraphQLBuilder AddGraphTypes(
            this IGraphQLBuilder builder,
            Assembly assembly,
            ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
        {
            // Register all GraphQL types
            foreach (var type in assembly.GetTypes()
                .Where(x => !x.IsAbstract && typeof(IGraphType).IsAssignableFrom(x)))
            {
                builder.Services.TryAdd(new ServiceDescriptor(type, type, serviceLifetime));
            }

            return builder;
        }

        /// <summary>
        /// Adds the GraphQL Relay types <see cref="ConnectionType{TNodeType}"/>, <see cref="EdgeType{TNodeType}"/>
        /// and <see cref="PageInfoType"/>.
        /// </summary>
        /// <param name="builder">GraphQL builder used for GraphQL specific extension methods as 'this' argument.</param>
        /// <returns>Reference to <paramref name="builder"/>.</returns>
        [Obsolete]
        public static IGraphQLBuilder AddRelayGraphTypes(this IGraphQLBuilder builder)
        {
            builder
                .Services
                .AddSingleton(typeof(ConnectionType<>))
                .AddSingleton(typeof(EdgeType<>))
                .AddSingleton<PageInfoType>();

            return builder;
        }
    }
}
