Feature: US0003 - Bookshelf Reordering
  # User Story: US0003 - Bookshelf Reordering
  # As a book collector who wants to organize my collection
  # I want to reorder and reorganize my books
  # So that I can maintain a logical structure that suits my needs

  Scenario: Move book to custom position in bookshelf
    Given I have a bookshelf with multiple books in alphabetical order
    When I move the book "Advanced Python" to position 5
    Then the book should appear at the 5th position in the list
    And other books should be shifted accordingly

  Scenario: Rename book file
    Given I have a book named "oldname.pdf" in my bookshelf
    When I rename it to "newname.pdf"
    Then the file should be renamed in the filesystem
    And the book should appear with the new name in the list

  Scenario: Create custom collections or categories
    Given I have multiple books in my bookshelf
    When I create a category called "Programming"
    And I assign selected books to this category
    Then I should be able to filter books by the "Programming" category
    And books can belong to multiple categories

  Scenario: Sort bookshelf by custom criteria
    Given I have a bookshelf with multiple books
    When I choose to sort by custom order
    Then I should be able to drag and drop books to reorder them
    And the custom order should be persisted for future sessions
