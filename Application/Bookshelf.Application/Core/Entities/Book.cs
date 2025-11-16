using Bookshelf.Application.Core.ValueObjects;

namespace Bookshelf.Application.Core.Entities;

/// <summary>
/// Represents a book in the system
/// </summary>
public sealed record Book(BookPath Path, BookMetadata Metadata, bool IsCollection = false)
{
    /// <summary>
    /// Creates a book from a file path
    /// </summary>
    /// <param name="filePath">The file path</param>
    /// <param name="metadata">Optional metadata</param>
    /// <returns>A new book instance</returns>
    public static Book FromFile(string filePath, BookMetadata? metadata = null)
    {
        var bookPath = new BookPath(filePath);
        return new Book(bookPath, metadata ?? BookMetadata.Empty, false);
    }

    /// <summary>
    /// Creates a book collection from a directory path
    /// </summary>
    /// <param name="directoryPath">The directory path</param>
    /// <param name="metadata">Optional metadata</param>
    /// <returns>A new book instance representing a collection</returns>
    public static Book FromCollection(string directoryPath, BookMetadata? metadata = null)
    {
        var bookPath = new BookPath(directoryPath);
        return new Book(bookPath, metadata ?? BookMetadata.Empty, true);
    }
}
