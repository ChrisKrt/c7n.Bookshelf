namespace Bookshelf.Application.Core.ValueObjects;

/// <summary>
/// Represents metadata for a book
/// </summary>
public sealed record BookMetadata(string? Title = null, string? Author = null, DateTime? CreationDate = null)
{
    /// <summary>
    /// Creates an empty metadata instance
    /// </summary>
    public static BookMetadata Empty => new BookMetadata();
}
