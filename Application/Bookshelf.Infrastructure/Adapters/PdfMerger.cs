using Bookshelf.Application.Core.ValueObjects;
using Bookshelf.Application.Spi;
using Microsoft.Extensions.Logging;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace Bookshelf.Infrastructure.Adapters;

/// <summary>
/// PDF merger implementation using PdfSharp
/// </summary>
public class PdfMerger : IPdfMerger
{
    private readonly ILogger<PdfMerger> _logger;

    /// <summary>
    /// Initializes a new instance of the PdfMerger class
    /// </summary>
    /// <param name="logger">The logger</param>
    public PdfMerger(ILogger<PdfMerger> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<bool> MergePdfsAsync(
        IEnumerable<string> sourcePdfPaths,
        string outputPdfPath,
        BookMetadata? metadata = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await Task.Run(() =>
            {
                var sourcePathsList = sourcePdfPaths.ToList();
                
                var hasNoSourcePdfs = !sourcePathsList.Any();
                if (hasNoSourcePdfs)
                {
                    _logger.LogWarning("No source PDFs provided for merging");
                    return false;
                }

                // Create output PDF document
                using var outputDocument = new PdfDocument();
                
                SetMetadataIfProvided(outputDocument, metadata);
                MergeAllSourcePdfs(sourcePathsList, outputDocument, cancellationToken);

                return SaveMergedDocument(outputDocument, outputPdfPath);
            }, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("PDF merge operation was cancelled");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during PDF merge to {OutputPath}", outputPdfPath);
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<BookMetadata> ExtractMetadataAsync(string pdfPath)
    {
        try
        {
            return await Task.Run(() =>
            {
                var fileDoesNotExist = !File.Exists(pdfPath);
                if (fileDoesNotExist)
                {
                    _logger.LogWarning("PDF file not found: {PdfPath}", pdfPath);
                    return BookMetadata.Empty;
                }

                try
                {
                    using var document = PdfReader.Open(pdfPath, PdfDocumentOpenMode.Import);
                    
                    var title = document.Info.Title;
                    var author = document.Info.Author;
                    var creationDate = TryGetFileCreationDate(pdfPath);

                    return new BookMetadata(title, author, creationDate);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error reading PDF metadata from {PdfPath}", pdfPath);
                    return BookMetadata.Empty;
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting metadata from {PdfPath}", pdfPath);
            return BookMetadata.Empty;
        }
    }

    /// <summary>
    /// Sets metadata on the PDF document if provided
    /// </summary>
    private static void SetMetadataIfProvided(PdfDocument outputDocument, BookMetadata? metadata)
    {
        var hasMetadata = metadata != null;
        if (!hasMetadata)
        {
            return;
        }

        var hasTitle = !string.IsNullOrWhiteSpace(metadata!.Title);
        if (hasTitle)
        {
            outputDocument.Info.Title = metadata.Title!;
        }
        
        var hasAuthor = !string.IsNullOrWhiteSpace(metadata.Author);
        if (hasAuthor)
        {
            outputDocument.Info.Author = metadata.Author!;
        }
    }

    /// <summary>
    /// Merges all source PDFs into the output document
    /// </summary>
    private void MergeAllSourcePdfs(
        List<string> sourcePathsList, 
        PdfDocument outputDocument, 
        CancellationToken cancellationToken)
    {
        foreach (var sourcePath in sourcePathsList)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var fileDoesNotExist = !File.Exists(sourcePath);
            if (fileDoesNotExist)
            {
                _logger.LogWarning("Source PDF not found: {SourcePath}", sourcePath);
                continue;
            }

            TryMergeSinglePdf(sourcePath, outputDocument);
        }
    }

    /// <summary>
    /// Attempts to merge a single PDF into the output document
    /// </summary>
    private void TryMergeSinglePdf(string sourcePath, PdfDocument outputDocument)
    {
        try
        {
            using var sourceDocument = PdfReader.Open(sourcePath, PdfDocumentOpenMode.Import);
            
            // Copy all pages from source to output using LINQ
            var pageIndices = Enumerable.Range(0, sourceDocument.PageCount);
            foreach (var pageIndex in pageIndices)
            {
                outputDocument.AddPage(sourceDocument.Pages[pageIndex]);
            }
            
            _logger.LogDebug("Merged PDF: {SourcePath} ({PageCount} pages)", 
                sourcePath, sourceDocument.PageCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error merging PDF: {SourcePath}", sourcePath);
            // Continue with other PDFs
        }
    }

    /// <summary>
    /// Saves the merged document if it has pages
    /// </summary>
    private bool SaveMergedDocument(PdfDocument outputDocument, string outputPdfPath)
    {
        var hasPages = outputDocument.PageCount > 0;
        if (hasPages)
        {
            outputDocument.Save(outputPdfPath);
            return true;
        }

        _logger.LogWarning("No pages to save in merged PDF");
        return false;
    }

    /// <summary>
    /// Tries to get the file creation date, returns null on failure
    /// </summary>
    private static DateTime? TryGetFileCreationDate(string pdfPath)
    {
        try
        {
            var fileInfo = new FileInfo(pdfPath);
            return fileInfo.CreationTime;
        }
        catch
        {
            // Ignore errors getting file creation date
            return null;
        }
    }
}
