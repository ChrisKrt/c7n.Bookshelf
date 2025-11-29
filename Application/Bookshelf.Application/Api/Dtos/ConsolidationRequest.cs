namespace Bookshelf.Application.Api.Dtos;

/// <summary>
/// Request to consolidate PDF files
/// </summary>
public sealed record ConsolidationRequest(
    string SourceDirectory,
    string TargetDirectory);
