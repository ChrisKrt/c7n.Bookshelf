namespace Bookshelf.Application.Core.ValueObjects;

/// <summary>
/// Represents metadata for a book
/// </summary>
public sealed class BookMetadata
{
    /// <summary>
    /// Gets the book title
    /// </summary>
    public string? Title { get; }

    /// <summary>
    /// Gets the book author
    /// </summary>
    public string? Author { get; }

    /// <summary>
    /// Gets the creation date
    /// </summary>
    public DateTime? CreationDate { get; }

    /// <summary>
    /// Initializes a new instance of the BookMetadata class
    /// </summary>
    /// <param name="title">The book title</param>
    /// <param name="author">The book author</param>
    /// <param name="creationDate">The creation date</param>
    public BookMetadata(string? title = null, string? author = null, DateTime? creationDate = null)
    {
        Title = title;
        Author = author;
        CreationDate = creationDate;
    }

    /// <summary>
    /// Creates an empty metadata instance
    /// </summary>
    public static BookMetadata Empty => new BookMetadata();
}
