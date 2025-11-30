using System.Text.RegularExpressions;

namespace Bookshelf.Application.Core.Plugins;

/// <summary>
/// Naming pattern plugin for Wichmann Verlag books with underscore-based numbering
/// Handles patterns like: Vorwort, Inhalt, _1_, _2_, ..., _8_, Anhnge, Stichwortverzeichnis
/// </summary>
public sealed class WichmannNamingPatternPlugin : NamingPatternPluginBase
{
    private static readonly string[] FrontMatterPatterns = ["Vorwort", "Inhalt"];
    private static readonly string[] BackMatterPatterns = ["Stichwortverzeichnis"];
    private static readonly string[] AppendixPatterns = ["Anhnge", "Anhänge"];

    /// <inheritdoc />
    public override string PluginName => "Wichmann Verlag";

    /// <inheritdoc />
    public override int Priority => 5;

    /// <inheritdoc />
    public override bool CanHandle(IReadOnlyList<string> pdfFilePaths)
    {
        if (pdfFilePaths == null)
        {
            throw new ArgumentNullException(nameof(pdfFilePaths));
        }

        var fileNames = pdfFilePaths.Select(GetFileName).ToList();
        
        // Check for characteristic Wichmann patterns: _1_, _2_, etc.
        var hasNumberPattern = fileNames.Any(f => 
            Regex.IsMatch(f, @"_\d+_", RegexOptions.IgnoreCase));
        
        var hasFrontMatter = fileNames.Any(f => 
            FrontMatterPatterns.Any(p => f.Contains(p, StringComparison.OrdinalIgnoreCase)));

        // Check for Anhänge/Anhnge pattern
        var hasAnhange = fileNames.Any(f => 
            AppendixPatterns.Any(p => f.Contains(p, StringComparison.OrdinalIgnoreCase)));

        // Must have number pattern and either front matter or appendix pattern
        // But should NOT have Kapitel_ pattern (which would be mitp)
        var hasKapitelPattern = fileNames.Any(f => 
            Regex.IsMatch(f, @"Kapitel_\d+_", RegexOptions.IgnoreCase));

        return hasNumberPattern && (hasFrontMatter || hasAnhange) && !hasKapitelPattern;
    }

    /// <inheritdoc />
    protected override FileCategory CategorizeFile(string filePath)
    {
        var fileName = GetFileName(filePath);

        if (FrontMatterPatterns.Any(p => fileName.Contains(p, StringComparison.OrdinalIgnoreCase)))
        {
            return FileCategory.FrontMatter;
        }

        if (Regex.IsMatch(fileName, @"_\d+_", RegexOptions.IgnoreCase) &&
            !AppendixPatterns.Any(p => fileName.Contains(p, StringComparison.OrdinalIgnoreCase)))
        {
            return FileCategory.Chapter;
        }

        if (AppendixPatterns.Any(p => fileName.Contains(p, StringComparison.OrdinalIgnoreCase)))
        {
            return FileCategory.Appendix;
        }

        if (BackMatterPatterns.Any(p => fileName.Contains(p, StringComparison.OrdinalIgnoreCase)))
        {
            return FileCategory.BackMatter;
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
            FileCategory.FrontMatter => GetFrontMatterSortKey(fileName),
            FileCategory.Chapter => GetChapterSortKey(fileName),
            FileCategory.Appendix => "0000", // Single appendix section
            FileCategory.BackMatter => GetBackMatterSortKey(fileName),
            _ => fileName
        };
    }

    private static string GetFrontMatterSortKey(string fileName)
    {
        // Order: Vorwort, Inhalt
        for (var i = 0; i < FrontMatterPatterns.Length; i++)
        {
            if (fileName.Contains(FrontMatterPatterns[i], StringComparison.OrdinalIgnoreCase))
            {
                return i.ToString("D4");
            }
        }
        return "9999";
    }

    private static string GetChapterSortKey(string fileName)
    {
        var number = ExtractNumber(fileName, @"_(\d+)_");
        return number.ToString("D4");
    }

    private static string GetBackMatterSortKey(string fileName)
    {
        for (var i = 0; i < BackMatterPatterns.Length; i++)
        {
            if (fileName.Contains(BackMatterPatterns[i], StringComparison.OrdinalIgnoreCase))
            {
                return i.ToString("D4");
            }
        }
        return "9999";
    }
}
