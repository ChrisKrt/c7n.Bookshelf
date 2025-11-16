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

