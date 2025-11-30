using Bookshelf.Application.Spi;
using Bookshelf.Application.Spi.Dtos;

namespace Bookshelf.Infrastructure.Adapters;

/// <summary>
/// File system adapter implementation
/// </summary>
public class FileSystemAdapter : IFileSystemAdapter
{
    /// <inheritdoc />
    public Task<IReadOnlyList<string>> GetPdfFilesAsync(GetPdfFilesRequest request)
    {
        return Task.Run<IReadOnlyList<string>>(() =>
        {
            try
            {
                var directoryDoesNotExist = !Directory.Exists(request.DirectoryPath);
                if (directoryDoesNotExist)
                {
                    return Array.Empty<string>();
                }

                return Directory.GetFiles(request.DirectoryPath, "*.pdf", SearchOption.TopDirectoryOnly)
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
    public Task<IReadOnlyList<string>> GetSubdirectoriesAsync(GetSubdirectoriesRequest request)
    {
        return Task.Run<IReadOnlyList<string>>(() =>
        {
            try
            {
                var directoryDoesNotExist = !Directory.Exists(request.DirectoryPath);
                if (directoryDoesNotExist)
                {
                    return Array.Empty<string>();
                }

                return Directory.GetDirectories(request.DirectoryPath)
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
    public async Task<bool> CopyFileAsync(CopyFileRequest request)
    {
        try
        {
            await Task.Run(() =>
            {
                File.Copy(request.SourcePath, request.DestinationPath, request.Overwrite);
            });
            return true;
        }
        catch (Exception ex) when (ex is UnauthorizedAccessException or IOException)
        {
            return false;
        }
    }

    /// <inheritdoc />
    public bool DirectoryExists(DirectoryExistsRequest request)
    {
        return Directory.Exists(request.DirectoryPath);
    }

    /// <inheritdoc />
    public void EnsureDirectoryExists(EnsureDirectoryExistsRequest request)
    {
        var directoryDoesNotExist = !Directory.Exists(request.DirectoryPath);
        if (directoryDoesNotExist)
        {
            Directory.CreateDirectory(request.DirectoryPath);
        }
    }

    /// <inheritdoc />
    public bool FileExists(FileExistsRequest request)
    {
        return File.Exists(request.FilePath);
    }

    /// <inheritdoc />
    public string GenerateUniqueFileName(GenerateUniqueFileNameRequest request)
    {
        var nameWithoutExtension = Path.GetFileNameWithoutExtension(request.FileName);
        var extension = Path.GetExtension(request.FileName);
        var counter = 1;

        string candidateFileName;
        string candidatePath;

        do
        {
            candidateFileName = $"{nameWithoutExtension}_{counter}{extension}";
            candidatePath = Path.Combine(request.DirectoryPath, candidateFileName);
            counter++;
        }
        while (File.Exists(candidatePath));

        return candidateFileName;
    }

    /// <inheritdoc />
    public Task<FileInfoResult> GetFileInfoAsync(GetFileInfoRequest request)
    {
        return Task.Run(() =>
        {
            try
            {
                var fileInfo = new FileInfo(request.FilePath);
                return new FileInfoResult(
                    fileInfo.Name,
                    fileInfo.FullName,
                    fileInfo.Length,
                    fileInfo.CreationTime);
            }
            catch (Exception ex) when (ex is UnauthorizedAccessException or IOException or FileNotFoundException)
            {
                // Return default values for inaccessible files
                return new FileInfoResult(
                    Path.GetFileName(request.FilePath),
                    request.FilePath,
                    0,
                    DateTime.MinValue);
            }
        });
    }
}
