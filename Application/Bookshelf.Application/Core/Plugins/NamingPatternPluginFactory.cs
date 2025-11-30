namespace Bookshelf.Application.Core.Plugins;

/// <summary>
/// Factory for creating and selecting appropriate naming pattern plugins
/// </summary>
public sealed class NamingPatternPluginFactory : INamingPatternPluginFactory
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
    /// Initializes a new instance of the NamingPatternPluginFactory class with custom plugins
    /// </summary>
    /// <param name="plugins">The plugins to register</param>
    /// <param name="defaultPlugin">The default plugin to use when no plugin matches</param>
    /// <exception cref="ArgumentNullException">Thrown when plugins or defaultPlugin is null</exception>
    public NamingPatternPluginFactory(IEnumerable<INamingPatternPlugin> plugins, INamingPatternPlugin defaultPlugin)
    {
        if (plugins == null)
        {
            throw new ArgumentNullException(nameof(plugins));
        }

        _defaultPlugin = defaultPlugin ?? throw new ArgumentNullException(nameof(defaultPlugin));
        _plugins = plugins.OrderByDescending(p => p.Priority).ToList();
    }

    /// <inheritdoc />
    public IReadOnlyList<INamingPatternPlugin> Plugins => _plugins;

    /// <inheritdoc />
    public INamingPatternPlugin DetectPlugin(IReadOnlyList<string> pdfFilePaths)
    {
        if (pdfFilePaths == null)
        {
            throw new ArgumentNullException(nameof(pdfFilePaths));
        }

        foreach (var plugin in _plugins)
        {
            if (plugin.CanHandle(pdfFilePaths))
            {
                return plugin;
            }
        }

        return _defaultPlugin;
    }

    /// <inheritdoc />
    public INamingPatternPlugin? GetPlugin(string pluginName)
    {
        if (string.IsNullOrWhiteSpace(pluginName))
        {
            throw new ArgumentException("Plugin name cannot be null or whitespace", nameof(pluginName));
        }

        return _plugins.FirstOrDefault(p => 
            p.PluginName.Equals(pluginName, StringComparison.OrdinalIgnoreCase));
    }
}
