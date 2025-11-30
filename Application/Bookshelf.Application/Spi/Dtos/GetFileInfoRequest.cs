namespace Bookshelf.Application.Spi.Dtos;

/// <summary>
/// Request to get file information
/// </summary>
public sealed record GetFileInfoRequest(string FilePath);
