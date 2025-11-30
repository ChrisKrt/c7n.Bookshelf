namespace Bookshelf.Application.Api.Dtos;

/// <summary>
/// Specifies the field by which to sort the book list
/// </summary>
public enum BookListSortField
{
    /// <summary>
    /// Sort by book title alphabetically
    /// </summary>
    Title,

    /// <summary>
    /// Sort by file size
    /// </summary>
    FileSize,

    /// <summary>
    /// Sort by creation date
    /// </summary>
    CreationDate,

    /// <summary>
    /// Sort by number of pages
    /// </summary>
    PageCount
}

/// <summary>
/// Specifies the sort direction
/// </summary>
public enum SortDirection
{
    /// <summary>
    /// Sort in ascending order
    /// </summary>
    Ascending,

    /// <summary>
    /// Sort in descending order
    /// </summary>
    Descending
}

/// <summary>
/// Request to list books in a bookshelf directory
/// </summary>
public sealed record ListBooksRequest(
    string BookshelfDirectory,
    string? TitleFilter = null,
    bool IncludeDetails = false,
    BookListSortField SortBy = BookListSortField.Title,
    SortDirection SortDirection = SortDirection.Ascending);
