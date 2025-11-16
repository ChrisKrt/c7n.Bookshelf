namespace Bookshelf.Application.Spi.Dtos;

/// <summary>
/// Request to check if a file exists
/// </summary>
public sealed record FileExistsRequest(string FilePath);
