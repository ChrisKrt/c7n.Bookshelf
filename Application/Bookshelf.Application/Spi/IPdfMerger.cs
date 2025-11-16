using Bookshelf.Application.Core.ValueObjects;

namespace Bookshelf.Application.Spi;

/// <summary>
/// Interface for merging multiple PDF files into a single PDF
/// </summary>
public interface IPdfMerger
{
    /// <summary>
    /// Merges multiple PDF files into a single PDF file
    /// </summary>
    /// <param name="sourcePdfPaths">The paths of the PDF files to merge</param>
    /// <param name="outputPdfPath">The output path for the merged PDF</param>
    /// <param name="metadata">Metadata to apply to the merged PDF</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if the merge was successful, false otherwise</returns>
    Task<bool> MergePdfsAsync(
        IEnumerable<string> sourcePdfPaths,
        string outputPdfPath,
        BookMetadata? metadata = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Extracts metadata from a PDF file
    /// </summary>
    /// <param name="pdfPath">The path to the PDF file</param>
    /// <returns>The extracted metadata</returns>
    Task<BookMetadata> ExtractMetadataAsync(string pdfPath);
}
