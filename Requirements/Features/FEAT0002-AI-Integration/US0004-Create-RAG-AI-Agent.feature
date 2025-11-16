Feature: US0004 - Create RAG AI Agent
  # User Story: US0004 - RAG AI Agent Integration
  # As a researcher with a large collection of academic papers
  # I want an AI agent that can answer questions based on my book collection
  # So that I can quickly find information without manually searching through documents

  Scenario: Initialize RAG system with bookshelf collection
    Given I have a bookshelf with multiple PDF documents
    When I initialize the RAG AI agent
    Then the system should index all PDF contents
    And the system should create vector embeddings for text chunks
    And the agent should be ready to answer questions

  Scenario: Query AI agent with simple question
    Given the RAG AI agent is initialized with my bookshelf
    When I ask "What is machine learning?"
    Then the agent should search relevant documents
    And the agent should provide an answer with citations
    And the agent should reference specific books and page numbers

  Scenario: Ask complex multi-document question
    Given the RAG AI agent is initialized with my bookshelf
    When I ask "Compare the approaches to neural networks in books A and B"
    Then the agent should retrieve information from multiple documents
    And the agent should synthesize a comparative answer
    And the agent should cite sources from both documents

  Scenario: Update RAG index when books are added
    Given the RAG AI agent is initialized with my bookshelf
    When I add a new book to the bookshelf
    Then the agent should automatically detect the new book
    And the agent should index the new content
    And the new book should be searchable in queries

  Scenario: Handle queries when no relevant information exists
    Given the RAG AI agent is initialized with my bookshelf
    When I ask a question unrelated to my collection
    Then the agent should indicate no relevant information was found
    And the agent should not hallucinate or provide unsourced answers

  Scenario: Provide context and source verification
    Given the RAG AI agent is initialized with my bookshelf
    When I ask a question and receive an answer
    Then I should be able to view the source excerpts used
    And I should see the book titles and page numbers
    And I should be able to open the referenced books directly
