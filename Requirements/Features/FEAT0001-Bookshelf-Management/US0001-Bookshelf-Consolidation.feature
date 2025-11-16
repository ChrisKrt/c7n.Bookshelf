Feature: US0001 - Bookshelf Consolidation
  # User Story: US0001 - Bookshelf Consolidation
  # As a book collector with scattered PDF files across multiple folders
  # I want to consolidate all my books into a single organized bookshelf
  # So that I can easily find and access my entire collection from one location

  Scenario: Consolidate scattered individual PDF files into bookshelf
    Given I have multiple PDF files scattered across different folders
    And I have specified a source directory containing these files
    And I have specified a target bookshelf directory
    When I run the consolidation command
    Then all individual PDF files should be copied to the bookshelf directory
    And the bookshelf should contain a flat structure of PDF files
    And the original files should remain unchanged in their source locations

  Scenario: Merge PDF collections into single books
    Given I have folders containing multiple PDF files representing book chapters
    And each folder represents a single book collection
    And I have specified a source directory containing these collections
    And I have specified a target bookshelf directory
    When I run the consolidation command
    Then each collection folder should be merged into a single PDF file
    And the merged PDF should be named after the collection folder
    And the merged PDF should contain all pages in the correct order
    And the single PDF should be placed in the bookshelf directory

  Scenario: Handle mixed content with both individual PDFs and collections
    Given I have a source directory with both individual PDF files and collection folders
    And some folders contain single PDFs while others contain multiple PDFs
    When I run the consolidation command
    Then individual PDF files should be copied as-is to the bookshelf
    And folders with multiple PDFs should be merged into single PDF files
    And the bookshelf should contain only individual PDF files with no subfolders
    And all books should be accessible from a single location

  Scenario: Preserve book metadata during consolidation
    Given I have PDF files with existing metadata (title, author, creation date)
    And some files are in collections that need merging
    When I run the consolidation command
    Then individual PDFs should retain their original metadata
    And merged PDFs should preserve metadata from the first file in the collection
    And file creation timestamps should be maintained where possible

  Scenario: Handle naming conflicts during consolidation
    Given I have multiple PDF files or collections with identical names
    And these files exist in different source folders
    When I run the consolidation command
    Then the system should detect naming conflicts
    And the system should apply a unique naming strategy (e.g., append counter or hash)
    And no files should be overwritten without user awareness
    And all original content should be preserved in the consolidated bookshelf

  Scenario: Provide progress feedback during consolidation
    Given I have a large collection of PDFs to consolidate
    When I run the consolidation command
    Then the CLI should display progress information
    And the CLI should show which files are being processed
    And the CLI should indicate when merging operations are occurring
    And the CLI should report the total number of books consolidated
    And the CLI should notify upon successful completion
