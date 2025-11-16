using Bookshelf.Application.Core.ValueObjects;
using Bookshelf.Application.Spi;
using iText.Kernel.Pdf;
using Microsoft.Extensions.Logging;

namespace Bookshelf.Infrastructure.Adapters;

/// <summary>
/// PDF merger implementation using iText7
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
                
                if (!sourcePathsList.Any())
                {
                    _logger.LogWarning("No source PDFs provided for merging");
                    return false;
                }

                // Create output PDF
                using var outputWriter = new PdfWriter(outputPdfPath);
                using var outputPdf = new PdfDocument(outputWriter);
                
                // Set metadata if provided
                if (metadata != null)
                {
                    var pdfInfo = outputPdf.GetDocumentInfo();
                    
                    if (!string.IsNullOrWhiteSpace(metadata.Title))
                    {
                        pdfInfo.SetTitle(metadata.Title);
                    }
                    
                    if (!string.IsNullOrWhiteSpace(metadata.Author))
                    {
                        pdfInfo.SetAuthor(metadata.Author);
                    }
                }

                // Merge all source PDFs
                foreach (var sourcePath in sourcePathsList)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (!File.Exists(sourcePath))
                    {
                        _logger.LogWarning("Source PDF not found: {SourcePath}", sourcePath);
                        continue;
                    }

                    try
                    {
                        using var sourceReader = new PdfReader(sourcePath);
                        using var sourcePdf = new PdfDocument(sourceReader);
                        
                        // Copy all pages from source to output
                        sourcePdf.CopyPagesTo(1, sourcePdf.GetNumberOfPages(), outputPdf);
                        
                        _logger.LogDebug("Merged PDF: {SourcePath}", sourcePath);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error merging PDF: {SourcePath}", sourcePath);
                        // Continue with other PDFs
                    }
                }

                return true;
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
                if (!File.Exists(pdfPath))
                {
                    _logger.LogWarning("PDF file not found: {PdfPath}", pdfPath);
                    return BookMetadata.Empty;
                }

                try
                {
                    using var reader = new PdfReader(pdfPath);
                    using var pdfDocument = new PdfDocument(reader);
                    var info = pdfDocument.GetDocumentInfo();

                    var title = info.GetTitle();
                    var author = info.GetAuthor();
                    
                    // Try to get creation date from file info if not in PDF metadata
                    DateTime? creationDate = null;
                    try
                    {
                        var fileInfo = new FileInfo(pdfPath);
                        creationDate = fileInfo.CreationTime;
                    }
                    catch
                    {
                        // Ignore errors getting file creation date
                    }

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
}
