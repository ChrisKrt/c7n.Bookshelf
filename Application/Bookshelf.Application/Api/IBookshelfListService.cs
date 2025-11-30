using Bookshelf.Application.Api.Dtos;
using Bookshelf.Application.Core.Entities;

namespace Bookshelf.Application.Api;

/// <summary>
/// Service for listing books in a bookshelf
/// </summary>
public interface IBookshelfListService
{
    /// <summary>
    /// Lists all books in the specified bookshelf directory
    /// </summary>
    /// <param name="request">The list books request containing directory and options</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The list of books result</returns>
    Task<BookListResult> ListBooksAsync(
        ListBooksRequest request,
        CancellationToken cancellationToken = default);
}
