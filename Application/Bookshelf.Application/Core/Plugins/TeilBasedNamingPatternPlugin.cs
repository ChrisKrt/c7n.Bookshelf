using System.Text.RegularExpressions;

namespace Bookshelf.Application.Core.Plugins;

/// <summary>
/// Naming pattern plugin for books with German Teil (Part) structure
/// Handles patterns like: Teil_I_, Teil_II_, Teil_III_, with chapters nested within parts
/// Also handles: BEGINN, Vorwort, Inhaltsverzeichnis, Index, Anhang
/// </summary>
public sealed class TeilBasedNamingPatternPlugin : NamingPatternPluginBase
{
    private static readonly string[] FrontMatterPatterns = 
        ["BEGINN", "Vorwort", "Inhaltsverzeichnis"];
    
    private static readonly string[] BackMatterPatterns = ["Index"];
    private static readonly string[] AppendixPatterns = ["Anhang"];

    /// <inheritdoc />
    public override string PluginName => "Teil-based";

    /// <inheritdoc />
    public override int Priority => 12;

    /// <inheritdoc />
    public override bool CanHandle(IReadOnlyList<string> pdfFilePaths)
    {
        if (pdfFilePaths == null)
        {
            throw new ArgumentNullException(nameof(pdfFilePaths));
        }

        var fileNames = pdfFilePaths.Select(GetFileName).ToList();
        
        // Check for Teil pattern with Roman numerals
        var hasTeilPattern = fileNames.Any(f => 
            Regex.IsMatch(f, @"Teil_[IVX]+_", RegexOptions.IgnoreCase));

        // Must have Teil pattern - this is the distinguishing feature
        return hasTeilPattern;
    }

    /// <inheritdoc />
    protected override FileCategory CategorizeFile(string filePath)
    {
        var fileName = GetFileName(filePath);

        if (FrontMatterPatterns.Any(p => fileName.Contains(p, StringComparison.OrdinalIgnoreCase)))
        {
            return FileCategory.FrontMatter;
        }

        // Teil (Part) patterns
        if (Regex.IsMatch(fileName, @"Teil_[IVX]+_", RegexOptions.IgnoreCase))
        {
            return FileCategory.Part;
        }

        // Chapters within parts
        if (Regex.IsMatch(fileName, @"Kapitel_\d+_", RegexOptions.IgnoreCase))
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
            FileCategory.Part => GetPartSortKey(fileName),
            FileCategory.Chapter => GetChapterSortKey(fileName),
            FileCategory.Appendix => GetAppendixSortKey(fileName),
            FileCategory.BackMatter => GetBackMatterSortKey(fileName),
            _ => fileName
        };
    }

    private static string GetFrontMatterSortKey(string fileName)
    {
        // Order: BEGINN, Vorwort, Inhaltsverzeichnis
        for (var i = 0; i < FrontMatterPatterns.Length; i++)
        {
            if (fileName.Contains(FrontMatterPatterns[i], StringComparison.OrdinalIgnoreCase))
            {
                return i.ToString("D4");
            }
        }
        return "9999";
    }

    private static string GetPartSortKey(string fileName)
    {
        // Extract Roman numeral from Teil_I_, Teil_II_, etc.
        var match = Regex.Match(fileName, @"Teil_([IVX]+)_", RegexOptions.IgnoreCase);
        if (match.Success)
        {
            var romanValue = RomanToInt(match.Groups[1].Value);
            return romanValue.ToString("D4");
        }
        return "5000";
    }

    private static string GetChapterSortKey(string fileName)
    {
        var number = ExtractNumber(fileName, @"Kapitel_(\d+)_");
        return number.ToString("D4");
    }

    private static string GetAppendixSortKey(string fileName)
    {
        // Extract letter from Anhang_A_, Anhang_B_, etc.
        var match = Regex.Match(fileName, @"Anhang_([A-Z])_", RegexOptions.IgnoreCase);
        if (match.Success)
        {
            return match.Groups[1].Value.ToUpperInvariant();
        }
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
