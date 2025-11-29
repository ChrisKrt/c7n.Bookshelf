using System.Diagnostics;

namespace Bookshelf.Application.Core.Plugins;

/// <summary>
/// Factory for creating and selecting appropriate naming pattern plugins
/// </summary>
public sealed class NamingPatternPluginFactory
{
    private readonly List<INamingPatternPlugin> _plugins;
    private readonly INamingPatternPlugin _defaultPlugin;

    /// <summary>
    /// Initializes a new instance of the NamingPatternPluginFactory class
    /// </summary>
    public NamingPatternPluginFactory()
    {
        _defaultPlugin = new DefaultNamingPatternPlugin();
        _plugins =
        [
            new TeilBasedNamingPatternPlugin(),
            new HanserNamingPatternPlugin(),
            new MitpNamingPatternPlugin(),
            new OReillyNamingPatternPlugin(),
            new WichmannNamingPatternPlugin()
        ];
        
        // Sort by priority (highest first)
        _plugins = _plugins.OrderByDescending(p => p.Priority).ToList();
    }

    /// <summary>
    /// Gets all registered plugins
    /// </summary>
    public IReadOnlyList<INamingPatternPlugin> Plugins => _plugins;

    /// <summary>
    /// Detects the appropriate naming pattern plugin for the given PDF files
    /// </summary>
    /// <param name="pdfFilePaths">The PDF file paths to analyze</param>
    /// <returns>The detected plugin, or the default plugin if none match</returns>
    public INamingPatternPlugin DetectPlugin(IReadOnlyList<string> pdfFilePaths)
    {
        Debug.Assert(pdfFilePaths != null, "PDF file paths must not be null");

        foreach (var plugin in _plugins)
        {
            if (plugin.CanHandle(pdfFilePaths))
            {
                return plugin;
            }
        }

        return _defaultPlugin;
    }

    /// <summary>
    /// Gets a plugin by name
    /// </summary>
    /// <param name="pluginName">The plugin name</param>
    /// <returns>The plugin if found, null otherwise</returns>
    public INamingPatternPlugin? GetPlugin(string pluginName)
    {
        Debug.Assert(!string.IsNullOrWhiteSpace(pluginName), "Plugin name must not be null or whitespace");

        return _plugins.FirstOrDefault(p => 
            p.PluginName.Equals(pluginName, StringComparison.OrdinalIgnoreCase));
    }
}
