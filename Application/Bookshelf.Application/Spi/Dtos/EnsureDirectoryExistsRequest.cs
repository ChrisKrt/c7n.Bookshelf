namespace Bookshelf.Application.Spi.Dtos;

/// <summary>
/// Request to ensure a directory exists
/// </summary>
public sealed record EnsureDirectoryExistsRequest(string DirectoryPath);
