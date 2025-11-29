using Bookshelf.Application.Spi;
using Bookshelf.Infrastructure.Adapters;
using Microsoft.Extensions.DependencyInjection;

namespace Bookshelf.Infrastructure;

/// <summary>
/// Extension methods for registering infrastructure services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds infrastructure services to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddSingleton<IFileSystemAdapter, FileSystemAdapter>();
        services.AddSingleton<IPdfMerger, PdfMerger>();
        
        return services;
    }
}
