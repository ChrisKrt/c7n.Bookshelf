namespace Bookshelf.Application.Spi;

/// <summary>
/// Interface for naming pattern plugins that detect and order PDF files based on publisher-specific naming conventions
/// </summary>
public interface INamingPatternPlugin
{
    /// <summary>
    /// Gets the name of the publisher or pattern this plugin handles
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Determines whether this plugin can handle the given collection of PDF file paths
    /// </summary>
    /// <param name="pdfFilePaths">List of PDF file paths in the collection</param>
    /// <returns>True if this plugin can handle the naming pattern, false otherwise</returns>
    bool CanHandle(IReadOnlyList<string> pdfFilePaths);

    /// <summary>
    /// Orders the PDF file paths according to the publisher's naming convention
    /// </summary>
    /// <param name="pdfFilePaths">List of PDF file paths to order</param>
    /// <returns>Ordered list of PDF file paths in the correct reading order</returns>
    IReadOnlyList<string> OrderFiles(IReadOnlyList<string> pdfFilePaths);
}
