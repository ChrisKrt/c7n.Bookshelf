using Bookshelf.Infrastructure.Adapters;

namespace Bookshelf.Infrastructure.Tests;

/// <summary>
/// Unit tests for SemanticFileOrderer
/// </summary>
public class SemanticFileOrdererTests
{
    private readonly SemanticFileOrderer _orderer = new();

    [Fact]
    public void OrderFiles_GermanBookStructure_OrdersCorrectly()
    {
        // Arrange
        var files = new List<string>
        {
            "/path/Kapitel_19.pdf",
            "/path/Anhang_A.pdf",
            "/path/Kapitel_2.pdf",
            "/path/Deckblatt.pdf",
            "/path/Inhaltsverzeichnis.pdf",
            "/path/Glossar.pdf",
            "/path/Kapitel_1.pdf"
        };

        // Act
        var ordered = _orderer.OrderFiles(files);

        // Assert - order should be: Cover, TOC, Chapters (1, 2, 19), Appendix, Glossary
        Assert.Equal(7, ordered.Count);
        Assert.Contains("Deckblatt", ordered[0]); // Cover first
        Assert.Contains("Inhaltsverzeichnis", ordered[1]); // TOC second
        Assert.Contains("Kapitel_1", ordered[2]); // Chapter 1
        Assert.Contains("Kapitel_2", ordered[3]); // Chapter 2
        Assert.Contains("Kapitel_19", ordered[4]); // Chapter 19
        Assert.Contains("Anhang_A", ordered[5]); // Appendix
        Assert.Contains("Glossar", ordered[6]); // Glossary
    }

    [Fact]
    public void OrderFiles_EnglishBookStructure_OrdersCorrectly()
    {
        // Arrange
        var files = new List<string>
        {
            "/path/Chapter_10.pdf",
            "/path/Appendix_A.pdf",
            "/path/Chapter_2.pdf",
            "/path/Cover.pdf",
            "/path/Table_of_Contents.pdf",
            "/path/Index.pdf",
            "/path/Chapter_1.pdf"
        };

        // Act
        var ordered = _orderer.OrderFiles(files);

        // Assert - order should be: Cover, TOC, Chapters (1, 2, 10), Appendix, Index
        Assert.Equal(7, ordered.Count);
        Assert.Contains("Cover", ordered[0]);
        Assert.Contains("Table_of_Contents", ordered[1]);
        Assert.Contains("Chapter_1", ordered[2]);
        Assert.Contains("Chapter_2", ordered[3]);
        Assert.Contains("Chapter_10", ordered[4]);
        Assert.Contains("Appendix", ordered[5]);
        Assert.Contains("Index", ordered[6]);
    }

    [Fact]
    public void OrderFiles_NumericEdgeCases_HandlesCorrectly()
    {
        // Arrange
        var files = new List<string>
        {
            "/path/Kapitel_100.pdf",
            "/path/Kapitel_10.pdf",
            "/path/Kapitel_1.pdf",
            "/path/Kapitel_2.pdf",
            "/path/Kapitel_20.pdf"
        };

        // Act
        var ordered = _orderer.OrderFiles(files);

        // Assert - should be in natural numeric order
        Assert.Equal(5, ordered.Count);
        Assert.Contains("Kapitel_1.pdf", ordered[0]);
        Assert.Contains("Kapitel_2.pdf", ordered[1]);
        Assert.Contains("Kapitel_10.pdf", ordered[2]);
        Assert.Contains("Kapitel_20.pdf", ordered[3]);
        Assert.Contains("Kapitel_100.pdf", ordered[4]);
    }

    [Fact]
    public void OrderFiles_EmptyList_ReturnsEmpty()
    {
        // Arrange
        var files = new List<string>();

        // Act
        var ordered = _orderer.OrderFiles(files);

        // Assert
        Assert.Empty(ordered);
    }

    [Fact]
    public void OrderFiles_SingleFile_ReturnsSingleFile()
    {
        // Arrange
        var files = new List<string> { "/path/Chapter_1.pdf" };

        // Act
        var ordered = _orderer.OrderFiles(files);

        // Assert
        Assert.Single(ordered);
        Assert.Equal("/path/Chapter_1.pdf", ordered[0]);
    }

    [Fact]
    public void OrderFiles_UnrecognizedPatterns_PlacedAtEnd()
    {
        // Arrange
        var files = new List<string>
        {
            "/path/RandomFile.pdf",
            "/path/Cover.pdf",
            "/path/SomeDocument.pdf",
            "/path/Chapter_1.pdf"
        };

        // Act
        var ordered = _orderer.OrderFiles(files);

        // Assert - Cover and Chapter should come first, unknown files at end
        Assert.Equal(4, ordered.Count);
        Assert.Contains("Cover", ordered[0]);
        Assert.Contains("Chapter_1", ordered[1]);
        // Unknown files should be at the end, sorted naturally
        var lastTwoAreUnknown = 
            (ordered[2].Contains("Random") || ordered[2].Contains("Some")) &&
            (ordered[3].Contains("Random") || ordered[3].Contains("Some"));
        Assert.True(lastTwoAreUnknown);
    }

    [Fact]
    public void OrderFiles_MixedGermanEnglish_HandlesGracefully()
    {
        // Arrange
        var files = new List<string>
        {
            "/path/Kapitel_2.pdf",
            "/path/Chapter_1.pdf",
            "/path/Anhang_A.pdf",
            "/path/Appendix_B.pdf"
        };

        // Act
        var ordered = _orderer.OrderFiles(files);

        // Assert - Chapters before appendices
        Assert.Equal(4, ordered.Count);
        
        // Find indices
        var chapter1Index = ordered.ToList().FindIndex(f => f.Contains("Chapter_1"));
        var kapitel2Index = ordered.ToList().FindIndex(f => f.Contains("Kapitel_2"));
        var anhangIndex = ordered.ToList().FindIndex(f => f.Contains("Anhang_A"));
        var appendixIndex = ordered.ToList().FindIndex(f => f.Contains("Appendix_B"));
        
        // Chapters should come before appendices
        Assert.True(chapter1Index < anhangIndex);
        Assert.True(chapter1Index < appendixIndex);
        Assert.True(kapitel2Index < anhangIndex);
        Assert.True(kapitel2Index < appendixIndex);
    }

    [Fact]
    public void OrderFiles_DuplicateFilenames_HandledConsistently()
    {
        // Arrange - simulate files from different paths but same name
        var files = new List<string>
        {
            "/path1/Chapter_1.pdf",
            "/path2/Chapter_1.pdf",
            "/path1/Chapter_2.pdf"
        };

        // Act
        var ordered = _orderer.OrderFiles(files);

        // Assert - should still maintain order
        Assert.Equal(3, ordered.Count);
    }
}
