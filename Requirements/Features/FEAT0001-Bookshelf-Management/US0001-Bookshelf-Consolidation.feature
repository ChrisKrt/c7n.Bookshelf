Feature: US0001 - Bookshelf Consolidation
  # User Story: US0001 - Bookshelf Consolidation
  # GitHub Issue: #13
  # As a book collector with scattered PDF files across multiple folders
  # I want to consolidate all my books into a single organized bookshelf
  # So that I can easily find and access my entire collection from one location

  @Issue14
  Scenario: Consolidate scattered individual PDF files into bookshelf
    Given I have multiple PDF files scattered across different folders
    And I have specified a source directory containing these files
    And I have specified a target bookshelf directory
    When I run the consolidation command
    Then all individual PDF files should be copied to the bookshelf directory
    And the bookshelf should contain a flat structure of PDF files
    And the original files should remain unchanged in their source locations

  @Issue15
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

  @Issue16
  Scenario: Handle mixed content with both individual PDFs and collections
    Given I have a source directory with both individual PDF files and collection folders
    And some folders contain single PDFs while others contain multiple PDFs
    When I run the consolidation command
    Then individual PDF files should be copied as-is to the bookshelf
    And folders with multiple PDFs should be merged into single PDF files
    And the bookshelf should contain only individual PDF files with no subfolders
    And all books should be accessible from a single location

  @Issue17
  Scenario: Preserve book metadata during consolidation
    Given I have PDF files with existing metadata (title, author, creation date)
    And some files are in collections that need merging
    When I run the consolidation command
    Then individual PDFs should retain their original metadata
    And merged PDFs should preserve metadata from the first file in the collection
    And file creation timestamps should be maintained where possible

  @Issue18
  Scenario: Handle naming conflicts during consolidation
    Given I have multiple PDF files or collections with identical names
    And these files exist in different source folders
    When I run the consolidation command
    Then the system should detect naming conflicts
    And the system should apply a unique naming strategy (e.g., append counter or hash)
    And no files should be overwritten without user awareness
    And all original content should be preserved in the consolidated bookshelf

  @Issue19
  Scenario: Provide progress feedback during consolidation
    Given I have a large collection of PDFs to consolidate
    When I run the consolidation command
    Then the CLI should display progress information
    And the CLI should show which files are being processed
    And the CLI should indicate when merging operations are occurring
    And the CLI should report the total number of books consolidated
    And the CLI should notify upon successful completion

  @Issue25
  Scenario: Handle mitp publisher naming pattern for chapter ordering
    Given I have a PDF collection from mitp publisher
    And the collection contains files with patterns like "Cover", "Titel", "Inhaltsverzeichnis", "Einleitung"
    And the collection contains files with patterns like "Kapitel_1_", "Kapitel_2_", "Kapitel_10_", "Kapitel_11_"
    And the collection contains files with patterns like "Anhang_A_", "Anhang_B_"
    And the collection contains files with patterns like "Glossar", "Stichwortverzeichnis"
    And the collection may contain duplicate files with "(1)" suffix
    When I run the consolidation command
    Then the system should detect the mitp naming pattern plugin
    And front matter files (Cover, Titel, Inhaltsverzeichnis, Einleitung) should be placed first
    And chapters should be ordered numerically (Kapitel_1, Kapitel_2, ..., Kapitel_10, Kapitel_11)
    And appendices should be ordered alphabetically after chapters (Anhang_A, Anhang_B)
    And back matter files (Glossar, Stichwortverzeichnis) should be placed at the end
    And duplicate files with "(1)" suffix should be ignored
    And the merged PDF should maintain the correct logical reading order

  @Issue26
  Scenario: Handle Wichmann Verlag naming pattern for chapter ordering
    Given I have a PDF collection from Wichmann Verlag
    And the collection contains files with patterns like "Vorwort", "Inhalt"
    And the collection contains files with patterns like "_1_", "_2_", "_3_", "_4_", "_5_", "_6_", "_7_", "_8_"
    And the collection contains files with patterns like "Anhnge", "Stichwortverzeichnis"
    When I run the consolidation command
    Then the system should detect the Wichmann Verlag naming pattern plugin
    And front matter files (Vorwort, Inhalt) should be placed first
    And chapters should be ordered numerically by the number in the pattern (_1_, _2_, ..., _8_)
    And appendices (Anhnge) should be placed after chapters
    And back matter files (Stichwortverzeichnis) should be placed at the end
    And the merged PDF should maintain the correct logical reading order

  @Issue27
  Scenario: Plugin architecture supports multiple publisher patterns
    Given I have PDF collections from different publishers
    And the system uses a plugin architecture for naming pattern recognition
    And I have a collection from mitp publisher with German naming conventions
    And I have a collection from Wichmann Verlag with underscore-based numbering
    When I run the consolidation command
    Then the system should detect the appropriate naming pattern plugin for each collection
    And each collection should be merged according to its publisher's pattern rules
    And all merged PDFs should maintain their respective logical reading orders

  @Issue28
  Scenario: Handle Hanser Verlag ISBN-based naming pattern
    Given I have a PDF collection from Hanser Verlag
    And the collection contains files with ISBN pattern "9783446######.fm.pdf"
    And the collection contains files with ISBN pattern "9783446######.001.pdf", "9783446######.002.pdf"
    And the collection contains files with ISBN pattern "9783446######.bm.pdf"
    When I run the consolidation command
    Then the system should detect the Hanser Verlag naming pattern plugin
    And front matter file (.fm.pdf) should be placed first
    And chapters should be ordered numerically (.001, .002, ..., .010, .011)
    And back matter file (.bm.pdf) should be placed at the end
    And the merged PDF should maintain the correct logical reading order

  @Issue29
  Scenario: Handle O'Reilly English naming pattern
    Given I have a PDF collection from O'Reilly publisher
    And the collection contains files with patterns like "BEGINN", "Inhalt", "Vorwort"
    And the collection contains files with pattern "Kapitel_1_", "Kapitel_2_", "Chapter_1_"
    And the collection contains files with patterns like "Index", "Anhang"
    When I run the consolidation command
    Then the system should detect the O'Reilly naming pattern plugin
    And front matter files (BEGINN, Inhalt, Vorwort) should be placed first
    And chapters should be ordered numerically
    And appendices (Anhang) should be placed after chapters
    And back matter files (Index) should be placed at the end
    And the merged PDF should maintain the correct logical reading order

  @Issue30
  Scenario: Handle Teil-based (Part-based) structure with chapters
    Given I have a PDF collection with German Teil (Part) structure
    And the collection contains files with pattern "Teil_I_", "Teil_II_", "Teil_III_"
    And each Teil may contain multiple chapters with pattern "Kapitel_1_", "Kapitel_2_"
    And the collection contains front matter like "BEGINN", "Vorwort", "Inhaltsverzeichnis"
    And the collection contains back matter like "Index", "Anhang"
    When I run the consolidation command
    Then the system should detect the Teil-based naming pattern plugin
    And front matter should be placed first
    And Teile should be ordered numerically (Teil_I, Teil_II, Teil_III)
    And chapters within each Teil should maintain their order
    And back matter should be placed at the end
    And the merged PDF should maintain the correct logical reading order
