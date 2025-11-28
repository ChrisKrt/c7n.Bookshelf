using Bookshelf.Application.Spi;

namespace Bookshelf.Infrastructure.Adapters;

/// <summary>
/// File orderer that sorts by file modification timestamp
/// </summary>
public class TimestampFileOrderer : IFileOrderingStrategy
{
    /// <inheritdoc />
    public IReadOnlyList<string> OrderFiles(IEnumerable<string> files)
    {
        return files
            .OrderBy(GetFileTimestamp)
            .ThenBy(f => Path.GetFileName(f), NaturalStringComparer.Instance)
            .ToList();
    }

    /// <summary>
    /// Gets the file modification timestamp, or DateTime.MinValue if unavailable
    /// </summary>
    private static DateTime GetFileTimestamp(string filePath)
    {
        try
        {
            var fileExists = File.Exists(filePath);
            if (fileExists)
            {
                return File.GetLastWriteTimeUtc(filePath);
            }
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            // Fall back to min value if we can't read the timestamp
        }

        return DateTime.MinValue;
    }
}
