Feature: US0005 - Bibtex Files
  # User Story: US0005 - Bibtex File Management
  # As a researcher managing a large number of academic papers
  # I want to organize and maintain my Bibtex files effectively
  # So that I can easily reference and cite my sources in my work

  Background:
    Given the bookshelf system is initialized
    And I have several academic papers in my collection

  Scenario: Create Bibtex file for a paper
    Given I have a PDF paper on machine learning
    When I create a Bibtex entry for this paper
    Then a Bibtex file should be created next to the PDF
    And the Bibtex file should contain the citation information
    And the Bibtex file should be automatically linked to the PDF

  Scenario: Import existing Bibtex file for a paper
    Given I have a PDF paper on neural networks
    And I have an existing Bibtex file from another source
    When I import the Bibtex file for this paper
    Then the Bibtex file should be placed next to the PDF
    And the PDF and Bibtex file should be linked

  Scenario: Automatically detect existing Bibtex file
    Given I have a PDF paper with an accompanying Bibtex file
    When I add the paper to my bookshelf
    Then the system should automatically detect the Bibtex file
    And the citation information should be available

  Scenario: Edit Bibtex citation information
    Given I have a paper with an associated Bibtex file
    When I edit the Bibtex file to update the author information
    Then the changes should be saved
    And the updated citation should be reflected in the bookshelf

  Scenario: Move paper with its Bibtex file
    Given I have a paper with an associated Bibtex file
    When I move the paper to a different location
    Then the Bibtex file should be moved together with the paper
    And both files should remain linked

  Scenario: Delete paper with option to preserve Bibtex
    Given I have a paper with an associated Bibtex file
    When I delete the paper
    Then the system should ask whether to keep or delete the Bibtex file
    And I should be able to preserve the Bibtex file if desired

  Scenario: Create shared Bibtex file for related papers
    Given I have multiple parts of a multi-part paper series
    When I create a shared Bibtex file for these papers
    Then all papers should reference the same Bibtex file
    And the Bibtex file should be accessible from any of the papers

  Scenario: Export Bibtex entries for selected papers
    Given I have multiple papers with associated Bibtex files
    When I select several papers
    And I export their Bibtex entries
    Then a combined Bibtex file should be created
    And it should contain all selected citation entries

  Scenario: Validate Bibtex file format
    Given I have a paper with an associated Bibtex file
    When I validate the Bibtex file
    Then the system should check for required fields like title, author, and year
    And report any syntax errors or missing information

  Scenario: Sync Bibtex with PDF metadata
    Given I have a paper with embedded metadata
    And the paper has an associated Bibtex file
    When I synchronize the metadata
    Then the Bibtex file should be updated with information from the PDF
    And any conflicts should be reported for manual resolution
