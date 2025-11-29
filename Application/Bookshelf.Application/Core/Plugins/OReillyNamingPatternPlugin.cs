using System.Text.RegularExpressions;

namespace Bookshelf.Application.Core.Plugins;

/// <summary>
/// Naming pattern plugin for O'Reilly publisher books
/// Handles patterns like: BEGINN, Inhalt, Vorwort, Kapitel_1_, Chapter_1_, Index, Anhang
/// </summary>
public sealed class OReillyNamingPatternPlugin : NamingPatternPluginBase
{
    private static readonly string[] FrontMatterPatterns = 
        ["BEGINN", "Inhalt", "Vorwort"];
    
    private static readonly string[] BackMatterPatterns = ["Index"];
    private static readonly string[] AppendixPatterns = ["Anhang"];

    /// <inheritdoc />
    public override string PluginName => "O'Reilly";

    /// <inheritdoc />
    public override int Priority => 8;

    /// <inheritdoc />
    public override bool CanHandle(IReadOnlyList<string> pdfFilePaths)
    {
        var fileNames = pdfFilePaths.Select(GetFileName).ToList();
        
        // O'Reilly supports both Kapitel_ and Chapter_ patterns
        var hasKapitelPattern = fileNames.Any(f => 
            Regex.IsMatch(f, @"Kapitel_\d+_", RegexOptions.IgnoreCase));
        
        var hasChapterPattern = fileNames.Any(f => 
            Regex.IsMatch(f, @"Chapter_\d+_", RegexOptions.IgnoreCase));

        var hasBeginn = fileNames.Any(f => 
            f.Contains("BEGINN", StringComparison.OrdinalIgnoreCase));

        var hasIndex = fileNames.Any(f => 
            f.Contains("Index", StringComparison.OrdinalIgnoreCase));

        // Must have BEGINN pattern and either chapter patterns or Index
        // This distinguishes O'Reilly from mitp (which doesn't have BEGINN)
        return hasBeginn && ((hasKapitelPattern || hasChapterPattern) || hasIndex);
    }

    /// <inheritdoc />
    protected override FileCategory CategorizeFile(string filePath)
    {
        var fileName = GetFileName(filePath);

        if (FrontMatterPatterns.Any(p => fileName.Contains(p, StringComparison.OrdinalIgnoreCase)))
        {
            return FileCategory.FrontMatter;
        }

        if (Regex.IsMatch(fileName, @"(Kapitel|Chapter)_\d+_", RegexOptions.IgnoreCase))
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
            FileCategory.Appendix => GetAppendixSortKey(fileName),
            FileCategory.BackMatter => GetBackMatterSortKey(fileName),
            _ => fileName
        };
    }

    private static string GetFrontMatterSortKey(string fileName)
    {
        // Order: BEGINN, Inhalt, Vorwort
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
        // Try Kapitel pattern first
        var number = ExtractNumber(fileName, @"Kapitel_(\d+)_");
        if (number == int.MaxValue)
        {
            // Try Chapter pattern
            number = ExtractNumber(fileName, @"Chapter_(\d+)_");
        }
        return number.ToString("D4");
    }

    private static string GetAppendixSortKey(string fileName)
    {
        // Extract letter from Anhang_A_, Anhang_B_, etc. or just Anhang
        var match = Regex.Match(fileName, @"Anhang_([A-Z])_", RegexOptions.IgnoreCase);
        if (match.Success)
        {
            return match.Groups[1].Value.ToUpperInvariant();
        }
        
        // If just "Anhang" without letter, it's a single appendix
        return "A";
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
