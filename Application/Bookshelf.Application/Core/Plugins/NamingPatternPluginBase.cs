using System.Text.RegularExpressions;

namespace Bookshelf.Application.Core.Plugins;

/// <summary>
/// Base class for naming pattern plugins providing common functionality
/// </summary>
public abstract class NamingPatternPluginBase : INamingPatternPlugin
{
    /// <inheritdoc />
    public abstract string PluginName { get; }

    /// <inheritdoc />
    public virtual int Priority => 0;

    /// <inheritdoc />
    public abstract bool CanHandle(IReadOnlyList<string> pdfFilePaths);

    /// <inheritdoc />
    public virtual IReadOnlyList<string> OrderFiles(IReadOnlyList<string> pdfFilePaths)
    {
        if (pdfFilePaths == null)
        {
            throw new ArgumentNullException(nameof(pdfFilePaths));
        }

        var filteredFiles = FilterFiles(pdfFilePaths);
        var categorizedFiles = filteredFiles
            .Select(f => new { Path = f, Category = CategorizeFile(f), SortKey = GetSortKey(f) })
            .OrderBy(f => f.Category)
            .ThenBy(f => f.SortKey, StringComparer.OrdinalIgnoreCase)
            .Select(f => f.Path)
            .ToList();

        return categorizedFiles;
    }

    /// <inheritdoc />
    public virtual IReadOnlyList<string> FilterFiles(IReadOnlyList<string> pdfFilePaths)
    {
        if (pdfFilePaths == null)
        {
            throw new ArgumentNullException(nameof(pdfFilePaths));
        }

        return pdfFilePaths
            .Where(f => !IsDuplicateFile(f))
            .ToList();
    }

    /// <summary>
    /// Categorizes a file into its book section category
    /// </summary>
    /// <param name="filePath">The file path to categorize</param>
    /// <returns>The file category</returns>
    protected abstract FileCategory CategorizeFile(string filePath);

    /// <summary>
    /// Gets the sort key for ordering files within the same category
    /// </summary>
    /// <param name="filePath">The file path to get the sort key for</param>
    /// <returns>A string sort key for ordering</returns>
    protected abstract string GetSortKey(string filePath);

    /// <summary>
    /// Determines if a file is a duplicate that should be filtered out
    /// </summary>
    /// <param name="filePath">The file path to check</param>
    /// <returns>True if the file is a duplicate, false otherwise</returns>
    protected virtual bool IsDuplicateFile(string filePath)
    {
        var fileName = Path.GetFileName(filePath);
        // Check for common duplicate patterns like "(1)", "(2)", etc.
        return Regex.IsMatch(fileName, @"\(\d+\)\.[^.]+$");
    }

    /// <summary>
    /// Extracts a numeric value from a string for sorting
    /// </summary>
    /// <param name="input">The input string</param>
    /// <param name="pattern">The regex pattern with a capture group for the number</param>
    /// <returns>The extracted number, or int.MaxValue if not found</returns>
    protected static int ExtractNumber(string input, string pattern)
    {
        var match = Regex.Match(input, pattern, RegexOptions.IgnoreCase);
        if (match.Success && match.Groups.Count > 1)
        {
            if (int.TryParse(match.Groups[1].Value, out var number))
            {
                return number;
            }
        }
        return int.MaxValue;
    }

    /// <summary>
    /// Converts a Roman numeral to an integer
    /// </summary>
    /// <param name="roman">The Roman numeral string</param>
    /// <returns>The integer value</returns>
    protected static int RomanToInt(string roman)
    {
        var romanNumerals = new Dictionary<char, int>
        {
            { 'I', 1 }, { 'V', 5 }, { 'X', 10 }, { 'L', 50 },
            { 'C', 100 }, { 'D', 500 }, { 'M', 1000 }
        };

        var result = 0;
        var previousValue = 0;

        foreach (var c in roman.ToUpperInvariant().Reverse())
        {
            if (!romanNumerals.TryGetValue(c, out var value))
            {
                return int.MaxValue;
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
    /// Gets the file name without path for pattern matching
    /// </summary>
    /// <param name="filePath">The file path</param>
    /// <returns>The file name</returns>
    protected static string GetFileName(string filePath)
    {
        return Path.GetFileName(filePath);
    }
}
