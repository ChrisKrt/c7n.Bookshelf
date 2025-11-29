namespace Bookshelf.Application.Spi.Dtos;

/// <summary>
/// Request to get PDF files from a directory
/// </summary>
public sealed record GetPdfFilesRequest(string DirectoryPath);
