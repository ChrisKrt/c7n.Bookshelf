namespace Bookshelf.Application.Core.Entities;

/// <summary>
/// Represents the result of a consolidation operation
/// </summary>
public sealed class ConsolidationResult
{
    /// <summary>
    /// Gets the total number of books processed
    /// </summary>
    public int TotalBooksProcessed { get; }

    /// <summary>
    /// Gets the number of individual PDFs copied
    /// </summary>
    public int IndividualPdfsCopied { get; }

    /// <summary>
    /// Gets the number of collections merged
    /// </summary>
    public int CollectionsMerged { get; }

    /// <summary>
    /// Gets the list of consolidated book paths
    /// </summary>
    public IReadOnlyList<string> ConsolidatedBooks { get; }

    /// <summary>
    /// Gets the list of naming conflicts that were resolved
    /// </summary>
    public IReadOnlyList<string> NamingConflicts { get; }

    /// <summary>
    /// Gets whether the consolidation was successful
    /// </summary>
    public bool Success { get; }

    /// <summary>
    /// Gets any error message if the consolidation failed
    /// </summary>
    public string? ErrorMessage { get; }

    /// <summary>
    /// Initializes a new instance of the ConsolidationResult class
    /// </summary>
    /// <param name="success">Whether the consolidation was successful</param>
    /// <param name="totalBooksProcessed">The total number of books processed</param>
    /// <param name="individualPdfsCopied">The number of individual PDFs copied</param>
    /// <param name="collectionsMerged">The number of collections merged</param>
    /// <param name="consolidatedBooks">The list of consolidated book paths</param>
    /// <param name="namingConflicts">The list of naming conflicts</param>
    /// <param name="errorMessage">Any error message</param>
    public ConsolidationResult(
        bool success,
        int totalBooksProcessed,
        int individualPdfsCopied,
        int collectionsMerged,
        IReadOnlyList<string> consolidatedBooks,
        IReadOnlyList<string> namingConflicts,
        string? errorMessage = null)
    {
        Success = success;
        TotalBooksProcessed = totalBooksProcessed;
        IndividualPdfsCopied = individualPdfsCopied;
        CollectionsMerged = collectionsMerged;
        ConsolidatedBooks = consolidatedBooks ?? Array.Empty<string>();
        NamingConflicts = namingConflicts ?? Array.Empty<string>();
        ErrorMessage = errorMessage;
    }

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
