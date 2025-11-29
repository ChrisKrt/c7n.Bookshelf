namespace Bookshelf.Application.Spi.Dtos;

/// <summary>
/// Request to get subdirectories from a directory
/// </summary>
public sealed record GetSubdirectoriesRequest(string DirectoryPath);
