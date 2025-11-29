using Bookshelf.Application.Spi;
using System.Text.RegularExpressions;

namespace Bookshelf.Infrastructure.Adapters.NamingPatternPlugins;

/// <summary>
/// Plugin for handling Teil-based (Part-based) structure with chapters.
/// Handles patterns like Teil_I_, Teil_II_, Teil_III_, Kapitel_1_, Kapitel_2_,
/// BEGINN, Vorwort, Inhaltsverzeichnis, Index, Anhang.
/// </summary>
public sealed class TeilBasedNamingPatternPlugin : INamingPatternPlugin
{
    /// <summary>
    /// Front matter patterns in order
    /// </summary>
    private static readonly string[] FrontMatterPatterns = new[]
    {
        "BEGINN", "Vorwort", "Inhaltsverzeichnis"
    };

    /// <summary>
    /// Back matter patterns in order
    /// </summary>
    private static readonly string[] BackMatterPatterns = new[]
    {
        "Anhang", "Index"
    };

    /// <summary>
    /// Regex pattern for Teil (Part) like Teil_I_, Teil_II_, Teil_III_
    /// </summary>
    private static readonly Regex TeilPattern = new(
        @"Teil_([IVX]+|[0-9]+)_", 
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    /// <summary>
    /// Regex pattern for chapters like Kapitel_1_, Kapitel_2_
    /// </summary>
    private static readonly Regex ChapterPattern = new(
        @"Kapitel_(\d+)_", 
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    /// <inheritdoc />
    public string Name => "Teil-based";

    /// <inheritdoc />
    public bool CanHandle(IReadOnlyList<string> pdfFilePaths)
    {
        var fileNames = pdfFilePaths
            .Select(Path.GetFileName)
            .Where(f => f != null)
            .ToList();

        // Must have at least one Teil pattern
        var hasTeile = fileNames.Any(f => TeilPattern.IsMatch(f!));
        
        if (!hasTeile)
        {
            return false;
        }

        // Should have front matter or chapters as well
        var hasFrontMatter = fileNames.Any(f => 
            FrontMatterPatterns.Any(p => f!.Contains(p, StringComparison.OrdinalIgnoreCase)));
        var hasChapters = fileNames.Any(f => ChapterPattern.IsMatch(f!));

        return hasFrontMatter || hasChapters;
    }

    /// <inheritdoc />
    public IReadOnlyList<string> OrderFiles(IReadOnlyList<string> pdfFilePaths)
    {
        var frontMatter = new List<string>();
        var teileWithChapters = new List<(string Path, int TeilNumber, int? ChapterNumber)>();
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

            // Check back matter first
            if (IsBackMatter(fileName))
            {
                if (fileName.Contains("Anhang", StringComparison.OrdinalIgnoreCase))
                {
                    appendices.Add(filePath);
                }
                else
                {
                    backMatter.Add(filePath);
                }
                continue;
            }

            // Check Teil and/or Chapter
            var teilMatch = TeilPattern.Match(fileName);
            var chapterMatch = ChapterPattern.Match(fileName);

            if (teilMatch.Success || chapterMatch.Success)
            {
                var teilNumber = teilMatch.Success ? ParseRomanOrArabic(teilMatch.Groups[1].Value) : 0;
                int? chapterNumber = chapterMatch.Success ? int.Parse(chapterMatch.Groups[1].Value) : null;
                teileWithChapters.Add((filePath, teilNumber, chapterNumber));
                continue;
            }

            unmatched.Add(filePath);
        }

        // Order each category
        var orderedFrontMatter = OrderByPatternList(frontMatter, FrontMatterPatterns);
        
        // Order Teile first by Teil number, then by chapter number
        var orderedTeileWithChapters = teileWithChapters
            .OrderBy(t => t.TeilNumber)
            .ThenBy(t => t.ChapterNumber ?? 0)
            .Select(t => t.Path)
            .ToList();
        
        var orderedAppendices = appendices.OrderBy(f => f, StringComparer.OrdinalIgnoreCase).ToList();
        var orderedBackMatter = OrderByPatternList(backMatter, BackMatterPatterns);
        var orderedUnmatched = unmatched.OrderBy(f => f, StringComparer.OrdinalIgnoreCase).ToList();

        // Combine in order: front matter, teile with chapters, appendices, back matter, unmatched
        var result = new List<string>();
        result.AddRange(orderedFrontMatter);
        result.AddRange(orderedTeileWithChapters);
        result.AddRange(orderedAppendices);
        result.AddRange(orderedBackMatter);
        result.AddRange(orderedUnmatched);

        return result;
    }

    /// <summary>
    /// Parses a Roman numeral or Arabic number to an integer
    /// </summary>
    private static int ParseRomanOrArabic(string value)
    {
        // Try Arabic number first
        if (int.TryParse(value, out var arabicNumber))
        {
            return arabicNumber;
        }

        // Parse Roman numerals
        return ParseRomanNumeral(value.ToUpperInvariant());
    }

    /// <summary>
    /// Parses a Roman numeral to an integer
    /// </summary>
    private static int ParseRomanNumeral(string roman)
    {
        var romanNumerals = new Dictionary<char, int>
        {
            { 'I', 1 },
            { 'V', 5 },
            { 'X', 10 },
            { 'L', 50 },
            { 'C', 100 },
            { 'D', 500 },
            { 'M', 1000 }
        };

        var result = 0;
        var previousValue = 0;

        foreach (var c in roman.Reverse())
        {
            if (!romanNumerals.TryGetValue(c, out var value))
            {
                return 0; // Invalid character
            }

            if (value < previousValue)
            {
                result -= value;
            }
            else
            {
                result += value;
            }

            previousValue = value;
        }

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
