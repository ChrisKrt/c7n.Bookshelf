using System.Text.RegularExpressions;

namespace Bookshelf.Application.Core.Plugins;

/// <summary>
/// Naming pattern plugin for mitp publisher books with German naming conventions
/// Handles patterns like: Cover, Titel, Inhaltsverzeichnis, Einleitung, über den Autor, Kapitel_1_, Anhang_A_, Glossar, Stichwortverzeichnis
/// </summary>
public sealed class MitpNamingPatternPlugin : NamingPatternPluginBase
{
    private static readonly string[] FrontMatterPatterns = 
        ["Cover", "Titel", "Inhaltsverzeichnis", "Einleitung", "über den Autor"];
    
    private static readonly string[] BackMatterPatterns = 
        ["Glossar", "Stichwortverzeichnis"];

    /// <inheritdoc />
    public override string PluginName => "mitp";

    /// <inheritdoc />
    public override int Priority => 10;

    /// <inheritdoc />
    public override bool CanHandle(IReadOnlyList<string> pdfFilePaths)
    {
        if (pdfFilePaths == null)
        {
            throw new ArgumentNullException(nameof(pdfFilePaths));
        }

        var fileNames = pdfFilePaths.Select(GetFileName).ToList();
        
        // Check for characteristic mitp patterns
        var hasKapitelPattern = fileNames.Any(f => 
            Regex.IsMatch(f, @"Kapitel_\d+_", RegexOptions.IgnoreCase));
        
        var hasFrontMatter = fileNames.Any(f => 
            FrontMatterPatterns.Any(p => f.Contains(p, StringComparison.OrdinalIgnoreCase)));

        var hasAnhangPattern = fileNames.Any(f => 
            Regex.IsMatch(f, @"Anhang_[A-Z]_", RegexOptions.IgnoreCase));

        // Must have chapter pattern and at least one other mitp characteristic
        return hasKapitelPattern && (hasFrontMatter || hasAnhangPattern);
    }

    /// <inheritdoc />
    protected override FileCategory CategorizeFile(string filePath)
    {
        var fileName = GetFileName(filePath);

        if (FrontMatterPatterns.Any(p => fileName.Contains(p, StringComparison.OrdinalIgnoreCase)))
        {
            return FileCategory.FrontMatter;
        }

        if (Regex.IsMatch(fileName, @"Kapitel_\d+_", RegexOptions.IgnoreCase))
        {
            return FileCategory.Chapter;
        }

        if (Regex.IsMatch(fileName, @"Anhang_[A-Z]_", RegexOptions.IgnoreCase))
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
        // Order: Cover, Titel, Inhaltsverzeichnis, Einleitung, über den Autor
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
        return "Z";
    }

    private static string GetBackMatterSortKey(string fileName)
    {
        // Order: Glossar, Stichwortverzeichnis
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
