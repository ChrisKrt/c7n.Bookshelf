Feature: US0002 - Bookshelf List
  # User Story: US0002 - Bookshelf List
  # GitHub Issue: #31
  # As a book collector with an organized bookshelf
  # I want to view a list of all my books
  # So that I can see what's in my collection and access specific books

  @UserStory0002 @Issue31
  Scenario: Display basic list of books in bookshelf
    Given I have a bookshelf with multiple PDF files
    When I request to list all books
    Then I should see a list of all book titles
    And the list should be displayed in alphabetical order by default

  @UserStory0002 @Issue32
  Scenario: Show detailed book information in list
    Given I have a bookshelf with multiple PDF files
    When I request to list all books with details
    Then I should see book titles, file sizes, and creation dates
    And I should see the number of pages for each book
    And the information should be formatted in a readable table

  @UserStory0002 @Issue33
  Scenario: Filter list by book title
    Given I have a bookshelf with multiple books
    When I search for books containing "Python" in the title
    Then I should see only books with "Python" in their title
    And other books should be hidden from the results

  @UserStory0002 @Issue34
  Scenario: Sort list by different criteria
    Given I have a bookshelf with multiple books
    When I sort the list by file size
    Then books should be displayed in order from smallest to largest
    And I should be able to reverse the sort order

  @UserStory0002 @Issue35
  Scenario: Display empty bookshelf message
    Given I have an empty bookshelf directory
    When I request to list all books
    Then I should see a message indicating the bookshelf is empty
    And I should see instructions on how to add books
