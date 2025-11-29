namespace Bookshelf.Application.Api.Dtos;

/// <summary>
/// Represents a destination path for a file copy operation
/// </summary>
public sealed record FileDestination(string Path, bool HadConflict);
