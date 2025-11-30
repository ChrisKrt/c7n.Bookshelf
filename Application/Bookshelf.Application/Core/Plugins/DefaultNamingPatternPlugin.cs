namespace Bookshelf.Application.Core.Plugins;

/// <summary>
/// Default naming pattern plugin that uses alphabetical ordering
/// Used when no specific publisher pattern is detected
/// </summary>
public sealed class DefaultNamingPatternPlugin : NamingPatternPluginBase
{
    /// <inheritdoc />
    public override string PluginName => "Default";

    /// <inheritdoc />
    public override int Priority => -1; // Lowest priority

    /// <inheritdoc />
    public override bool CanHandle(IReadOnlyList<string> pdfFilePaths)
    {
        if (pdfFilePaths == null)
        {
            throw new ArgumentNullException(nameof(pdfFilePaths));
        }

        // Always returns true as this is the fallback
        return true;
    }

    /// <inheritdoc />
    protected override FileCategory CategorizeFile(string filePath)
    {
        // All files treated as chapters for simple alphabetical sorting
        return FileCategory.Chapter;
    }

    /// <inheritdoc />
    protected override string GetSortKey(string filePath)
    {
        return GetFileName(filePath);
    }
}
