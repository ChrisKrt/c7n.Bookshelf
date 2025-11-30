using Bookshelf.Application.Api.Dtos;

namespace Bookshelf.Application.Core.Entities;

/// <summary>
/// Represents the result of a list books operation
/// </summary>
public sealed record BookListResult(
    bool Success,
    IReadOnlyList<BookInfo> Books,
    string? ErrorMessage = null)
{
    /// <summary>
    /// Gets whether the bookshelf is empty
    /// </summary>
    public bool IsEmpty => Success && Books.Count == 0;

    /// <summary>
    /// Gets the total number of books
    /// </summary>
    public int TotalBooks => Books.Count;

    /// <summary>
    /// Creates a successful book list result
    /// </summary>
    /// <param name="books">The list of books</param>
    /// <returns>A successful result</returns>
    public static BookListResult CreateSuccess(IReadOnlyList<BookInfo> books)
    {
        return new BookListResult(true, books);
    }

    /// <summary>
    /// Creates a failed book list result
    /// </summary>
    /// <param name="errorMessage">The error message</param>
    /// <returns>A failed result</returns>
    public static BookListResult CreateFailure(string errorMessage)
    {
        return new BookListResult(false, Array.Empty<BookInfo>(), errorMessage);
    }
}
