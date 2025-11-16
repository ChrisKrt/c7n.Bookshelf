namespace Bookshelf.Application.Spi.Dtos;

/// <summary>
/// Request to generate a unique file name
/// </summary>
public sealed record GenerateUniqueFileNameRequest(
    string DirectoryPath,
    string FileName);
