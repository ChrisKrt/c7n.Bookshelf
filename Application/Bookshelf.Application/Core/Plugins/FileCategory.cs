namespace Bookshelf.Application.Core.Plugins;

/// <summary>
/// Represents the category of a file in a book structure
/// </summary>
public enum FileCategory
{
    /// <summary>
    /// Front matter (Cover, Title, Table of Contents, Preface)
    /// </summary>
    FrontMatter = 0,

    /// <summary>
    /// Part or section divider (Teil, Part)
    /// </summary>
    Part = 1,

    /// <summary>
    /// Main content chapters
    /// </summary>
    Chapter = 2,

    /// <summary>
    /// Appendices (Anhang)
    /// </summary>
    Appendix = 3,

    /// <summary>
    /// Back matter (Index, Glossary)
    /// </summary>
    BackMatter = 4,

    /// <summary>
    /// Unknown category - will be sorted after front matter
    /// </summary>
    Unknown = 99
}
