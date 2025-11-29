namespace Bookshelf.Application.Core.ValueObjects;

/// <summary>
/// Represents a book file path with validation
/// </summary>
public sealed record BookPath
{
    /// <summary>
    /// Gets the full file path
    /// </summary>
    public string FullPath { get; }

    /// <summary>
    /// Gets the file name without extension
    /// </summary>
    public string NameWithoutExtension { get; }

    /// <summary>
    /// Gets the file extension
    /// </summary>
    public string Extension { get; }

    /// <summary>
    /// Initializes a new instance of the BookPath record
    /// </summary>
    /// <param name="fullPath">The full file path</param>
    /// <exception cref="ArgumentException">Thrown when the path is invalid</exception>
    public BookPath(string fullPath)
    {
        if (string.IsNullOrWhiteSpace(fullPath))
        {
            throw new ArgumentException("Path cannot be null or whitespace", nameof(fullPath));
        }

        FullPath = Path.GetFullPath(fullPath);
        NameWithoutExtension = Path.GetFileNameWithoutExtension(fullPath);
        Extension = Path.GetExtension(fullPath);
    }

    /// <summary>
    /// Gets the file name with extension
    /// </summary>
    public string FileName => Path.GetFileName(FullPath);

    /// <summary>
    /// Gets the directory path
    /// </summary>
    public string DirectoryPath => Path.GetDirectoryName(FullPath) ?? string.Empty;

    public override string ToString() => FullPath;

    public bool Equals(BookPath? other)
    {
        return other is not null && 
               string.Equals(FullPath, other.FullPath, StringComparison.OrdinalIgnoreCase);
    }

    public override int GetHashCode()
    {
        return StringComparer.OrdinalIgnoreCase.GetHashCode(FullPath);
    }
}
