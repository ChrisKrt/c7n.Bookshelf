using Bookshelf.Application.Api;
using Bookshelf.Application.Api.Dtos;
using Bookshelf.Application.Core.Entities;
using Bookshelf.Application.Spi;
using Bookshelf.Application.Spi.Dtos;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Bookshelf.Application.Services;

/// <summary>
/// Service for listing books in a bookshelf
/// </summary>
public sealed class BookshelfListService : IBookshelfListService
{
    private readonly IFileSystemAdapter _fileSystemAdapter;
    private readonly IPdfMerger _pdfMerger;
    private readonly ILogger<BookshelfListService> _logger;

    /// <summary>
    /// Initializes a new instance of the BookshelfListService class
    /// </summary>
    /// <param name="fileSystemAdapter">The file system adapter</param>
    /// <param name="pdfMerger">The PDF merger for extracting page counts</param>
    /// <param name="logger">The logger</param>
    public BookshelfListService(
        IFileSystemAdapter fileSystemAdapter,
        IPdfMerger pdfMerger,
        ILogger<BookshelfListService> logger)
    {
        _fileSystemAdapter = fileSystemAdapter ?? throw new ArgumentNullException(nameof(fileSystemAdapter));
        _pdfMerger = pdfMerger ?? throw new ArgumentNullException(nameof(pdfMerger));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<BookListResult> ListBooksAsync(
        ListBooksRequest request,
        CancellationToken cancellationToken = default)
    {
        // Guard clauses
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        if (string.IsNullOrWhiteSpace(request.BookshelfDirectory))
        {
            throw new ArgumentException("Bookshelf directory cannot be null or whitespace", nameof(request));
        }

        var directoryDoesNotExist = !_fileSystemAdapter.DirectoryExists(
            new DirectoryExistsRequest(request.BookshelfDirectory));
        if (directoryDoesNotExist)
        {
            return BookListResult.CreateFailure($"Bookshelf directory does not exist: {request.BookshelfDirectory}");
        }

        try
        {
            _logger.LogInformation("Listing books from {BookshelfDirectory}", request.BookshelfDirectory);

            // Get all PDF files in the bookshelf directory
            var pdfFiles = await _fileSystemAdapter.GetPdfFilesAsync(
                new GetPdfFilesRequest(request.BookshelfDirectory));

            var hasNoBooks = pdfFiles.Count == 0;
            if (hasNoBooks)
            {
                _logger.LogInformation("Bookshelf is empty: {BookshelfDirectory}", request.BookshelfDirectory);
                return BookListResult.CreateSuccess(Array.Empty<BookInfo>());
            }

            // Build book info list
            var books = new List<BookInfo>();
            foreach (var pdfFile in pdfFiles)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var bookInfo = await CreateBookInfoAsync(pdfFile, request.IncludeDetails);
                books.Add(bookInfo);
            }

            // Apply title filter if specified
            var hasFilter = !string.IsNullOrWhiteSpace(request.TitleFilter);
            if (hasFilter)
            {
                books = ApplyTitleFilter(books, request.TitleFilter!);
            }

            // Apply sorting
            books = ApplySorting(books, request.SortBy, request.SortDirection);

            _logger.LogInformation("Found {BookCount} books in bookshelf", books.Count);

            return BookListResult.CreateSuccess(books);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Book listing was cancelled");
            return BookListResult.CreateFailure("Book listing was cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing books from {BookshelfDirectory}", request.BookshelfDirectory);
            return BookListResult.CreateFailure($"Error listing books: {ex.Message}");
        }
    }

    /// <summary>
    /// Creates a BookInfo from a PDF file path
    /// </summary>
    private async Task<BookInfo> CreateBookInfoAsync(string pdfFile, bool includeDetails)
    {
        // Precondition
        Debug.Assert(!string.IsNullOrWhiteSpace(pdfFile), "PDF file path must not be null");

        var fileInfo = await _fileSystemAdapter.GetFileInfoAsync(new GetFileInfoRequest(pdfFile));
        var title = Path.GetFileNameWithoutExtension(fileInfo.FileName);

        int? pageCount = null;
        if (includeDetails)
        {
            pageCount = await GetPageCountSafelyAsync(pdfFile);
        }

        return new BookInfo(
            title,
            fileInfo.FullPath,
            fileInfo.FileSizeBytes,
            fileInfo.CreationTime,
            pageCount);
    }

    /// <summary>
    /// Gets the page count safely, returning null if unable to read
    /// </summary>
    private async Task<int?> GetPageCountSafelyAsync(string pdfFile)
    {
        try
        {
            return await _pdfMerger.GetPageCountAsync(new GetPdfPageCountRequest(pdfFile));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Unable to read page count for {PdfFile}", pdfFile);
            return null;
        }
    }

    /// <summary>
    /// Applies title filter to the book list
    /// </summary>
    private static List<BookInfo> ApplyTitleFilter(List<BookInfo> books, string titleFilter)
    {
        // Precondition
        Debug.Assert(!string.IsNullOrWhiteSpace(titleFilter), "Title filter must not be null");

        return books
            .Where(b => b.Title.Contains(titleFilter, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    /// <summary>
    /// Applies sorting to the book list
    /// </summary>
    private static List<BookInfo> ApplySorting(
        List<BookInfo> books,
        BookListSortField sortBy,
        SortDirection direction)
    {
        IEnumerable<BookInfo> sorted = sortBy switch
        {
            BookListSortField.Title => direction == SortDirection.Ascending
                ? books.OrderBy(b => b.Title, StringComparer.OrdinalIgnoreCase)
                : books.OrderByDescending(b => b.Title, StringComparer.OrdinalIgnoreCase),
            BookListSortField.FileSize => direction == SortDirection.Ascending
                ? books.OrderBy(b => b.FileSizeBytes)
                : books.OrderByDescending(b => b.FileSizeBytes),
            BookListSortField.CreationDate => direction == SortDirection.Ascending
                ? books.OrderBy(b => b.CreationDate)
                : books.OrderByDescending(b => b.CreationDate),
            BookListSortField.PageCount => SortByPageCount(books, direction),
            _ => books.OrderBy(b => b.Title, StringComparer.OrdinalIgnoreCase)
        };

        return sorted.ToList();
    }

    /// <summary>
    /// Sorts books by page count, placing books with unknown page counts at the end
    /// </summary>
    private static IEnumerable<BookInfo> SortByPageCount(List<BookInfo> books, SortDirection direction)
    {
        // Separate books with and without page counts
        var booksWithPages = books.Where(b => b.PageCount.HasValue);
        var booksWithoutPages = books.Where(b => !b.PageCount.HasValue);

        // Sort books with pages, keeping those without at the end
        var sortedWithPages = direction == SortDirection.Ascending
            ? booksWithPages.OrderBy(b => b.PageCount!.Value)
            : booksWithPages.OrderByDescending(b => b.PageCount!.Value);

        return sortedWithPages.Concat(booksWithoutPages);
    }
}
