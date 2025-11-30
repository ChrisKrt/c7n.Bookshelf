using Bookshelf.Application.Core.ValueObjects;
using Bookshelf.Application.Spi.Dtos;

namespace Bookshelf.Application.Spi;

/// <summary>
/// Interface for merging multiple PDF files into a single PDF
/// </summary>
public interface IPdfMerger
{
    /// <summary>
    /// Merges multiple PDF files into a single PDF file
    /// </summary>
    /// <param name="request">The request containing source PDF paths, output path, and metadata</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if the merge was successful, false otherwise</returns>
    Task<bool> MergePdfsAsync(
        MergePdfsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Extracts metadata from a PDF file
    /// </summary>
    /// <param name="request">The request containing the PDF path</param>
    /// <returns>The extracted metadata</returns>
    Task<BookMetadata> ExtractMetadataAsync(ExtractMetadataRequest request);

    /// <summary>
    /// Gets the page count of a PDF file
    /// </summary>
    /// <param name="request">The request containing the PDF path</param>
    /// <returns>The number of pages in the PDF, or null if unable to read</returns>
    Task<int?> GetPageCountAsync(GetPdfPageCountRequest request);
}
