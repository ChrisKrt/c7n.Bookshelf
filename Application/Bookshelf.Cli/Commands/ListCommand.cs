using System.ComponentModel;
using Bookshelf.Application.Api;
using Bookshelf.Application.Api.Dtos;
using Bookshelf.Application.Core.Entities;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Bookshelf.Cli.Commands;

/// <summary>
/// Command settings for the list command
/// </summary>
public sealed class ListSettings : CommandSettings
{
    /// <summary>
    /// Gets or sets the bookshelf directory to list books from
    /// </summary>
    [CommandArgument(0, "<BOOKSHELF>")]
    [Description("The bookshelf directory containing PDF files")]
    public string BookshelfDirectory { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the title filter
    /// </summary>
    [CommandOption("-f|--filter <FILTER>")]
    [Description("Filter books by title (case-insensitive search)")]
    public string? TitleFilter { get; set; }

    /// <summary>
    /// Gets or sets whether to show detailed information
    /// </summary>
    [CommandOption("-d|--details")]
    [Description("Show detailed book information including file size, creation date, and page count")]
    [DefaultValue(false)]
    public bool ShowDetails { get; set; }

    /// <summary>
    /// Gets or sets the sort field
    /// </summary>
    [CommandOption("-s|--sort <FIELD>")]
    [Description("Sort by: title, size, date, or pages")]
    [DefaultValue("title")]
    public string SortField { get; set; } = "title";

    /// <summary>
    /// Gets or sets whether to reverse the sort order
    /// </summary>
    [CommandOption("-r|--reverse")]
    [Description("Reverse the sort order (descending instead of ascending)")]
    [DefaultValue(false)]
    public bool ReverseSort { get; set; }

    /// <summary>
    /// Validates the command settings
    /// </summary>
    public override ValidationResult Validate()
    {
        if (string.IsNullOrWhiteSpace(BookshelfDirectory))
        {
            return ValidationResult.Error("Bookshelf directory is required");
        }

        if (!Directory.Exists(BookshelfDirectory))
        {
            return ValidationResult.Error($"Bookshelf directory does not exist: {BookshelfDirectory}");
        }

        var validSortFields = new[] { "title", "size", "date", "pages" };
        var isValidSortField = validSortFields.Contains(SortField.ToLowerInvariant());
        if (!isValidSortField)
        {
            return ValidationResult.Error($"Invalid sort field: {SortField}. Valid options: title, size, date, pages");
        }

        return ValidationResult.Success();
    }

    /// <summary>
    /// Gets the sort field enum value from string
    /// </summary>
    public BookListSortField GetSortFieldEnum()
    {
        return SortField.ToLowerInvariant() switch
        {
            "title" => BookListSortField.Title,
            "size" => BookListSortField.FileSize,
            "date" => BookListSortField.CreationDate,
            "pages" => BookListSortField.PageCount,
            _ => BookListSortField.Title
        };
    }

    /// <summary>
    /// Gets the sort direction
    /// </summary>
    public SortDirection GetSortDirection()
    {
        return ReverseSort ? SortDirection.Descending : SortDirection.Ascending;
    }
}

/// <summary>
/// Command for listing books in a bookshelf
/// </summary>
public sealed class ListCommand : AsyncCommand<ListSettings>
{
    private readonly IBookshelfListService _listService;

    /// <summary>
    /// Initializes a new instance of the ListCommand class
    /// </summary>
    /// <param name="listService">The list service</param>
    public ListCommand(IBookshelfListService listService)
    {
        _listService = listService ?? throw new ArgumentNullException(nameof(listService));
    }

    /// <summary>
    /// Executes the list command
    /// </summary>
    public override async Task<int> ExecuteAsync(CommandContext context, ListSettings settings, CancellationToken cancellationToken)
    {
        var panel = new Panel("[bold]Bookshelf Listing[/]")
            .Border(BoxBorder.Rounded)
            .BorderColor(Color.Blue);

        AnsiConsole.Write(panel);
        AnsiConsole.WriteLine();

        AnsiConsole.MarkupLine($"[grey]Bookshelf:[/] [cyan]{settings.BookshelfDirectory}[/]");
        
        var hasFilter = !string.IsNullOrWhiteSpace(settings.TitleFilter);
        if (hasFilter)
        {
            AnsiConsole.MarkupLine($"[grey]Filter:[/] [cyan]{settings.TitleFilter}[/]");
        }
        
        AnsiConsole.WriteLine();

        var request = new ListBooksRequest(
            settings.BookshelfDirectory,
            settings.TitleFilter,
            settings.ShowDetails,
            settings.GetSortFieldEnum(),
            settings.GetSortDirection());

        var result = await _listService.ListBooksAsync(request, cancellationToken);

        if (!result.Success)
        {
            AnsiConsole.MarkupLine($"[red]✗ Error: {result.ErrorMessage}[/]");
            return 1;
        }

        if (result.IsEmpty)
        {
            DisplayEmptyBookshelfMessage();
            return 0;
        }

        DisplayBookList(result, settings.ShowDetails);
        return 0;
    }

    /// <summary>
    /// Displays a message when the bookshelf is empty
    /// </summary>
    private static void DisplayEmptyBookshelfMessage()
    {
        var emptyPanel = new Panel(
            "[yellow]📚 Your bookshelf is empty![/]\n\n" +
            "[grey]To add books to your bookshelf, you can:[/]\n" +
            "  [cyan]•[/] Copy PDF files directly to this directory\n" +
            "  [cyan]•[/] Use the [green]consolidate[/] command to organize books from another location")
            .Border(BoxBorder.Rounded)
            .BorderColor(Color.Yellow);

        AnsiConsole.Write(emptyPanel);
    }

    /// <summary>
    /// Displays the list of books in the bookshelf
    /// </summary>
    private static void DisplayBookList(BookListResult result, bool showDetails)
    {
        if (showDetails)
        {
            DisplayDetailedTable(result);
        }
        else
        {
            DisplaySimpleList(result);
        }

        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine($"[green]Total books: {result.TotalBooks}[/]");
    }

    /// <summary>
    /// Displays a simple list of book titles
    /// </summary>
    private static void DisplaySimpleList(BookListResult result)
    {
        foreach (var book in result.Books)
        {
            AnsiConsole.MarkupLine($"  [cyan]📖[/] {Markup.Escape(book.Title)}");
        }
    }

    /// <summary>
    /// Displays a detailed table with book information
    /// </summary>
    private static void DisplayDetailedTable(BookListResult result)
    {
        var table = new Table()
            .Border(TableBorder.Rounded)
            .BorderColor(Color.Blue)
            .AddColumn("[bold]Title[/]")
            .AddColumn("[bold]Size[/]")
            .AddColumn("[bold]Created[/]")
            .AddColumn("[bold]Pages[/]");

        foreach (var book in result.Books)
        {
            var pageCountStr = book.PageCount.HasValue ? book.PageCount.Value.ToString() : "-";
            table.AddRow(
                Markup.Escape(book.Title),
                book.FormattedFileSize,
                book.CreationDate.ToString("yyyy-MM-dd"),
                pageCountStr);
        }

        AnsiConsole.Write(table);
    }
}
