using Bookshelf.Application.Api;
using Bookshelf.Application.Core.Entities;
using Bookshelf.Application.Core.ValueObjects;
using Bookshelf.Application.Spi;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Bookshelf.Application.Services;

/// <summary>
/// Service for consolidating scattered PDF files into a single bookshelf
/// </summary>
public class BookshelfConsolidationService : IBookshelfConsolidationService
{
    private readonly IPdfMerger _pdfMerger;
    private readonly IFileSystemAdapter _fileSystemAdapter;
    private readonly ILogger<BookshelfConsolidationService> _logger;

    /// <summary>
    /// Initializes a new instance of the BookshelfConsolidationService class
    /// </summary>
    /// <param name="pdfMerger">The PDF merger</param>
    /// <param name="fileSystemAdapter">The file system adapter</param>
    /// <param name="logger">The logger</param>
    public BookshelfConsolidationService(
        IPdfMerger pdfMerger,
        IFileSystemAdapter fileSystemAdapter,
        ILogger<BookshelfConsolidationService> logger)
    {
        _pdfMerger = pdfMerger ?? throw new ArgumentNullException(nameof(pdfMerger));
        _fileSystemAdapter = fileSystemAdapter ?? throw new ArgumentNullException(nameof(fileSystemAdapter));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<ConsolidationResult> ConsolidateAsync(
        string sourceDirectory,
        string targetDirectory,
        IProgress<string>? progressCallback = null,
        CancellationToken cancellationToken = default)
    {
        // Guard clauses
        if (string.IsNullOrWhiteSpace(sourceDirectory))
        {
            throw new ArgumentException("Source directory cannot be null or whitespace", nameof(sourceDirectory));
        }

        if (string.IsNullOrWhiteSpace(targetDirectory))
        {
            throw new ArgumentException("Target directory cannot be null or whitespace", nameof(targetDirectory));
        }

        if (!_fileSystemAdapter.DirectoryExists(sourceDirectory))
        {
            return ConsolidationResult.CreateFailure($"Source directory does not exist: {sourceDirectory}");
        }

        try
        {
            _logger.LogInformation("Starting consolidation from {SourceDirectory} to {TargetDirectory}", 
                sourceDirectory, targetDirectory);
            
            progressCallback?.Report("Starting consolidation...");

            // Ensure target directory exists
            _fileSystemAdapter.EnsureDirectoryExists(targetDirectory);

            var consolidatedBooks = new List<string>();
            var namingConflicts = new List<string>();
            var individualPdfsCopied = 0;
            var collectionsMerged = 0;

            // Get all PDF files in the root of the source directory
            var rootPdfFiles = await _fileSystemAdapter.GetPdfFilesAsync(sourceDirectory);
            
            // Process individual PDFs in root directory
            foreach (var pdfFile in rootPdfFiles)
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                var fileName = Path.GetFileName(pdfFile);
                progressCallback?.Report($"Copying individual PDF: {fileName}");
                
                var destinationPath = Path.Combine(targetDirectory, fileName);
                
                // Handle naming conflicts
                if (_fileSystemAdapter.FileExists(destinationPath))
                {
                    var uniqueFileName = _fileSystemAdapter.GenerateUniqueFileName(targetDirectory, fileName);
                    destinationPath = Path.Combine(targetDirectory, uniqueFileName);
                    namingConflicts.Add(fileName);
                    _logger.LogWarning("Naming conflict detected for {FileName}, using {UniqueFileName}", 
                        fileName, uniqueFileName);
                }

                await _fileSystemAdapter.CopyFileAsync(pdfFile, destinationPath, false);
                consolidatedBooks.Add(destinationPath);
                individualPdfsCopied++;
                
                _logger.LogDebug("Copied individual PDF: {FileName}", fileName);
            }

            // Get all subdirectories
            var subdirectories = await _fileSystemAdapter.GetSubdirectoriesAsync(sourceDirectory);

            // Process each subdirectory (collections)
            foreach (var subdirectory in subdirectories)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var collectionName = Path.GetFileName(subdirectory);
                progressCallback?.Report($"Processing collection: {collectionName}");

                // Get all PDFs in this collection
                var collectionPdfs = await GetAllPdfsRecursivelyAsync(subdirectory);

                if (collectionPdfs.Count == 0)
                {
                    _logger.LogWarning("No PDFs found in collection: {CollectionName}", collectionName);
                    continue;
                }

                if (collectionPdfs.Count == 1)
                {
                    // Single PDF in collection - just copy it
                    var singlePdf = collectionPdfs[0];
                    var fileName = Path.GetFileName(singlePdf);
                    var destinationPath = Path.Combine(targetDirectory, fileName);

                    if (_fileSystemAdapter.FileExists(destinationPath))
                    {
                        var uniqueFileName = _fileSystemAdapter.GenerateUniqueFileName(targetDirectory, fileName);
                        destinationPath = Path.Combine(targetDirectory, uniqueFileName);
                        namingConflicts.Add(fileName);
                    }

                    await _fileSystemAdapter.CopyFileAsync(singlePdf, destinationPath, false);
                    consolidatedBooks.Add(destinationPath);
                    individualPdfsCopied++;
                }
                else
                {
                    // Multiple PDFs - merge them
                    progressCallback?.Report($"Merging collection: {collectionName}");

                    var outputFileName = $"{collectionName}.pdf";
                    var outputPath = Path.Combine(targetDirectory, outputFileName);

                    if (_fileSystemAdapter.FileExists(outputPath))
                    {
                        var uniqueFileName = _fileSystemAdapter.GenerateUniqueFileName(targetDirectory, outputFileName);
                        outputPath = Path.Combine(targetDirectory, uniqueFileName);
                        namingConflicts.Add(outputFileName);
                    }

                    // Extract metadata from the first PDF in the collection
                    var firstPdfMetadata = await _pdfMerger.ExtractMetadataAsync(collectionPdfs[0]);

                    // Merge all PDFs
                    var mergeSuccess = await _pdfMerger.MergePdfsAsync(
                        collectionPdfs, 
                        outputPath, 
                        firstPdfMetadata,
                        cancellationToken);

                    if (mergeSuccess)
                    {
                        consolidatedBooks.Add(outputPath);
                        collectionsMerged++;
                        _logger.LogInformation("Merged collection {CollectionName} with {Count} PDFs", 
                            collectionName, collectionPdfs.Count);
                    }
                    else
                    {
                        _logger.LogError("Failed to merge collection: {CollectionName}", collectionName);
                    }
                }
            }

            var totalBooks = individualPdfsCopied + collectionsMerged;
            progressCallback?.Report($"Consolidation complete! Total books: {totalBooks}");
            
            _logger.LogInformation(
                "Consolidation completed. Total: {Total}, Individual: {Individual}, Merged: {Merged}, Conflicts: {Conflicts}",
                totalBooks, individualPdfsCopied, collectionsMerged, namingConflicts.Count);

            return ConsolidationResult.CreateSuccess(
                totalBooks,
                individualPdfsCopied,
                collectionsMerged,
                consolidatedBooks,
                namingConflicts);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Consolidation was cancelled");
            return ConsolidationResult.CreateFailure("Consolidation was cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during consolidation");
            return ConsolidationResult.CreateFailure($"Error during consolidation: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets all PDF files recursively from a directory
    /// </summary>
    private async Task<List<string>> GetAllPdfsRecursivelyAsync(string directoryPath)
    {
        // Precondition: directory path must be valid
        Debug.Assert(!string.IsNullOrWhiteSpace(directoryPath), "Directory path must not be null or whitespace");
        Debug.Assert(_fileSystemAdapter.DirectoryExists(directoryPath), "Directory must exist");
        
        var allPdfs = new List<string>();
        
        // Get PDFs in current directory
        var pdfs = await _fileSystemAdapter.GetPdfFilesAsync(directoryPath);
        allPdfs.AddRange(pdfs);

        // Get PDFs from subdirectories
        var subdirectories = await _fileSystemAdapter.GetSubdirectoriesAsync(directoryPath);
        foreach (var subdirectory in subdirectories)
        {
            var subPdfs = await GetAllPdfsRecursivelyAsync(subdirectory);
            allPdfs.AddRange(subPdfs);
        }

        // Sort to ensure consistent ordering
        allPdfs.Sort(StringComparer.OrdinalIgnoreCase);
        
        // Postcondition: result should not be null and all paths should be valid
        Debug.Assert(allPdfs != null, "Result list must not be null");
        Debug.Assert(allPdfs.All(p => !string.IsNullOrWhiteSpace(p)), "All paths must be valid");
        
        return allPdfs;
    }
}
