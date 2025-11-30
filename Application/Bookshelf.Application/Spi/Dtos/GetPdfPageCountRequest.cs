namespace Bookshelf.Application.Spi.Dtos;

/// <summary>
/// Request to get PDF page count
/// </summary>
public sealed record GetPdfPageCountRequest(string FilePath);
