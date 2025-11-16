namespace Bookshelf.Application.Spi.Dtos;

/// <summary>
/// Request to check if a directory exists
/// </summary>
public sealed record DirectoryExistsRequest(string DirectoryPath);
