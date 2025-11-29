using System.Text.RegularExpressions;

namespace Bookshelf.Application.Core.Plugins;

/// <summary>
/// Naming pattern plugin for Hanser Verlag books with ISBN-based naming
/// Handles patterns like: 9783446######.fm.pdf, 9783446######.001.pdf, 9783446######.bm.pdf
/// </summary>
public sealed class HanserNamingPatternPlugin : NamingPatternPluginBase
{
    // Pattern for ISBN-based file naming (9783446 followed by digits)
    private const string IsbnPattern = @"97834\d{8}";

    /// <inheritdoc />
    public override string PluginName => "Hanser Verlag";

    /// <inheritdoc />
    public override int Priority => 15;

    /// <inheritdoc />
    public override bool CanHandle(IReadOnlyList<string> pdfFilePaths)
    {
        var fileNames = pdfFilePaths.Select(GetFileName).ToList();
        
        // Check for ISBN-based naming pattern with .fm. or .bm. or numeric extensions
        var hasIsbnPattern = fileNames.Any(f => 
            Regex.IsMatch(f, $@"{IsbnPattern}\.\d{{3}}\.pdf", RegexOptions.IgnoreCase));
        
        var hasFrontMatter = fileNames.Any(f => 
            Regex.IsMatch(f, $@"{IsbnPattern}\.fm\.pdf", RegexOptions.IgnoreCase));

        var hasBackMatter = fileNames.Any(f => 
            Regex.IsMatch(f, $@"{IsbnPattern}\.bm\.pdf", RegexOptions.IgnoreCase));

        // Must have numeric chapter pattern and at least front or back matter
        return hasIsbnPattern && (hasFrontMatter || hasBackMatter);
    }

    /// <inheritdoc />
    protected override FileCategory CategorizeFile(string filePath)
    {
        var fileName = GetFileName(filePath);

        // Front matter (.fm.pdf)
        if (Regex.IsMatch(fileName, $@"{IsbnPattern}\.fm\.pdf", RegexOptions.IgnoreCase))
        {
            return FileCategory.FrontMatter;
        }

        // Back matter (.bm.pdf)
        if (Regex.IsMatch(fileName, $@"{IsbnPattern}\.bm\.pdf", RegexOptions.IgnoreCase))
        {
            return FileCategory.BackMatter;
        }

        // Chapters (numeric extensions like .001.pdf, .002.pdf)
        if (Regex.IsMatch(fileName, $@"{IsbnPattern}\.\d{{3}}\.pdf", RegexOptions.IgnoreCase))
        {
            return FileCategory.Chapter;
        }

        return FileCategory.Unknown;
    }

    /// <inheritdoc />
    protected override string GetSortKey(string filePath)
    {
        var fileName = GetFileName(filePath);
        var category = CategorizeFile(filePath);

        return category switch
        {
            FileCategory.FrontMatter => "0000",
            FileCategory.Chapter => GetChapterSortKey(fileName),
            FileCategory.BackMatter => "9999",
            _ => fileName
        };
    }

    private static string GetChapterSortKey(string fileName)
    {
        // Extract the three-digit chapter number
        var match = Regex.Match(fileName, $@"{IsbnPattern}\.(\d{{3}})\.pdf", RegexOptions.IgnoreCase);
        if (match.Success)
        {
            if (int.TryParse(match.Groups[1].Value, out var number))
            {
                return number.ToString("D4");
            }
        }
        return "5000";
    }
}
