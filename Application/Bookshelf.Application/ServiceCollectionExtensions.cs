using Bookshelf.Application.Api;
using Bookshelf.Application.Core.Plugins;
using Bookshelf.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Bookshelf.Application;

/// <summary>
/// Extension methods for registering application services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds application services to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register plugin factory as singleton (plugins are stateless)
        services.AddSingleton<INamingPatternPluginFactory, NamingPatternPluginFactory>();
        
        // Register application services
        services.AddTransient<IBookshelfConsolidationService, BookshelfConsolidationService>();
        services.AddTransient<IBookshelfListService, BookshelfListService>();
        
        return services;
    }
}
