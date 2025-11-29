using Bookshelf.Application.Spi;
using System.Text.RegularExpressions;

namespace Bookshelf.Infrastructure.Adapters.NamingPatternPlugins;

/// <summary>
/// Plugin for handling O'Reilly publisher naming patterns.
/// Handles patterns like BEGINN, Inhalt, Vorwort, Kapitel_1_, Kapitel_2_, Chapter_1_, Index, Anhang.
/// </summary>
public sealed class OReillyNamingPatternPlugin : INamingPatternPlugin
{
    /// <summary>
    /// Patterns that indicate O'Reilly naming convention
    /// </summary>
    private static readonly string[] OReillyIndicators = new[]
    {
        "BEGINN", "Inhalt", "Vorwort", "Kapitel_", "Chapter_", "Index", "Anhang"
    };

    /// <summary>
    /// Front matter patterns in order
    /// </summary>
    private static readonly string[] FrontMatterPatterns = new[]
    {
        "BEGINN", "Inhalt", "Vorwort"
    };

    /// <summary>
    /// Back matter patterns in order
    /// </summary>
    private static readonly string[] BackMatterPatterns = new[]
    {
        "Anhang", "Index"
    };

    /// <summary>
    /// Regex pattern for chapters like Kapitel_1_, Kapitel_2_, Chapter_1_
    /// </summary>
    private static readonly Regex ChapterPattern = new(
        @"(?:Kapitel|Chapter)_(\d+)_", 
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    /// <inheritdoc />
    public string Name => "O'Reilly";

    /// <inheritdoc />
    public bool CanHandle(IReadOnlyList<string> pdfFilePaths)
    {
        var fileNames = pdfFilePaths
            .Select(Path.GetFileName)
            .Where(f => f != null)
            .ToList();

        // Check if at least 2 O'Reilly indicators are present
        var matchCount = OReillyIndicators.Count(indicator =>
            fileNames.Any(f => f!.Contains(indicator, StringComparison.OrdinalIgnoreCase)));

        // Must have BEGINN or have both chapter pattern and other indicators
        var hasBeginn = fileNames.Any(f => f!.Contains("BEGINN", StringComparison.OrdinalIgnoreCase));
        var hasChapters = fileNames.Any(f => ChapterPattern.IsMatch(f!));

        return (hasBeginn && matchCount >= 2) || (hasChapters && matchCount >= 3);
    }

    /// <inheritdoc />
    public IReadOnlyList<string> OrderFiles(IReadOnlyList<string> pdfFilePaths)
    {
        var frontMatter = new List<string>();
        var chapters = new List<(string Path, int Number)>();
        var appendices = new List<string>();
        var backMatter = new List<string>();
        var unmatched = new List<string>();

        foreach (var filePath in pdfFilePaths)
        {
            var fileName = Path.GetFileName(filePath) ?? string.Empty;
            
            // Check front matter
            if (IsFrontMatter(fileName))
            {
                frontMatter.Add(filePath);
                continue;
            }

            // Check chapters
            var chapterMatch = ChapterPattern.Match(fileName);
            if (chapterMatch.Success)
            {
                var chapterNumber = int.Parse(chapterMatch.Groups[1].Value);
                chapters.Add((filePath, chapterNumber));
                continue;
            }

            // Check appendices (Anhang)
            if (fileName.Contains("Anhang", StringComparison.OrdinalIgnoreCase))
            {
                appendices.Add(filePath);
                continue;
            }

            // Check back matter (Index)
            if (fileName.Contains("Index", StringComparison.OrdinalIgnoreCase))
            {
                backMatter.Add(filePath);
                continue;
            }

            unmatched.Add(filePath);
        }

        // Order each category
        var orderedFrontMatter = OrderByPatternList(frontMatter, FrontMatterPatterns);
        var orderedChapters = chapters.OrderBy(c => c.Number).Select(c => c.Path).ToList();
        var orderedAppendices = appendices.OrderBy(f => f, StringComparer.OrdinalIgnoreCase).ToList();
        var orderedBackMatter = OrderByPatternList(backMatter, BackMatterPatterns);
        var orderedUnmatched = unmatched.OrderBy(f => f, StringComparer.OrdinalIgnoreCase).ToList();

        // Combine in order: front matter, chapters, appendices, back matter, unmatched
        var result = new List<string>();
        result.AddRange(orderedFrontMatter);
        result.AddRange(orderedChapters);
        result.AddRange(orderedAppendices);
        result.AddRange(orderedBackMatter);
        result.AddRange(orderedUnmatched);

        return result;
    }

    /// <summary>
    /// Checks if a file is front matter
    /// </summary>
    private static bool IsFrontMatter(string fileName)
    {
        return FrontMatterPatterns.Any(p => fileName.Contains(p, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Orders files by their position in a pattern list
    /// </summary>
    private static List<string> OrderByPatternList(List<string> files, string[] patterns)
    {
        return files
            .Select(f => new
            {
                Path = f,
                Index = Array.FindIndex(patterns, p =>
                    (Path.GetFileName(f) ?? string.Empty).Contains(p, StringComparison.OrdinalIgnoreCase))
            })
            .OrderBy(x => x.Index < 0 ? int.MaxValue : x.Index)
            .Select(x => x.Path)
            .ToList();
    }
}
