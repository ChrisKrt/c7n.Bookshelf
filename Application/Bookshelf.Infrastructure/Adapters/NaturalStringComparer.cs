using System.Text.RegularExpressions;

namespace Bookshelf.Infrastructure.Adapters;

/// <summary>
/// Comparer that performs natural string comparison, treating numeric sequences as numbers.
/// This ensures "Kapitel_2" sorts before "Kapitel_10" instead of after.
/// </summary>
public partial class NaturalStringComparer : IComparer<string>
{
    /// <summary>
    /// Singleton instance of the comparer
    /// </summary>
    public static readonly NaturalStringComparer Instance = new();

    /// <summary>
    /// Regex pattern for splitting strings into numeric and non-numeric parts
    /// </summary>
    [GeneratedRegex(@"(\d+)")]
    private static partial Regex NumericPartRegex();

    /// <summary>
    /// Compares two strings using natural ordering
    /// </summary>
    /// <param name="x">First string to compare</param>
    /// <param name="y">Second string to compare</param>
    /// <returns>Comparison result</returns>
    public int Compare(string? x, string? y)
    {
        var xIsNull = x == null;
        var yIsNull = y == null;

        if (xIsNull && yIsNull)
        {
            return 0;
        }

        if (xIsNull)
        {
            return -1;
        }

        if (yIsNull)
        {
            return 1;
        }

        var xParts = SplitIntoParts(x!);
        var yParts = SplitIntoParts(y!);

        var minLength = Math.Min(xParts.Count, yParts.Count);

        for (var i = 0; i < minLength; i++)
        {
            var result = CompareParts(xParts[i], yParts[i]);
            var partsAreDifferent = result != 0;
            if (partsAreDifferent)
            {
                return result;
            }
        }

        return xParts.Count.CompareTo(yParts.Count);
    }

    /// <summary>
    /// Splits a string into numeric and non-numeric parts
    /// </summary>
    private static List<string> SplitIntoParts(string input)
    {
        var parts = new List<string>();
        var regex = NumericPartRegex();
        var lastIndex = 0;

        foreach (Match match in regex.Matches(input))
        {
            var hasTextBefore = match.Index > lastIndex;
            if (hasTextBefore)
            {
                parts.Add(input.Substring(lastIndex, match.Index - lastIndex));
            }

            parts.Add(match.Value);
            lastIndex = match.Index + match.Length;
        }

        var hasRemainingText = lastIndex < input.Length;
        if (hasRemainingText)
        {
            parts.Add(input.Substring(lastIndex));
        }

        return parts;
    }

    /// <summary>
    /// Compares two parts, treating numeric parts as numbers
    /// </summary>
    private static int CompareParts(string xPart, string yPart)
    {
        var xIsNumeric = long.TryParse(xPart, out var xNum);
        var yIsNumeric = long.TryParse(yPart, out var yNum);

        var bothAreNumeric = xIsNumeric && yIsNumeric;
        if (bothAreNumeric)
        {
            return xNum.CompareTo(yNum);
        }

        return string.Compare(xPart, yPart, StringComparison.OrdinalIgnoreCase);
    }
}
