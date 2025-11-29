using Bookshelf.Application.Spi;
using System.Text.RegularExpressions;

namespace Bookshelf.Infrastructure.Adapters.NamingPatternPlugins;

/// <summary>
/// Plugin for handling mitp publisher naming patterns.
/// Handles patterns like Cover, Titel, Inhaltsverzeichnis, Einleitung, Kapitel_1_, Kapitel_2_, 
/// Anhang_A_, Anhang_B_, Glossar, Stichwortverzeichnis.
/// Ignores duplicate files with "(1)" suffix.
/// </summary>
public sealed class MitpNamingPatternPlugin : INamingPatternPlugin
{
    /// <summary>
    /// Patterns that indicate mitp publisher naming convention
    /// </summary>
    private static readonly string[] MitpIndicators = new[]
    {
        "Cover", "Titel", "Inhaltsverzeichnis", "Einleitung",
        "Kapitel_", "Anhang_", "Glossar", "Stichwortverzeichnis"
    };

    /// <summary>
    /// Front matter patterns in order
    /// </summary>
    private static readonly string[] FrontMatterPatterns = new[]
    {
        "Cover", "Titel", "Inhaltsverzeichnis", "Einleitung"
    };

    /// <summary>
    /// Back matter patterns in order
    /// </summary>
    private static readonly string[] BackMatterPatterns = new[]
    {
        "Glossar", "Stichwortverzeichnis"
    };

    /// <summary>
    /// Regex pattern for chapters like Kapitel_1_, Kapitel_2_, Kapitel_10_
    /// </summary>
    private static readonly Regex ChapterPattern = new(@"Kapitel_(\d+)_", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    /// <summary>
    /// Regex pattern for appendices like Anhang_A_, Anhang_B_
    /// </summary>
    private static readonly Regex AppendixPattern = new(@"Anhang_([A-Z])_", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    /// <summary>
    /// Regex pattern for duplicate files with "(1)" suffix
    /// </summary>
    private static readonly Regex DuplicatePattern = new(@"\(\d+\)\.(pdf)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    /// <inheritdoc />
    public string Name => "mitp";

    /// <inheritdoc />
    public bool CanHandle(IReadOnlyList<string> pdfFilePaths)
    {
        var fileNames = pdfFilePaths
            .Select(Path.GetFileName)
            .Where(f => f != null)
            .ToList();

        // Check if at least 3 mitp indicators are present
        var matchCount = MitpIndicators.Count(indicator =>
            fileNames.Any(f => f!.Contains(indicator, StringComparison.OrdinalIgnoreCase)));

        return matchCount >= 3;
    }

    /// <inheritdoc />
    public IReadOnlyList<string> OrderFiles(IReadOnlyList<string> pdfFilePaths)
    {
        // Filter out duplicate files with "(1)" suffix
        var filteredFiles = pdfFilePaths
            .Where(f => !DuplicatePattern.IsMatch(Path.GetFileName(f) ?? string.Empty))
            .ToList();

        var frontMatter = new List<string>();
        var chapters = new List<(string Path, int Number)>();
        var appendices = new List<(string Path, char Letter)>();
        var backMatter = new List<string>();
        var unmatched = new List<string>();

        foreach (var filePath in filteredFiles)
        {
            var fileName = Path.GetFileName(filePath) ?? string.Empty;
            
            // Check front matter
            var frontMatterIndex = GetFrontMatterIndex(fileName);
            if (frontMatterIndex >= 0)
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

            // Check appendices
            var appendixMatch = AppendixPattern.Match(fileName);
            if (appendixMatch.Success)
            {
                var appendixLetter = char.ToUpper(appendixMatch.Groups[1].Value[0]);
                appendices.Add((filePath, appendixLetter));
                continue;
            }

            // Check back matter
            var backMatterIndex = GetBackMatterIndex(fileName);
            if (backMatterIndex >= 0)
            {
                backMatter.Add(filePath);
                continue;
            }

            unmatched.Add(filePath);
        }

        // Order each category
        var orderedFrontMatter = OrderByPatternList(frontMatter, FrontMatterPatterns);
        var orderedChapters = chapters.OrderBy(c => c.Number).Select(c => c.Path).ToList();
        var orderedAppendices = appendices.OrderBy(a => a.Letter).Select(a => a.Path).ToList();
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
    /// Gets the index of a file in the front matter patterns
    /// </summary>
    private static int GetFrontMatterIndex(string fileName)
    {
        for (int i = 0; i < FrontMatterPatterns.Length; i++)
        {
            if (fileName.Contains(FrontMatterPatterns[i], StringComparison.OrdinalIgnoreCase))
            {
                return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// Gets the index of a file in the back matter patterns
    /// </summary>
    private static int GetBackMatterIndex(string fileName)
    {
        for (int i = 0; i < BackMatterPatterns.Length; i++)
        {
            if (fileName.Contains(BackMatterPatterns[i], StringComparison.OrdinalIgnoreCase))
            {
                return i;
            }
        }
        return -1;
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
