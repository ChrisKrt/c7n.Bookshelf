namespace Bookshelf.Application.Spi.Dtos;

/// <summary>
/// Request to copy a file
/// </summary>
public sealed record CopyFileRequest(
    string SourcePath,
    string DestinationPath,
    bool Overwrite = false);
