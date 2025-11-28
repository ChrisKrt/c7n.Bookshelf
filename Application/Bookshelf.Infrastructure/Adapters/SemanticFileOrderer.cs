using System.Text.RegularExpressions;
using Bookshelf.Application.Spi;

namespace Bookshelf.Infrastructure.Adapters;

/// <summary>
/// Semantic file orderer that categorizes files into document structure sections
/// and applies natural number sorting within each category.
/// </summary>
public partial class SemanticFileOrderer : IFileOrderingStrategy
{
    /// <summary>
    /// Document section types in order of appearance
    /// </summary>
    private enum DocumentSection
    {
        Cover = 0,
        FrontMatter = 1,
        Chapters = 2,
        Appendices = 3,
        BackMatter = 4,
        Unknown = 5
    }

    // Regex patterns for German and English document structures
    [GeneratedRegex(@"(cover|deckblatt|titel|title)", RegexOptions.IgnoreCase)]
    private static partial Regex CoverPatternRegex();

    [GeneratedRegex(@"(inhaltsverzeichnis|contents|table.?of.?contents|toc|vorwort|preface|introduction|einleitung|einführung)", RegexOptions.IgnoreCase)]
    private static partial Regex FrontMatterPatternRegex();

    [GeneratedRegex(@"(kapitel|chapter|teil|part|abschnitt|section)", RegexOptions.IgnoreCase)]
    private static partial Regex ChapterPatternRegex();

    [GeneratedRegex(@"(anhang|appendix|appendices|annex)", RegexOptions.IgnoreCase)]
    private static partial Regex AppendixPatternRegex();

    [GeneratedRegex(@"(glossar|glossary|index|literatur|bibliography|references|quellenverzeichnis|nachwort|epilogue|afterword)", RegexOptions.IgnoreCase)]
    private static partial Regex BackMatterPatternRegex();

    /// <inheritdoc />
    public IReadOnlyList<string> OrderFiles(IEnumerable<string> files)
    {
        var fileList = files.ToList();
        var hasNoFiles = fileList.Count == 0;
        if (hasNoFiles)
        {
            return Array.Empty<string>();
        }

        var categorizedFiles = CategorizeFiles(fileList);
        var orderedFiles = OrderCategorizedFiles(categorizedFiles);

        return orderedFiles;
    }

    /// <summary>
    /// Categorizes files into document sections based on filename patterns
    /// </summary>
    private static Dictionary<DocumentSection, List<string>> CategorizeFiles(List<string> files)
    {
        var categorized = new Dictionary<DocumentSection, List<string>>
        {
            { DocumentSection.Cover, new List<string>() },
            { DocumentSection.FrontMatter, new List<string>() },
            { DocumentSection.Chapters, new List<string>() },
            { DocumentSection.Appendices, new List<string>() },
            { DocumentSection.BackMatter, new List<string>() },
            { DocumentSection.Unknown, new List<string>() }
        };

        foreach (var file in files)
        {
            var fileName = Path.GetFileNameWithoutExtension(file);
            var section = DetermineSection(fileName);
            categorized[section].Add(file);
        }

        return categorized;
    }

    /// <summary>
    /// Determines which document section a file belongs to based on its name
    /// </summary>
    private static DocumentSection DetermineSection(string fileName)
    {
        var isCover = CoverPatternRegex().IsMatch(fileName);
        if (isCover)
        {
            return DocumentSection.Cover;
        }

        var isFrontMatter = FrontMatterPatternRegex().IsMatch(fileName);
        if (isFrontMatter)
        {
            return DocumentSection.FrontMatter;
        }

        var isChapter = ChapterPatternRegex().IsMatch(fileName);
        if (isChapter)
        {
            return DocumentSection.Chapters;
        }

        var isAppendix = AppendixPatternRegex().IsMatch(fileName);
        if (isAppendix)
        {
            return DocumentSection.Appendices;
        }

        var isBackMatter = BackMatterPatternRegex().IsMatch(fileName);
        if (isBackMatter)
        {
            return DocumentSection.BackMatter;
        }

        return DocumentSection.Unknown;
    }

    /// <summary>
    /// Orders files within each category using natural string comparison
    /// </summary>
    private static List<string> OrderCategorizedFiles(Dictionary<DocumentSection, List<string>> categorizedFiles)
    {
        var orderedFiles = new List<string>();

        // Process sections in document order
        var sectionsInOrder = new[]
        {
            DocumentSection.Cover,
            DocumentSection.FrontMatter,
            DocumentSection.Chapters,
            DocumentSection.Appendices,
            DocumentSection.BackMatter,
            DocumentSection.Unknown
        };

        foreach (var section in sectionsInOrder)
        {
            var sectionFiles = categorizedFiles[section];
            var hasSectionFiles = sectionFiles.Count > 0;
            if (hasSectionFiles)
            {
                var sortedSectionFiles = SortFilesNaturally(sectionFiles);
                orderedFiles.AddRange(sortedSectionFiles);
            }
        }

        return orderedFiles;
    }

    /// <summary>
    /// Sorts files using natural string comparison on filenames
    /// </summary>
    private static List<string> SortFilesNaturally(List<string> files)
    {
        return files
            .OrderBy(f => Path.GetFileName(f), NaturalStringComparer.Instance)
            .ToList();
    }
}
