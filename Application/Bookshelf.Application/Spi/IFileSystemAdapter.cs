using Bookshelf.Application.Spi.Dtos;

namespace Bookshelf.Application.Spi;

/// <summary>
/// Interface for file system operations
/// </summary>
public interface IFileSystemAdapter
{
    /// <summary>
    /// Gets all PDF files in a directory (non-recursive for root level, recursive for subdirectories)
    /// </summary>
    /// <param name="request">The request containing the directory path</param>
    /// <returns>List of PDF file paths</returns>
    Task<IReadOnlyList<string>> GetPdfFilesAsync(GetPdfFilesRequest request);

    /// <summary>
    /// Gets all subdirectories in a directory
    /// </summary>
    /// <param name="request">The request containing the directory path</param>
    /// <returns>List of subdirectory paths</returns>
    Task<IReadOnlyList<string>> GetSubdirectoriesAsync(GetSubdirectoriesRequest request);

    /// <summary>
    /// Copies a file from source to destination
    /// </summary>
    /// <param name="request">The request containing source path, destination path, and overwrite flag</param>
    /// <returns>True if the copy was successful</returns>
    Task<bool> CopyFileAsync(CopyFileRequest request);

    /// <summary>
    /// Checks if a directory exists
    /// </summary>
    /// <param name="request">The request containing the directory path</param>
    /// <returns>True if the directory exists</returns>
    bool DirectoryExists(DirectoryExistsRequest request);

    /// <summary>
    /// Creates a directory if it doesn't exist
    /// </summary>
    /// <param name="request">The request containing the directory path</param>
    void EnsureDirectoryExists(EnsureDirectoryExistsRequest request);

    /// <summary>
    /// Checks if a file exists
    /// </summary>
    /// <param name="request">The request containing the file path</param>
    /// <returns>True if the file exists</returns>
    bool FileExists(FileExistsRequest request);

    /// <summary>
    /// Generates a unique file name if the file already exists
    /// </summary>
    /// <param name="request">The request containing directory path and desired file name</param>
    /// <returns>A unique file name</returns>
    string GenerateUniqueFileName(GenerateUniqueFileNameRequest request);

    /// <summary>
    /// Gets file information for a specified file
    /// </summary>
    /// <param name="request">The request containing the file path</param>
    /// <returns>File information including size and creation date</returns>
    Task<FileInfoResult> GetFileInfoAsync(GetFileInfoRequest request);
}
