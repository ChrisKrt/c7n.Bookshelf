using Bookshelf.Application.Api;
using Bookshelf.Application.Api.Dtos;
using Bookshelf.Application.Core.Entities;
using Bookshelf.Application.Core.Plugins;
using Bookshelf.Application.Core.ValueObjects;
using Bookshelf.Application.Spi;
using Bookshelf.Application.Spi.Dtos;
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
    private readonly INamingPatternPluginFactory _pluginFactory;

    /// <summary>
    /// Initializes a new instance of the BookshelfConsolidationService class
    /// </summary>
    /// <param name="pdfMerger">The PDF merger</param>
    /// <param name="fileSystemAdapter">The file system adapter</param>
    /// <param name="logger">The logger</param>
    /// <param name="pluginFactory">The naming pattern plugin factory</param>
    public BookshelfConsolidationService(
        IPdfMerger pdfMerger,
        IFileSystemAdapter fileSystemAdapter,
        ILogger<BookshelfConsolidationService> logger,
        INamingPatternPluginFactory pluginFactory)
    {
        _pdfMerger = pdfMerger ?? throw new ArgumentNullException(nameof(pdfMerger));
        _fileSystemAdapter = fileSystemAdapter ?? throw new ArgumentNullException(nameof(fileSystemAdapter));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _pluginFactory = pluginFactory ?? throw new ArgumentNullException(nameof(pluginFactory));
    }

    /// <inheritdoc />
    public async Task<ConsolidationResult> ConsolidateAsync(
        ConsolidationRequest request,
        IProgress<string>? progressCallback = null,
        CancellationToken cancellationToken = default)
    {
        // Guard clauses
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        if (string.IsNullOrWhiteSpace(request.SourceDirectory))
        {
            throw new ArgumentException("Source directory cannot be null or whitespace", nameof(request));
        }

        if (string.IsNullOrWhiteSpace(request.TargetDirectory))
        {
            throw new ArgumentException("Target directory cannot be null or whitespace", nameof(request));
        }

        var sourceDirectoryDoesNotExist = !_fileSystemAdapter.DirectoryExists(
            new DirectoryExistsRequest(request.SourceDirectory));
        if (sourceDirectoryDoesNotExist)
        {
            return ConsolidationResult.CreateFailure($"Source directory does not exist: {request.SourceDirectory}");
        }

        try
        {
            _logger.LogInformation("Starting consolidation from {SourceDirectory} to {TargetDirectory}", 
                request.SourceDirectory, request.TargetDirectory);
            
            progressCallback?.Report("Starting consolidation...");

            // Ensure target directory exists
            _fileSystemAdapter.EnsureDirectoryExists(new EnsureDirectoryExistsRequest(request.TargetDirectory));

            var consolidatedBooks = new List<string>();
            var namingConflicts = new List<string>();
            var individualPdfsCopied = 0;
            var collectionsMerged = 0;

            // Process root PDFs
            var rootPdfFiles = await _fileSystemAdapter.GetPdfFilesAsync(
                new GetPdfFilesRequest(request.SourceDirectory));
            foreach (var pdfFile in rootPdfFiles)
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                var result = await ProcessIndividualPdfAsync(
                    pdfFile, 
                    request.TargetDirectory, 
                    progressCallback, 
                    namingConflicts);
                
                consolidatedBooks.Add(result.Path);
                individualPdfsCopied++;
            }

            // Process subdirectories (collections)
            var subdirectories = await _fileSystemAdapter.GetSubdirectoriesAsync(
                new GetSubdirectoriesRequest(request.SourceDirectory));
            foreach (var subdirectory in subdirectories)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var result = await ProcessCollectionAsync(
                    subdirectory, 
                    request.TargetDirectory, 
                    progressCallback, 
                    namingConflicts, 
                    cancellationToken);

                if (result.WasMerged)
                {
                    consolidatedBooks.Add(result.OutputPath);
                    collectionsMerged++;
                }
                else if (result.WasCopied)
                {
                    consolidatedBooks.Add(result.OutputPath);
                    individualPdfsCopied++;
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
        Debug.Assert(_fileSystemAdapter.DirectoryExists(new DirectoryExistsRequest(directoryPath)), "Directory must exist");
        
        var allPdfs = new List<string>();
        
        // Get PDFs in current directory
        var pdfs = await _fileSystemAdapter.GetPdfFilesAsync(new GetPdfFilesRequest(directoryPath));
        allPdfs.AddRange(pdfs);

        // Get PDFs from subdirectories
        var subdirectories = await _fileSystemAdapter.GetSubdirectoriesAsync(
            new GetSubdirectoriesRequest(directoryPath));
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

    /// <summary>
    /// Processes an individual PDF file and copies it to the target directory
    /// </summary>
    private async Task<FileDestination> ProcessIndividualPdfAsync(
        string pdfFile,
        string targetDirectory,
        IProgress<string>? progressCallback,
        List<string> namingConflicts)
    {
        // Precondition: parameters must be valid
        Debug.Assert(!string.IsNullOrWhiteSpace(pdfFile), "PDF file path must not be null");
        Debug.Assert(!string.IsNullOrWhiteSpace(targetDirectory), "Target directory must not be null");

        var fileName = Path.GetFileName(pdfFile);
        progressCallback?.Report($"Copying individual PDF: {fileName}");
        
        var destinationPath = ResolveDestinationPath(targetDirectory, fileName, namingConflicts);

        await _fileSystemAdapter.CopyFileAsync(
            new CopyFileRequest(pdfFile, destinationPath, false));
        _logger.LogDebug("Copied individual PDF: {FileName}", fileName);
        
        // Postcondition: destination file should exist
        Debug.Assert(_fileSystemAdapter.FileExists(new FileExistsRequest(destinationPath)), 
            "Destination file should exist after copy");
        
        var hadConflict = namingConflicts.Contains(fileName);
        return new FileDestination(destinationPath, hadConflict);
    }

    /// <summary>
    /// Processes a collection directory by either copying a single PDF or merging multiple PDFs
    /// </summary>
    private async Task<CollectionProcessingResult> ProcessCollectionAsync(
        string subdirectory,
        string targetDirectory,
        IProgress<string>? progressCallback,
        List<string> namingConflicts,
        CancellationToken cancellationToken)
    {
        // Precondition: parameters must be valid
        Debug.Assert(!string.IsNullOrWhiteSpace(subdirectory), "Subdirectory must not be null");
        Debug.Assert(_fileSystemAdapter.DirectoryExists(new DirectoryExistsRequest(subdirectory)), 
            "Subdirectory must exist");

        var collectionName = Path.GetFileName(subdirectory);
        progressCallback?.Report($"Processing collection: {collectionName}");

        var collectionPdfs = await GetAllPdfsRecursivelyAsync(subdirectory);

        var hasNoPdfs = collectionPdfs.Count == 0;
        if (hasNoPdfs)
        {
            _logger.LogWarning("No PDFs found in collection: {CollectionName}", collectionName);
            return new CollectionProcessingResult(string.Empty, false, false);
        }

        var isOnlyOnePdf = collectionPdfs.Count == 1;
        if (isOnlyOnePdf)
        {
            return await ProcessSinglePdfCollectionAsync(
                collectionPdfs[0], 
                targetDirectory, 
                namingConflicts);
        }

        return await ProcessMultiPdfCollectionAsync(
            collectionPdfs,
            collectionName,
            targetDirectory,
            progressCallback,
            namingConflicts,
            cancellationToken);
    }

    /// <summary>
    /// Processes a collection containing a single PDF
    /// </summary>
    private async Task<CollectionProcessingResult> ProcessSinglePdfCollectionAsync(
        string singlePdf,
        string targetDirectory,
        List<string> namingConflicts)
    {
        // Precondition
        Debug.Assert(!string.IsNullOrWhiteSpace(singlePdf), "Single PDF path must not be null");

        var fileName = Path.GetFileName(singlePdf);
        var destinationPath = ResolveDestinationPath(targetDirectory, fileName, namingConflicts);

        await _fileSystemAdapter.CopyFileAsync(
            new CopyFileRequest(singlePdf, destinationPath, false));
        
        // Postcondition
        Debug.Assert(_fileSystemAdapter.FileExists(new FileExistsRequest(destinationPath)), 
            "Destination file should exist after copy");
        
        return new CollectionProcessingResult(destinationPath, false, true);
    }

    /// <summary>
    /// Processes a collection containing multiple PDFs by merging them
    /// </summary>
    private async Task<CollectionProcessingResult> ProcessMultiPdfCollectionAsync(
        List<string> collectionPdfs,
        string collectionName,
        string targetDirectory,
        IProgress<string>? progressCallback,
        List<string> namingConflicts,
        CancellationToken cancellationToken)
    {
        // Precondition
        Debug.Assert(collectionPdfs != null && collectionPdfs.Count > 1, "Must have multiple PDFs");

        progressCallback?.Report($"Merging collection: {collectionName}");

        // Detect and apply naming pattern plugin for ordering
        var plugin = _pluginFactory.DetectPlugin(collectionPdfs);
        _logger.LogInformation("Using {PluginName} naming pattern plugin for collection {CollectionName}",
            plugin.PluginName, collectionName);
        progressCallback?.Report($"Detected {plugin.PluginName} naming pattern");

        // Filter and order files according to publisher pattern
        var filteredFiles = plugin.FilterFiles(collectionPdfs);
        var orderedFiles = plugin.OrderFiles(filteredFiles.ToList()).ToList();

        if (orderedFiles.Count == 0)
        {
            _logger.LogWarning("No files remaining after filtering for collection: {CollectionName}", collectionName);
            return new CollectionProcessingResult(string.Empty, false, false);
        }

        var outputFileName = $"{collectionName}.pdf";
        var outputPath = ResolveDestinationPath(targetDirectory, outputFileName, namingConflicts);

        var firstPdfMetadata = await _pdfMerger.ExtractMetadataAsync(
            new ExtractMetadataRequest(orderedFiles[0]));

        var mergeRequest = new MergePdfsRequest(
            orderedFiles,
            outputPath,
            firstPdfMetadata);

        var mergeSuccess = await _pdfMerger.MergePdfsAsync(mergeRequest, cancellationToken);

        if (mergeSuccess)
        {
            _logger.LogInformation("Merged collection {CollectionName} with {Count} PDFs using {PluginName} pattern", 
                collectionName, orderedFiles.Count, plugin.PluginName);
            
            // Postcondition
            Debug.Assert(_fileSystemAdapter.FileExists(new FileExistsRequest(outputPath)), 
                "Output file should exist after merge");
            
            return new CollectionProcessingResult(outputPath, true, false);
        }

        _logger.LogError("Failed to merge collection: {CollectionName}", collectionName);
        return new CollectionProcessingResult(string.Empty, false, false);
    }

    /// <summary>
    /// Resolves the destination path handling naming conflicts
    /// </summary>
    private string ResolveDestinationPath(
        string targetDirectory,
        string fileName,
        List<string> namingConflicts)
    {
        // Precondition
        Debug.Assert(!string.IsNullOrWhiteSpace(targetDirectory), "Target directory must not be null");
        Debug.Assert(!string.IsNullOrWhiteSpace(fileName), "File name must not be null");

        var destinationPath = Path.Combine(targetDirectory, fileName);
        
        var fileExists = _fileSystemAdapter.FileExists(new FileExistsRequest(destinationPath));
        if (fileExists)
        {
            var uniqueFileName = _fileSystemAdapter.GenerateUniqueFileName(
                new GenerateUniqueFileNameRequest(targetDirectory, fileName));
            destinationPath = Path.Combine(targetDirectory, uniqueFileName);
            namingConflicts.Add(fileName);
            _logger.LogWarning("Naming conflict detected for {FileName}, using {UniqueFileName}", 
                fileName, uniqueFileName);
        }

        // Postcondition
        Debug.Assert(!string.IsNullOrWhiteSpace(destinationPath), "Destination path must be valid");

        return destinationPath;
    }
}
