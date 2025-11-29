using Bookshelf.Application.Spi;

namespace Bookshelf.Infrastructure.Adapters.NamingPatternPlugins;

/// <summary>
/// Default plugin that orders files alphabetically when no specific publisher pattern is detected.
/// </summary>
public sealed class DefaultNamingPatternPlugin : INamingPatternPlugin
{
    /// <inheritdoc />
    public string Name => "Default";

    /// <inheritdoc />
    public bool CanHandle(IReadOnlyList<string> pdfFilePaths)
    {
        // This is the fallback plugin, it can handle any collection
        return true;
    }

    /// <inheritdoc />
    public IReadOnlyList<string> OrderFiles(IReadOnlyList<string> pdfFilePaths)
    {
        // Order alphabetically by file name
        return pdfFilePaths
            .OrderBy(f => Path.GetFileName(f), StringComparer.OrdinalIgnoreCase)
            .ToList();
    }
}
