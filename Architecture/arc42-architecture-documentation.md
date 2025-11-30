# ARC 42 Architecture Documentation

# Introduction and Goals

## Requirements Overview

## Quality Goals

## Stakeholders

# Architecture Constraints

# Context and Scope

## Business Context

## Technical Context

# Solution Strategy

# Building Block View

## Whitebox Overall System

### \<Name black box 1\>

### \<Name black box 2\>

### \<Name black box n\>

### \<Name interface 1\>

### \<Name interface m\>

## Level 2

### White Box *\<building block 1\>*

### White Box *\<building block 2\>*

### White Box *\<building block m\>*

## Level 3

### White Box \<\_building block x.1\_\>

### White Box \<\_building block x.2\_\>

### White Box \<\_building block y.1\_\>

# Runtime View

## \<Runtime Scenario 1\>

## \<Runtime Scenario 2\>

## \<Runtime Scenario n\>

# Deployment View

## Infrastructure Level 1

## Infrastructure Level 2

### *\<Infrastructure Element 1\>*

### *\<Infrastructure Element 2\>*

### *\<Infrastructure Element n\>*

# Cross-cutting Concepts

## *\<Concept 1\>*

## *\<Concept 2\>*

## *\<Concept n\>*

# Architecture Decisions

## 1 Programming Language
C# is the main programming language since clean code can be best achievd with it and the devlopers have experience with this programming language.

## 2 CLI Frontend
At first a CLI is the main development target as UI. It is simple and AI Agents cann execute CLI commands.

# Quality Requirements

## Quality Requirements Overview

## Quality Scenarios

# Risks and Technical Debts

# Glossary

# US0001 - Bookshelf Consolidation Feature

## Overview

The Bookshelf Consolidation feature implements US0001, enabling users to consolidate scattered PDF files from multiple folders into a single organized bookshelf location.

## Architecture

The implementation follows Clean Architecture principles with clear separation of concerns:

### Domain Layer (Application/Core)

**Value Objects:**
- `BookPath`: Represents a validated file path with convenience methods
- `BookMetadata`: Encapsulates PDF metadata (title, author, creation date)

**Entities:**
- `Book`: Represents a book with path and metadata
- `ConsolidationResult`: Contains the results of a consolidation operation

### Application Layer

**API (Public Interface):**
- `IBookshelfConsolidationService`: Service contract for consolidation operations

**Services:**
- `BookshelfConsolidationService`: Implements consolidation logic, orchestrates file operations and PDF merging

**SPI (Service Provider Interface):**
- `IPdfMerger`: Contract for PDF merging operations
- `IFileSystemAdapter`: Contract for file system operations

### Infrastructure Layer

**Adapters:**
- `PdfMerger`: Implements PDF merging using PdfSharp library
- `FileSystemAdapter`: Implements file system operations

### Presentation Layer (CLI)

**Commands:**
- `ConsolidateCommand`: Spectre.Console CLI command with progress reporting

## Design Decisions

### PDF Library Choice

**Decision:** Use PdfSharp instead of iText7
**Rationale:** 
- PdfSharp is MIT licensed (no commercial restrictions)
- iText7 9.0.0 requires commercial license
- PdfSharp provides sufficient functionality for merging operations

### Naming Conflict Resolution

**Decision:** Append numeric suffixes (_1, _2, etc.) to conflicting filenames
**Rationale:**
- Simple and predictable
- Preserves all content
- No data loss
- User can identify conflicts from output

### Flat Structure

**Decision:** Target bookshelf has no subdirectories
**Rationale:**
- Simplifies book discovery
- Aligns with user story "access from one location"
- Collections are merged into single files anyway

## Dependencies

- **PdfSharp 6.2.2**: PDF manipulation
- **Spectre.Console.Cli 0.53.0**: CLI framework with progress reporting
- **Serilog**: Structured logging
- **Microsoft.Extensions.DependencyInjection**: Dependency injection

## Component Interactions

```
User → ConsolidateCommand (CLI)
       ↓
       BookshelfConsolidationService (Application)
       ↓                              ↓
       FileSystemAdapter              PdfMerger
       (Infrastructure)               (Infrastructure)
```

## Testing Strategy

The feature is tested using Behavior-Driven Development (BDD):

- **Framework:** Behave (Python)
- **Test File:** `Application/tests/e2e/features/US0001-Bookshelf-Consolidation.feature`
- **Scenarios:** 6 acceptance criteria scenarios
  1. Consolidate scattered individual PDFs
  2. Merge PDF collections
  3. Handle mixed content
  4. Preserve metadata
  5. Handle naming conflicts
  6. Provide progress feedback

All scenarios verified and passing.

## Extension Points

Future enhancements could include:
- Custom naming strategies for conflicts
- Metadata enrichment
- Directory structure preservation option
- Incremental consolidation (skip already processed files)
- Duplicate detection based on content hash

# US0002 - Bookshelf List Feature

## Overview

The Bookshelf List feature implements US0002, enabling users to view all books in their bookshelf with filtering, sorting, and detailed information display options.

## Architecture

The implementation follows Clean Architecture principles with clear separation of concerns:

### Domain Layer (Application/Core)

**Entities:**
- `BookListResult`: Contains the results of a list operation with success status and book collection

### Application Layer

**API (Public Interface):**
- `IBookshelfListService`: Service contract for listing books operations
- `ListBooksRequest`: Request DTO with filtering and sorting options
- `BookInfo`: DTO representing book information (title, size, date, pages)

**Services:**
- `BookshelfListService`: Implements listing logic with filtering, sorting, and page count retrieval

**SPI Extensions:**
- `IFileSystemAdapter.GetFileInfoAsync`: Retrieves file metadata (size, creation date)
- `IPdfMerger.GetPageCountAsync`: Extracts page count from PDF files

### Infrastructure Layer

**Adapter Extensions:**
- `FileSystemAdapter.GetFileInfoAsync`: Implements file info retrieval using System.IO.FileInfo
- `PdfMerger.GetPageCountAsync`: Implements page count extraction using PdfSharp

### Presentation Layer (CLI)

**Commands:**
- `ListCommand`: Spectre.Console CLI command with table and list display modes
- `ListSettings`: Command settings with filter, sort, details, and reverse options

## Design Decisions

### Sorting Behavior

**Decision:** Default alphabetical sorting by title, with options for size, date, and pages
**Rationale:**
- Alphabetical is most intuitive for browsing
- Multiple sort options accommodate different use cases
- Null page counts sorted to end for consistent behavior

### Filtering

**Decision:** Case-insensitive partial matching on book titles
**Rationale:**
- Simple and intuitive user experience
- Covers most common search use cases
- Fast in-memory filtering

### Display Modes

**Decision:** Two display modes - simple list and detailed table
**Rationale:**
- Simple list for quick browsing
- Detailed table for comparison and analysis
- Spectre.Console provides rich table formatting

### Page Count Handling

**Decision:** Page count extraction is optional and failure-tolerant
**Rationale:**
- Not all PDFs are readable (corrupted, encrypted)
- Operation should not fail due to single file issues
- Display "-" for unknown page counts

## Component Interactions

```
User → ListCommand (CLI)
       ↓
       BookshelfListService (Application)
       ↓                        ↓
       FileSystemAdapter        PdfMerger
       (Get file info)          (Get page count)
       ↓
       BookListResult → Display (Table/List)
```

## Request/Response Flow

1. User invokes `bookshelf list <directory> [options]`
2. `ListCommand` validates settings and creates `ListBooksRequest`
3. `BookshelfListService` processes request:
   - Gets PDF files from directory via `IFileSystemAdapter`
   - Retrieves file info (size, date) for each file
   - Optionally retrieves page counts via `IPdfMerger`
   - Applies title filter if specified
   - Applies sorting based on sort field and direction
4. Returns `BookListResult` with filtered and sorted books
5. `ListCommand` displays results as list or table based on options

## Testing Strategy

The feature is tested using Behavior-Driven Development (BDD):

- **Framework:** Behave (Python)
- **Test File:** `Application/tests/e2e/features/US0002-Bookshelf-List.feature`
- **Scenarios:** 5 acceptance criteria scenarios
  1. Display basic list of books in bookshelf
  2. Show detailed book information in list
  3. Filter list by book title
  4. Sort list by different criteria
  5. Display empty bookshelf message

## Dependencies

- **PdfSharp 6.2.2**: PDF page count extraction
- **Spectre.Console.Cli 0.53.0**: Table formatting and CLI options
- **Serilog**: Structured logging
- **Microsoft.Extensions.DependencyInjection**: Dependency injection

## Extension Points

Future enhancements could include:
- Additional filter criteria (date range, size range)
- Multiple sort fields
- Export list to file (CSV, JSON)
- Search within PDF content
- Tag-based organization
