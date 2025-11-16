namespace Bookshelf.Application.Spi.Dtos;

/// <summary>
/// Request to extract metadata from a PDF
/// </summary>
public sealed record ExtractMetadataRequest(string PdfPath);
