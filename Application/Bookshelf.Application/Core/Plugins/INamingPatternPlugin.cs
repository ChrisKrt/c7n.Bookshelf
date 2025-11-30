namespace Bookshelf.Application.Core.Plugins;

/// <summary>
/// Interface for naming pattern plugins that determine PDF ordering for merging
/// </summary>
public interface INamingPatternPlugin
{
    /// <summary>
    /// Gets the name of the publisher or pattern this plugin handles
    /// </summary>
    string PluginName { get; }

    /// <summary>
    /// Gets the priority of this plugin (higher priority plugins are checked first)
    /// </summary>
    int Priority { get; }

    /// <summary>
    /// Determines if this plugin can handle the given collection of PDF files
    /// </summary>
    /// <param name="pdfFilePaths">The PDF file paths to check</param>
    /// <returns>True if this plugin can order the files, false otherwise</returns>
    bool CanHandle(IReadOnlyList<string> pdfFilePaths);

    /// <summary>
    /// Orders the PDF files according to the publisher's naming pattern
    /// </summary>
    /// <param name="pdfFilePaths">The PDF file paths to order</param>
    /// <returns>The ordered list of PDF file paths</returns>
    IReadOnlyList<string> OrderFiles(IReadOnlyList<string> pdfFilePaths);

    /// <summary>
    /// Filters out duplicate or invalid files
    /// </summary>
    /// <param name="pdfFilePaths">The PDF file paths to filter</param>
    /// <returns>The filtered list of PDF file paths</returns>
    IReadOnlyList<string> FilterFiles(IReadOnlyList<string> pdfFilePaths);
}
