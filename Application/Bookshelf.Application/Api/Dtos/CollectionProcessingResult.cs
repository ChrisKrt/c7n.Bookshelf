namespace Bookshelf.Application.Api.Dtos;

/// <summary>
/// Result of processing a collection
/// </summary>
public sealed record CollectionProcessingResult(
    string OutputPath,
    bool WasMerged,
    bool WasCopied);
