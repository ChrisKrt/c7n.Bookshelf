namespace Bookshelf.Application.Api.Dtos;

/// <summary>
/// Represents information about a book in the bookshelf
/// </summary>
public sealed record BookInfo(
    string Title,
    string FullPath,
    long FileSizeBytes,
    DateTime CreationDate,
    int? PageCount = null)
{
    /// <summary>
    /// Gets a human-readable file size string
    /// </summary>
    public string FormattedFileSize => FormatFileSize(FileSizeBytes);

    /// <summary>
    /// Formats file size in human-readable format
    /// </summary>
    private static string FormatFileSize(long bytes)
    {
        var isByteRange = bytes < 1024;
        if (isByteRange)
        {
            return $"{bytes} B";
        }

        var isKilobyteRange = bytes < 1024 * 1024;
        if (isKilobyteRange)
        {
            return $"{bytes / 1024.0:F1} KB";
        }

        var isMegabyteRange = bytes < 1024 * 1024 * 1024;
        if (isMegabyteRange)
        {
            return $"{bytes / (1024.0 * 1024.0):F1} MB";
        }

        return $"{bytes / (1024.0 * 1024.0 * 1024.0):F1} GB";
    }
}
