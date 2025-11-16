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
                
                if (!sourcePathsList.Any())
                {
                    _logger.LogWarning("No source PDFs provided for merging");
                    return false;
                }

                // Create output PDF document
                using var outputDocument = new PdfDocument();
                
                // Set metadata if provided
                if (metadata != null)
                {
                    if (!string.IsNullOrWhiteSpace(metadata.Title))
                    {
                        outputDocument.Info.Title = metadata.Title;
                    }
                    
                    if (!string.IsNullOrWhiteSpace(metadata.Author))
                    {
                        outputDocument.Info.Author = metadata.Author;
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
                        using var sourceDocument = PdfReader.Open(sourcePath, PdfDocumentOpenMode.Import);
                        
                        // Copy all pages from source to output
                        for (int i = 0; i < sourceDocument.PageCount; i++)
                        {
                            outputDocument.AddPage(sourceDocument.Pages[i]);
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

                // Save the merged document
                if (outputDocument.PageCount > 0)
                {
                    outputDocument.Save(outputPdfPath);
                    return true;
                }
                else
                {
                    _logger.LogWarning("No pages to save in merged PDF");
                    return false;
                }
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
                    using var document = PdfReader.Open(pdfPath, PdfDocumentOpenMode.Import);
                    
                    var title = document.Info.Title;
                    var author = document.Info.Author;
                    
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
