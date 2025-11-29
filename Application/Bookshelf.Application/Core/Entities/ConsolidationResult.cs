namespace Bookshelf.Application.Core.Entities;

/// <summary>
/// Represents the result of a consolidation operation
/// </summary>
public sealed record ConsolidationResult(
    bool Success,
    int TotalBooksProcessed,
    int IndividualPdfsCopied,
    int CollectionsMerged,
    IReadOnlyList<string> ConsolidatedBooks,
    IReadOnlyList<string> NamingConflicts,
    string? ErrorMessage = null)
{
    /// <summary>
    /// Creates a successful consolidation result
    /// </summary>
    public static ConsolidationResult CreateSuccess(
        int totalBooksProcessed,
        int individualPdfsCopied,
        int collectionsMerged,
        IReadOnlyList<string> consolidatedBooks,
        IReadOnlyList<string> namingConflicts)
    {
        return new ConsolidationResult(
            true,
            totalBooksProcessed,
            individualPdfsCopied,
            collectionsMerged,
            consolidatedBooks,
            namingConflicts);
    }

    /// <summary>
    /// Creates a failed consolidation result
    /// </summary>
    public static ConsolidationResult CreateFailure(string errorMessage)
    {
        return new ConsolidationResult(
            false,
            0,
            0,
            0,
            Array.Empty<string>(),
            Array.Empty<string>(),
            errorMessage);
    }
}
