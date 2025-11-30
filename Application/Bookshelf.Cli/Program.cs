using Bookshelf.Application;
using Bookshelf.Cli.Commands;
using Bookshelf.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using Spectre.Console.Cli;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.File(
        path: "logs/bookshelf-.log",
        rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

try
{
    Log.Information("Starting Bookshelf CLI application");

    // Create service collection and register services
    var services = new ServiceCollection();
    
    // Register logging
    services.AddLogging(loggingBuilder =>
    {
        loggingBuilder.AddSerilog(dispose: true);
    });
    
    // Register application and infrastructure services
    services.AddApplicationServices();
    services.AddInfrastructureServices();
    
    // Register commands
    services.AddTransient<ConsolidateCommand>();
    services.AddTransient<ListCommand>();
    
    // Build service provider
    var serviceProvider = services.BuildServiceProvider();

    // Create and configure the command app
    var app = new CommandApp(new TypeRegistrar(serviceProvider));
    
    app.Configure(config =>
    {
        config.SetApplicationName("bookshelf");
        
        config.AddCommand<ConsolidateCommand>("consolidate")
            .WithDescription("Consolidate scattered PDF files into a single bookshelf")
            .WithExample("consolidate", "/path/to/source", "/path/to/bookshelf");

        config.AddCommand<ListCommand>("list")
            .WithDescription("List all books in a bookshelf")
            .WithExample("list", "/path/to/bookshelf")
            .WithExample("list", "/path/to/bookshelf", "--details")
            .WithExample("list", "/path/to/bookshelf", "--filter", "Python")
            .WithExample("list", "/path/to/bookshelf", "--sort", "size", "--reverse");
    });

    return await app.RunAsync(args);
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
    return 1;
}
finally
{
    await Log.CloseAndFlushAsync();
}

/// <summary>
/// Type registrar for dependency injection with Spectre.Console.Cli
/// </summary>
internal sealed class TypeRegistrar : ITypeRegistrar
{
    private readonly IServiceProvider _serviceProvider;

    public TypeRegistrar(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ITypeResolver Build()
    {
        return new TypeResolver(_serviceProvider);
    }

    public void Register(Type service, Type implementation)
    {
        // Not used in this implementation
    }

    public void RegisterInstance(Type service, object implementation)
    {
        // Not used in this implementation
    }

    public void RegisterLazy(Type service, Func<object> factory)
    {
        // Not used in this implementation
    }
}

/// <summary>
/// Type resolver for dependency injection with Spectre.Console.Cli
/// </summary>
internal sealed class TypeResolver : ITypeResolver
{
    private readonly IServiceProvider _serviceProvider;

    public TypeResolver(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public object? Resolve(Type? type)
    {
        return type == null ? null : _serviceProvider.GetService(type);
    }

    public void Dispose()
    {
        if (_serviceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}