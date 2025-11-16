using System.ComponentModel;
using Bookshelf.Application.Api;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Bookshelf.Cli.Commands;

/// <summary>
/// Command settings for the consolidate command
/// </summary>
public sealed class ConsolidateSettings : CommandSettings
{
    /// <summary>
    /// Gets or sets the source directory containing PDF files to consolidate
    /// </summary>
    [CommandArgument(0, "<SOURCE>")]
    [Description("The source directory containing PDF files and collections to consolidate")]
    public string SourceDirectory { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the target bookshelf directory
    /// </summary>
    [CommandArgument(1, "<TARGET>")]
    [Description("The target bookshelf directory where consolidated books will be placed")]
    public string TargetDirectory { get; set; } = string.Empty;

    public override ValidationResult Validate()
    {
        if (string.IsNullOrWhiteSpace(SourceDirectory))
        {
            return ValidationResult.Error("Source directory is required");
        }

        if (string.IsNullOrWhiteSpace(TargetDirectory))
        {
            return ValidationResult.Error("Target directory is required");
        }

        if (!Directory.Exists(SourceDirectory))
        {
            return ValidationResult.Error($"Source directory does not exist: {SourceDirectory}");
        }

        return ValidationResult.Success();
    }
}

/// <summary>
/// Command for consolidating scattered PDF files into a single bookshelf
/// </summary>
public sealed class ConsolidateCommand : AsyncCommand<ConsolidateSettings>
{
    private readonly IBookshelfConsolidationService _consolidationService;

    /// <summary>
    /// Initializes a new instance of the ConsolidateCommand class
    /// </summary>
    /// <param name="consolidationService">The consolidation service</param>
    public ConsolidateCommand(IBookshelfConsolidationService consolidationService)
    {
        _consolidationService = consolidationService ?? throw new ArgumentNullException(nameof(consolidationService));
    }

    public override async Task<int> ExecuteAsync(CommandContext context, ConsolidateSettings settings, CancellationToken cancellationToken)
    {
        var panel = new Panel("[bold]Bookshelf Consolidation[/]")
            .Border(BoxBorder.Rounded)
            .BorderColor(Color.Blue);

        AnsiConsole.Write(panel);
        AnsiConsole.WriteLine();

        AnsiConsole.MarkupLine($"[grey]Source:[/] [cyan]{settings.SourceDirectory}[/]");
        AnsiConsole.MarkupLine($"[grey]Target:[/] [cyan]{settings.TargetDirectory}[/]");
        AnsiConsole.WriteLine();

        var result = await AnsiConsole.Progress()
            .AutoClear(false)
            .Columns(
                new TaskDescriptionColumn(),
                new ProgressBarColumn(),
                new SpinnerColumn())
            .StartAsync(async ctx =>
            {
                var task = ctx.AddTask("[green]Consolidating books...[/]");
                task.IsIndeterminate = true;

                var progressReporter = new Progress<string>(message =>
                {
                    task.Description = $"[green]{message}[/]";
                });

                return await _consolidationService.ConsolidateAsync(
                    settings.SourceDirectory,
                    settings.TargetDirectory,
                    progressReporter,
                    cancellationToken);
            });

        AnsiConsole.WriteLine();

        if (result.Success)
        {
            var table = new Table()
                .Border(TableBorder.Rounded)
                .BorderColor(Color.Green)
                .AddColumn("[bold]Metric[/]")
                .AddColumn("[bold]Count[/]");

            table.AddRow("Total Books Processed", result.TotalBooksProcessed.ToString());
            table.AddRow("Individual PDFs Copied", result.IndividualPdfsCopied.ToString());
            table.AddRow("Collections Merged", result.CollectionsMerged.ToString());
            table.AddRow("Naming Conflicts Resolved", result.NamingConflicts.Count.ToString());

            AnsiConsole.Write(table);
            AnsiConsole.WriteLine();

            if (result.NamingConflicts.Any())
            {
                AnsiConsole.MarkupLine("[yellow]⚠ Naming conflicts were detected and resolved:[/]");
                foreach (var conflict in result.NamingConflicts)
                {
                    AnsiConsole.MarkupLine($"  [grey]•[/] [yellow]{conflict}[/]");
                }
                AnsiConsole.WriteLine();
            }

            AnsiConsole.MarkupLine("[green]✓ Consolidation completed successfully![/]");
            return 0;
        }
        else
        {
            AnsiConsole.MarkupLine($"[red]✗ Consolidation failed: {result.ErrorMessage}[/]");
            return 1;
        }
    }
}
