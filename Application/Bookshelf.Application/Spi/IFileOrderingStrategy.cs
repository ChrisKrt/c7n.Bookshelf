namespace Bookshelf.Application.Spi;

/// <summary>
/// Defines the ordering strategy for files
/// </summary>
public enum FileOrderingStrategyType
{
    /// <summary>
    /// Semantic ordering that understands document structure (cover, chapters, appendices)
    /// </summary>
    Semantic,

    /// <summary>
    /// Simple alphabetic ordering
    /// </summary>
    Alphabetic,

    /// <summary>
    /// Order by file modification timestamp
    /// </summary>
    Timestamp
}

/// <summary>
/// Interface for file ordering strategies
/// </summary>
public interface IFileOrderingStrategy
{
    /// <summary>
    /// Orders the given files according to the strategy's logic
    /// </summary>
    /// <param name="files">The files to order</param>
    /// <returns>The ordered files</returns>
    IReadOnlyList<string> OrderFiles(IEnumerable<string> files);
}
