using Bookshelf.Application.Spi;

namespace Bookshelf.Infrastructure.Adapters;

/// <summary>
/// Simple alphabetic file orderer using natural string comparison
/// </summary>
public class AlphabeticFileOrderer : IFileOrderingStrategy
{
    /// <inheritdoc />
    public IReadOnlyList<string> OrderFiles(IEnumerable<string> files)
    {
        return files
            .OrderBy(f => Path.GetFileName(f), NaturalStringComparer.Instance)
            .ToList();
    }
}
