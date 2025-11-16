using Bookshelf.Application.Core.ValueObjects;

namespace Bookshelf.Application.Core.Entities;

/// <summary>
/// Represents a book in the system
/// </summary>
public sealed class Book
{
    /// <summary>
    /// Gets the book path
    /// </summary>
    public BookPath Path { get; }

    /// <summary>
    /// Gets the book metadata
    /// </summary>
    public BookMetadata Metadata { get; }

    /// <summary>
    /// Gets whether this book is a collection (contains multiple PDFs)
    /// </summary>
    public bool IsCollection { get; }

    /// <summary>
    /// Initializes a new instance of the Book class
    /// </summary>
    /// <param name="path">The book path</param>
    /// <param name="metadata">The book metadata</param>
    /// <param name="isCollection">Whether this is a collection</param>
    public Book(BookPath path, BookMetadata metadata, bool isCollection = false)
    {
        Path = path ?? throw new ArgumentNullException(nameof(path));
        Metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
        IsCollection = isCollection;
    }

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
