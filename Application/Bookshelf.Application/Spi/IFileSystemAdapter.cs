namespace Bookshelf.Application.Spi;

/// <summary>
/// Interface for file system operations
/// </summary>
public interface IFileSystemAdapter
{
    /// <summary>
    /// Gets all PDF files in a directory (non-recursive for root level, recursive for subdirectories)
    /// </summary>
    /// <param name="directoryPath">The directory path</param>
    /// <returns>List of PDF file paths</returns>
    Task<IReadOnlyList<string>> GetPdfFilesAsync(string directoryPath);

    /// <summary>
    /// Gets all subdirectories in a directory
    /// </summary>
    /// <param name="directoryPath">The directory path</param>
    /// <returns>List of subdirectory paths</returns>
    Task<IReadOnlyList<string>> GetSubdirectoriesAsync(string directoryPath);

    /// <summary>
    /// Copies a file from source to destination
    /// </summary>
    /// <param name="sourcePath">The source file path</param>
    /// <param name="destinationPath">The destination file path</param>
    /// <param name="overwrite">Whether to overwrite if the file exists</param>
    /// <returns>True if the copy was successful</returns>
    Task<bool> CopyFileAsync(string sourcePath, string destinationPath, bool overwrite = false);

    /// <summary>
    /// Checks if a directory exists
    /// </summary>
    /// <param name="directoryPath">The directory path</param>
    /// <returns>True if the directory exists</returns>
    bool DirectoryExists(string directoryPath);

    /// <summary>
    /// Creates a directory if it doesn't exist
    /// </summary>
    /// <param name="directoryPath">The directory path</param>
    void EnsureDirectoryExists(string directoryPath);

    /// <summary>
    /// Checks if a file exists
    /// </summary>
    /// <param name="filePath">The file path</param>
    /// <returns>True if the file exists</returns>
    bool FileExists(string filePath);

    /// <summary>
    /// Generates a unique file name if the file already exists
    /// </summary>
    /// <param name="directoryPath">The directory path</param>
    /// <param name="fileName">The desired file name</param>
    /// <returns>A unique file name</returns>
    string GenerateUniqueFileName(string directoryPath, string fileName);
}
