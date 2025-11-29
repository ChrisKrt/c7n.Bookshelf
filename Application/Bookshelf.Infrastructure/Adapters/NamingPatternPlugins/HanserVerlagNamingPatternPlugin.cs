using Bookshelf.Application.Spi;
using System.Text.RegularExpressions;

namespace Bookshelf.Infrastructure.Adapters.NamingPatternPlugins;

/// <summary>
/// Plugin for handling Hanser Verlag ISBN-based naming patterns.
/// Handles patterns like 9783446######.fm.pdf, 9783446######.001.pdf, 9783446######.bm.pdf.
/// </summary>
public sealed class HanserVerlagNamingPatternPlugin : INamingPatternPlugin
{
    /// <summary>
    /// Regex pattern for Hanser ISBN-based naming: 9783446xxxxxx.xxx.pdf
    /// </summary>
    private static readonly Regex HanserPattern = new(
        @"^9783446\d{6}\.(\w+)\.pdf$", 
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    /// <summary>
    /// Regex pattern for chapter numbers like .001, .002, .010
    /// </summary>
    private static readonly Regex ChapterNumberPattern = new(
        @"^(\d{3})$", 
        RegexOptions.Compiled);

    /// <inheritdoc />
    public string Name => "Hanser Verlag";

    /// <inheritdoc />
    public bool CanHandle(IReadOnlyList<string> pdfFilePaths)
    {
        var fileNames = pdfFilePaths
            .Select(Path.GetFileName)
            .Where(f => f != null)
            .ToList();

        // Check if files match the Hanser ISBN pattern (9783446xxxxxx.xxx.pdf)
        var matchingFiles = fileNames.Count(f => HanserPattern.IsMatch(f!));

        // Need at least 2 matching files to be confident
        return matchingFiles >= 2;
    }

    /// <inheritdoc />
    public IReadOnlyList<string> OrderFiles(IReadOnlyList<string> pdfFilePaths)
    {
        var frontMatter = new List<string>();
        var chapters = new List<(string Path, int Number)>();
        var backMatter = new List<string>();
        var unmatched = new List<string>();

        foreach (var filePath in pdfFilePaths)
        {
            var fileName = Path.GetFileName(filePath) ?? string.Empty;
            var match = HanserPattern.Match(fileName);

            if (!match.Success)
            {
                unmatched.Add(filePath);
                continue;
            }

            var suffix = match.Groups[1].Value.ToLowerInvariant();

            // Front matter (.fm)
            if (suffix == "fm")
            {
                frontMatter.Add(filePath);
                continue;
            }

            // Back matter (.bm)
            if (suffix == "bm")
            {
                backMatter.Add(filePath);
                continue;
            }

            // Chapter numbers (.001, .002, etc.)
            if (ChapterNumberPattern.IsMatch(suffix))
            {
                var chapterNumber = int.Parse(suffix);
                chapters.Add((filePath, chapterNumber));
                continue;
            }

            unmatched.Add(filePath);
        }

        // Order chapters numerically
        var orderedChapters = chapters.OrderBy(c => c.Number).Select(c => c.Path).ToList();
        var orderedUnmatched = unmatched.OrderBy(f => f, StringComparer.OrdinalIgnoreCase).ToList();

        // Combine in order: front matter, chapters, back matter, unmatched
        var result = new List<string>();
        result.AddRange(frontMatter);
        result.AddRange(orderedChapters);
        result.AddRange(backMatter);
        result.AddRange(orderedUnmatched);

        return result;
    }
}
