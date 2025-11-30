# User Guide - c7n.Bookshelf

## Overview

c7n.Bookshelf is a command-line tool for managing your PDF book collection. It helps you organize scattered PDF files from multiple folders into a single, well-organized bookshelf.

## Installation

### Prerequisites

- .NET 9.0 Runtime or SDK
- Linux, macOS, or Windows operating system

### Building from Source

1. Clone the repository
2. Navigate to the `Application` directory
3. Build the solution:
   ```bash
   dotnet build BookShelf.slnx
   ```
4. The executable will be in `Bookshelf.Cli/bin/Debug/net9.0/Bookshelf.Cli`

## Features

### Bookshelf Consolidation

The consolidation feature helps you gather all your PDF books from various locations into a single organized directory.

#### What It Does

- **Copies Individual PDFs**: Finds and copies standalone PDF files to your bookshelf
- **Merges PDF Collections**: Automatically combines multi-part PDFs (like book chapters) into single files
- **Handles Mixed Content**: Intelligently processes folders containing both single PDFs and collections
- **Preserves Metadata**: Retains book titles, authors, and other metadata during consolidation
- **Resolves Naming Conflicts**: Automatically renames files with duplicate names to prevent overwrites
- **Provides Progress Feedback**: Shows real-time progress as files are processed

## Tips and Best Practices

### Organizing Your Source Files

For best results:

1. **Group Related PDFs**: Place multi-part books in their own folders
2. **Use Descriptive Folder Names**: Folder names become the merged PDF filename
3. **Check Your Source**: Review source structure before consolidating

### Backup Your Files

Always keep backups of your original files. The consolidation process:
- Copies files (doesn't move them)
- Creates new merged PDFs
- Never modifies source files

### Bookshelf Listing

The listing feature allows you to view all books in your bookshelf with various display and filtering options.

#### What It Does

- **Lists All Books**: Displays all PDF files in your bookshelf directory
- **Alphabetical Ordering**: Books are sorted alphabetically by title by default
- **Detailed View**: Shows file sizes, creation dates, and page counts
- **Title Filtering**: Search for books containing specific text in their titles
- **Flexible Sorting**: Sort by title, size, date, or page count
- **Reversible Order**: Display in ascending or descending order

## Commands

### consolidate

Consolidates scattered PDF files from a source directory into a target bookshelf directory.

#### Syntax

```bash
bookshelf consolidate <SOURCE> <TARGET>
```

#### Arguments

- `<SOURCE>` - The source directory containing your scattered PDF files and collections
- `<TARGET>` - The target bookshelf directory where all books will be consolidated

#### Example Usage

**Basic Consolidation**

```bash
bookshelf consolidate ~/Documents/PDFs ~/Bookshelf
```

This command will:
1. Scan `~/Documents/PDFs` for PDF files
2. Copy individual PDFs directly to `~/Bookshelf`
3. Merge multi-file collections into single PDFs
4. Display progress and results

**Consolidate from Multiple Locations**

To consolidate from multiple source directories, run the command multiple times:

```bash
bookshelf consolidate ~/Downloads ~/Bookshelf
bookshelf consolidate ~/Desktop/Books ~/Bookshelf
bookshelf consolidate ~/Documents/Research ~/Bookshelf
```

### list

Lists all books in your bookshelf with optional filtering and sorting.

#### Syntax

```bash
bookshelf list <BOOKSHELF> [OPTIONS]
```

#### Arguments

- `<BOOKSHELF>` - The bookshelf directory containing your PDF files

#### Options

| Option | Description |
| ------ | ----------- |
| `-d, --details` | Show detailed information including file size, creation date, and page count |
| `-f, --filter <TEXT>` | Filter books by title (case-insensitive search) |
| `-s, --sort <FIELD>` | Sort by: `title`, `size`, `date`, or `pages` |
| `-r, --reverse` | Reverse the sort order (descending instead of ascending) |

#### Example Usage

**Basic List**

```bash
bookshelf list ~/Bookshelf
```

Displays all books alphabetically:
```
📖 Advanced Python
📖 Clean Code
📖 Design Patterns
📖 Java Intro

Total books: 4
```

**Detailed View**

```bash
bookshelf list ~/Bookshelf --details
```

Displays a formatted table with book information:
```
╭─────────────────┬────────┬────────────┬───────╮
│ Title           │ Size   │ Created    │ Pages │
├─────────────────┼────────┼────────────┼───────┤
│ Advanced Python │ 2.5 MB │ 2025-01-15 │ 342   │
│ Clean Code      │ 4.1 MB │ 2024-11-20 │ 464   │
│ Design Patterns │ 3.2 MB │ 2024-09-10 │ 395   │
│ Java Intro      │ 1.8 MB │ 2025-02-01 │ 256   │
╰─────────────────┴────────┴────────────┴───────╯

Total books: 4
```

**Filter by Title**

```bash
bookshelf list ~/Bookshelf --filter python
```

Shows only books containing "python" in their title:
```
📖 Advanced Python

Total books: 1
```

**Sort by File Size (Largest First)**

```bash
bookshelf list ~/Bookshelf --sort size --reverse
```

**Sort by Page Count**

```bash
bookshelf list ~/Bookshelf --details --sort pages
```

**Empty Bookshelf**

When the bookshelf is empty, helpful instructions are displayed:
```
╭─────────────────────────────────────────────────────────────────────────╮
│ 📚 Your bookshelf is empty!                                             │
│                                                                         │
│ To add books to your bookshelf, you can:                                │
│   • Copy PDF files directly to this directory                           │
│   • Use the consolidate command to organize books from another location │
╰─────────────────────────────────────────────────────────────────────────╯
```

## Tips and Best Practices

### Organizing Your Source Files

For best results:

1. **Group Related PDFs**: Place multi-part books in their own folders
2. **Use Descriptive Folder Names**: Folder names become the merged PDF filename
3. **Check Your Source**: Review source structure before consolidating

### Backup Your Files

Always keep backups of your original files. The consolidation process:
- Copies files (doesn't move them)
- Creates new merged PDFs
- Never modifies source files

## Getting Help

View available commands:
```bash
bookshelf --help
```

View command-specific help:
```bash
bookshelf consolidate --help
bookshelf list --help
```
