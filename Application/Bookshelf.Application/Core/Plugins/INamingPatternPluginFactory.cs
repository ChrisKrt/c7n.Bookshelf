namespace Bookshelf.Application.Core.Plugins;

/// <summary>
/// Factory interface for creating and selecting appropriate naming pattern plugins
/// </summary>
public interface INamingPatternPluginFactory
{
    /// <summary>
    /// Gets all registered plugins
    /// </summary>
    IReadOnlyList<INamingPatternPlugin> Plugins { get; }

    /// <summary>
    /// Detects the appropriate naming pattern plugin for the given PDF files
    /// </summary>
    /// <param name="pdfFilePaths">The PDF file paths to analyze</param>
    /// <returns>The detected plugin, or the default plugin if none match</returns>
    /// <exception cref="ArgumentNullException">Thrown when pdfFilePaths is null</exception>
    INamingPatternPlugin DetectPlugin(IReadOnlyList<string> pdfFilePaths);

    /// <summary>
    /// Gets a plugin by name
    /// </summary>
    /// <param name="pluginName">The plugin name</param>
    /// <returns>The plugin if found, null otherwise</returns>
    /// <exception cref="ArgumentException">Thrown when pluginName is null or whitespace</exception>
    INamingPatternPlugin? GetPlugin(string pluginName);
}
