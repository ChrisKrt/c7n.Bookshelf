using Bookshelf.Application.Api.Dtos;
using Bookshelf.Application.Core.Entities;

namespace Bookshelf.Application.Api;

/// <summary>
/// Service for consolidating scattered PDF files into a single bookshelf
/// </summary>
public interface IBookshelfConsolidationService
{
    /// <summary>
    /// Consolidates PDF files from a source directory into a target bookshelf directory
    /// </summary>
    /// <param name="request">The consolidation request containing source and target directories</param>
    /// <param name="progressCallback">Optional callback for progress updates</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The consolidation result</returns>
    Task<ConsolidationResult> ConsolidateAsync(
        ConsolidationRequest request,
        IProgress<string>? progressCallback = null,
        CancellationToken cancellationToken = default);
}
