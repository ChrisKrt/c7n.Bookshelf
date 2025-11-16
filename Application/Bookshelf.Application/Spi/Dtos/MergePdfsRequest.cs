using Bookshelf.Application.Core.ValueObjects;

namespace Bookshelf.Application.Spi.Dtos;

/// <summary>
/// Request to merge PDF files
/// </summary>
public sealed record MergePdfsRequest(
    IEnumerable<string> SourcePdfPaths,
    string OutputPdfPath,
    BookMetadata? Metadata = null);
