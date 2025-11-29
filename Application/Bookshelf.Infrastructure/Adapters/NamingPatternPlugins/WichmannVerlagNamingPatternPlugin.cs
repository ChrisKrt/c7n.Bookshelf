using Bookshelf.Application.Spi;
using System.Text.RegularExpressions;

namespace Bookshelf.Infrastructure.Adapters.NamingPatternPlugins;

/// <summary>
/// Plugin for handling Wichmann Verlag naming patterns.
/// Handles patterns like Vorwort, Inhalt, _1_, _2_, ..., _8_, Anhnge, Stichwortverzeichnis.
/// </summary>
public sealed class WichmannVerlagNamingPatternPlugin : INamingPatternPlugin
{
    /// <summary>
    /// Patterns that indicate Wichmann Verlag naming convention
    /// </summary>
    private static readonly string[] WichmannIndicators = new[]
    {
        "Vorwort", "Inhalt", "_1_", "_2_", "Anhnge", "Stichwortverzeichnis"
    };

    /// <summary>
    /// Front matter patterns in order
    /// </summary>
    private static readonly string[] FrontMatterPatterns = new[]
    {
        "Vorwort", "Inhalt"
    };

    /// <summary>
    /// Back matter patterns in order
    /// </summary>
    private static readonly string[] BackMatterPatterns = new[]
    {
        "Anhnge", "Stichwortverzeichnis"
    };

    /// <summary>
    /// Regex pattern for chapters like _1_, _2_, _10_
    /// </summary>
    private static readonly Regex ChapterPattern = new(@"_(\d+)_", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    /// <inheritdoc />
    public string Name => "Wichmann Verlag";

    /// <inheritdoc />
    public bool CanHandle(IReadOnlyList<string> pdfFilePaths)
    {
        var fileNames = pdfFilePaths
            .Select(Path.GetFileName)
            .Where(f => f != null)
            .ToList();

        // Check if at least 3 Wichmann indicators are present
        var matchCount = WichmannIndicators.Count(indicator =>
            fileNames.Any(f => f!.Contains(indicator, StringComparison.OrdinalIgnoreCase)));

        // Must have at least 2 indicators and the specific combination of Vorwort or Inhalt + numbered chapters
        var hasFrontMatter = fileNames.Any(f => 
            f!.Contains("Vorwort", StringComparison.OrdinalIgnoreCase) || 
            f.Contains("Inhalt", StringComparison.OrdinalIgnoreCase));
        
        var hasNumberedChapters = fileNames.Any(f => ChapterPattern.IsMatch(f!));

        return matchCount >= 2 && hasFrontMatter && hasNumberedChapters;
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

            // Check back matter first (before chapter pattern to avoid matching _1_ in Anhnge)
            if (IsBackMatter(fileName))
            {
                backMatter.Add(filePath);
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
            if (fileName.Contains("Anhnge", StringComparison.OrdinalIgnoreCase))
            {
                appendices.Add(filePath);
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
    /// Checks if a file is back matter
    /// </summary>
    private static bool IsBackMatter(string fileName)
    {
        return BackMatterPatterns.Any(p => fileName.Contains(p, StringComparison.OrdinalIgnoreCase));
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
