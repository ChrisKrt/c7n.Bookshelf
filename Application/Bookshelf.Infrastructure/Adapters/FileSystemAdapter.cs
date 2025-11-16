using Bookshelf.Application.Spi;

namespace Bookshelf.Infrastructure.Adapters;

/// <summary>
/// File system adapter implementation
/// </summary>
public class FileSystemAdapter : IFileSystemAdapter
{
    /// <inheritdoc />
    public Task<IReadOnlyList<string>> GetPdfFilesAsync(string directoryPath)
    {
        return Task.Run<IReadOnlyList<string>>(() =>
        {
            try
            {
                if (!Directory.Exists(directoryPath))
                {
                    return Array.Empty<string>();
                }

                return Directory.GetFiles(directoryPath, "*.pdf", SearchOption.TopDirectoryOnly)
                    .OrderBy(f => f, StringComparer.OrdinalIgnoreCase)
                    .ToList();
            }
            catch (Exception ex) when (ex is UnauthorizedAccessException or IOException)
            {
                // Log the error and return empty list
                return Array.Empty<string>();
            }
        });
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<string>> GetSubdirectoriesAsync(string directoryPath)
    {
        return Task.Run<IReadOnlyList<string>>(() =>
        {
            try
            {
                if (!Directory.Exists(directoryPath))
                {
                    return Array.Empty<string>();
                }

                return Directory.GetDirectories(directoryPath)
                    .OrderBy(d => d, StringComparer.OrdinalIgnoreCase)
                    .ToList();
            }
            catch (Exception ex) when (ex is UnauthorizedAccessException or IOException)
            {
                return Array.Empty<string>();
            }
        });
    }

    /// <inheritdoc />
    public async Task<bool> CopyFileAsync(string sourcePath, string destinationPath, bool overwrite = false)
    {
        try
        {
            await Task.Run(() =>
            {
                File.Copy(sourcePath, destinationPath, overwrite);
            });
            return true;
        }
        catch (Exception ex) when (ex is UnauthorizedAccessException or IOException)
        {
            return false;
        }
    }

    /// <inheritdoc />
    public bool DirectoryExists(string directoryPath)
    {
        return Directory.Exists(directoryPath);
    }

    /// <inheritdoc />
    public void EnsureDirectoryExists(string directoryPath)
    {
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
    }

    /// <inheritdoc />
    public bool FileExists(string filePath)
    {
        return File.Exists(filePath);
    }

    /// <inheritdoc />
    public string GenerateUniqueFileName(string directoryPath, string fileName)
    {
        var nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
        var extension = Path.GetExtension(fileName);
        var counter = 1;

        string candidateFileName;
        string candidatePath;

        do
        {
            candidateFileName = $"{nameWithoutExtension}_{counter}{extension}";
            candidatePath = Path.Combine(directoryPath, candidateFileName);
            counter++;
        }
        while (File.Exists(candidatePath));

        return candidateFileName;
    }
}
