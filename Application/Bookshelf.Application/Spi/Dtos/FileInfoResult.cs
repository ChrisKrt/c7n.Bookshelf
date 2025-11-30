namespace Bookshelf.Application.Spi.Dtos;

/// <summary>
/// Response containing file information
/// </summary>
public sealed record FileInfoResult(
    string FileName,
    string FullPath,
    long FileSizeBytes,
    DateTime CreationTime);
