using Bookshelf.Application.Spi;
using Microsoft.Extensions.Logging;

namespace Bookshelf.Infrastructure.Adapters.NamingPatternPlugins;

/// <summary>
/// Registry for managing naming pattern plugins
/// </summary>
public sealed class NamingPatternPluginRegistry : INamingPatternPluginRegistry
{
    private readonly ILogger<NamingPatternPluginRegistry> _logger;
    private readonly List<INamingPatternPlugin> _plugins;
    private readonly DefaultNamingPatternPlugin _defaultPlugin;

    /// <summary>
    /// Initializes a new instance of the NamingPatternPluginRegistry class
    /// </summary>
    /// <param name="plugins">The collection of naming pattern plugins</param>
    /// <param name="logger">The logger</param>
    public NamingPatternPluginRegistry(
        IEnumerable<INamingPatternPlugin> plugins,
        ILogger<NamingPatternPluginRegistry> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _defaultPlugin = new DefaultNamingPatternPlugin();
        
        // Filter out the default plugin from the injected plugins and store specific plugins only
        _plugins = plugins
            .Where(p => p is not DefaultNamingPatternPlugin)
            .ToList();
        
        _logger.LogInformation("Initialized NamingPatternPluginRegistry with {Count} specific plugins", _plugins.Count);
    }

    /// <inheritdoc />
    public IReadOnlyList<INamingPatternPlugin> Plugins => _plugins.AsReadOnly();

    /// <inheritdoc />
    public INamingPatternPlugin? FindPlugin(IReadOnlyList<string> pdfFilePaths)
    {
        if (pdfFilePaths == null || pdfFilePaths.Count == 0)
        {
            _logger.LogDebug("No PDF files provided, using default plugin");
            return _defaultPlugin;
        }

        // Try to find a specific plugin that can handle the collection
        foreach (var plugin in _plugins)
        {
            if (plugin.CanHandle(pdfFilePaths))
            {
                _logger.LogDebug("Found matching plugin: {PluginName}", plugin.Name);
                return plugin;
            }
        }

        // Fall back to default plugin
        _logger.LogDebug("No specific plugin found, using default plugin");
        return _defaultPlugin;
    }
}
