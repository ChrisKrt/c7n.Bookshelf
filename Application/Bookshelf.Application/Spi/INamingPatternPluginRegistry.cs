namespace Bookshelf.Application.Spi;

/// <summary>
/// Registry for managing naming pattern plugins
/// </summary>
public interface INamingPatternPluginRegistry
{
    /// <summary>
    /// Gets all registered naming pattern plugins
    /// </summary>
    IReadOnlyList<INamingPatternPlugin> Plugins { get; }

    /// <summary>
    /// Finds the appropriate plugin for a given collection of PDF files
    /// </summary>
    /// <param name="pdfFilePaths">List of PDF file paths in the collection</param>
    /// <returns>The matching plugin, or null if no plugin matches</returns>
    INamingPatternPlugin? FindPlugin(IReadOnlyList<string> pdfFilePaths);
}
