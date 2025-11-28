using Bookshelf.Infrastructure.Adapters;

namespace Bookshelf.Infrastructure.Tests;

/// <summary>
/// Unit tests for NaturalStringComparer
/// </summary>
public class NaturalStringComparerTests
{
    private readonly NaturalStringComparer _comparer = NaturalStringComparer.Instance;

    [Theory]
    [InlineData("Kapitel_2", "Kapitel_10", -1)]
    [InlineData("Kapitel_10", "Kapitel_2", 1)]
    [InlineData("Kapitel_1", "Kapitel_1", 0)]
    [InlineData("Chapter_2", "Chapter_19", -1)]
    [InlineData("Chapter_19", "Chapter_2", 1)]
    public void Compare_NumericSequences_SortsNaturally(string x, string y, int expectedSign)
    {
        // Act
        var result = _comparer.Compare(x, y);

        // Assert
        var resultSign = Math.Sign(result);
        Assert.Equal(expectedSign, resultSign);
    }

    [Theory]
    [InlineData("file1", "file2", -1)]
    [InlineData("file10", "file2", 1)]
    [InlineData("file100", "file10", 1)]
    [InlineData("file1", "file100", -1)]
    public void Compare_NumberEdgeCases_HandlesLargeNumbers(string x, string y, int expectedSign)
    {
        // Act
        var result = _comparer.Compare(x, y);

        // Assert
        var resultSign = Math.Sign(result);
        Assert.Equal(expectedSign, resultSign);
    }

    [Theory]
    [InlineData(null, null, 0)]
    [InlineData(null, "test", -1)]
    [InlineData("test", null, 1)]
    public void Compare_NullValues_HandlesGracefully(string? x, string? y, int expectedSign)
    {
        // Act
        var result = _comparer.Compare(x, y);

        // Assert
        var resultSign = Math.Sign(result);
        Assert.Equal(expectedSign, resultSign);
    }

    [Fact]
    public void Compare_MixedTextAndNumbers_PreservesOrder()
    {
        // Arrange
        var files = new List<string>
        {
            "Kapitel_10.pdf",
            "Kapitel_2.pdf",
            "Kapitel_1.pdf",
            "Kapitel_100.pdf"
        };

        // Act
        var sorted = files.OrderBy(f => f, _comparer).ToList();

        // Assert
        Assert.Equal("Kapitel_1.pdf", sorted[0]);
        Assert.Equal("Kapitel_2.pdf", sorted[1]);
        Assert.Equal("Kapitel_10.pdf", sorted[2]);
        Assert.Equal("Kapitel_100.pdf", sorted[3]);
    }

    [Fact]
    public void Compare_GermanChapterNames_SortsCorrectly()
    {
        // Arrange
        var files = new List<string>
        {
            "Kapitel_19_Zusammenfassung.pdf",
            "Kapitel_2_Einleitung.pdf",
            "Kapitel_1_Vorwort.pdf"
        };

        // Act
        var sorted = files.OrderBy(f => f, _comparer).ToList();

        // Assert
        Assert.Equal("Kapitel_1_Vorwort.pdf", sorted[0]);
        Assert.Equal("Kapitel_2_Einleitung.pdf", sorted[1]);
        Assert.Equal("Kapitel_19_Zusammenfassung.pdf", sorted[2]);
    }
}
