# Copilot Instructions for c7n.Bookshelf

## Project Overview

c7n.Bookshelf is a bookshelf management application built using C# and .NET, following Clean Architecture and Domain-Driven Design principles. The project uses trunk-based development and behavior-driven development (BDD) for testing.

## Development Philosophy

This project follows these core principles:
- **Clean Architecture**: Separation of concerns with dependency flow pointing inward
- **Domain-Driven Design (DDD)**: Domain logic encapsulated in domain entities
- **Test-Driven Development (TDD)**: Write tests before implementing features
- **Behavior-Driven Development (BDD)**: Use Gherkin syntax for acceptance criteria
- **SOLID Principles**: Applied throughout the codebase
- **12-Factor App**: Configuration via environment variables

## Workflow Instructions

- We use trunk based development
- Every feature is developed in a feature branch
- The feature branch is created from the main branch
- The feature branch is named `feature/#<feature-ticket-number>/#<user-story-ticket-number>-<short-description>` (e.g. `feature/#123/#456-add-login`)
- The feature branch is pushed to the remote repository
- A pull request is created from the feature branch to the main branch (as early as possible)
- When you create a ToDo list, add the list to the pull request description

- We use behavior driven development (BDD) to define the behavior of the system
- add a screenshort to the pull request, if the feature has a UI
- We use commitizen <a>.cz.toml</a> to automatically create a CHANGELOG.md
- We use `cz commit` to create commit messages

## Project Structure

### Code Structure
The code resides in the Application folder:
```
Application/
├── Bookshelf.Cli/          # CLI frontend
├── Bookshelf.Application/  # Application layer (API, SPI, Core domain)
├── Bookshelf.Infrastructure/ # Infrastructure implementations
```

### Architecture
- Clean Architecture with API/SPI separation
- Frontend code only accesses the API module of the application project
- Infrastructure implements the SPI (Service Provider Interface)
- Application project is independent of frontend and infrastructure
- Cross-cutting concerns (logging, caching, auth) are in an aspects module
- Dependency injection using .NET's built-in DI container

### Documentation Structure
```
Architecture/               # arc42 architecture documentation
├── arc42-architecture-documentation.md

Requirements/              # Product requirements (req42 format)
├── req42-product-requirments-document.md
├── Features/
│   └── FEAT####-Feature-Name/
│       └── US####-User-Story-Name.feature

Guides/                    # User and admin guides
├── User.md
├── Admin.md

DesignSystem/              # Design system documentation
```

## General Development Standards

- Every public method or class should have a comment
- Use descriptive variable and function names (domain-related)
- Do not use primitive obsession; always use objects to encapsulate related data
- Avoid magic numbers; use constants instead
- Use async/await for asynchronous operations
- Use Guard Clauses for public methods to handle invalid input early
- Use early returns to reduce nesting
- For private methods, use debug.assert (C#) or console.assert (JavaScript) for preconditions and postconditions
- Keep methods/functions flat with low cyclomatic complexity
- Private methods should be static methods
- Write unit tests for public methods
- Always use HTTPS/TLS for communication in web applications

## C# Specific Guidelines

When working with C# files (`**/*.{cs,csproj,sln,slnx}`):
- Frontend code should be in its own project (a web API is also a frontend)
- Application code should be in its own project
- Domain code should be in a "Core" module inside the application project
- Infrastructure code should be in its own project
- Aspects code (cross-cutting concerns) should be in its own project
- The API of application services should be in an "api" module inside the application project
- The SPI (Service Provider Interface) should be in an "spi" module inside the application project
- Use LINQ whenever possible
- Pass an ILogger to the constructor of classes that need logging
- Use dependency injection
- Each module adds its own services to the DI container
- The composition root is in the frontend project

See [C# Instructions](instructions/csharp.instructions.md) for detailed guidelines.

## Docker Guidelines

When working with Docker files (`docker-compose.yml`, `*.docker-compose.yml`, `Dockerfile`):
- Use docker-compose
- Images are self-contained applications, not simple microservices
- Use domain naming for services/containers, images, volumes, and networks
- Use multistage builds to reduce image size
- Always use HTTPS/TLS for communication between services
- Never use latest tags; always use specific version tags

See [Docker Instructions](instructions/docker.instructions.md) for detailed guidelines.

## Documentation Guidelines

When working with documentation files (`Architecture/*.md`, `Guides/*.md`, `DesignSystem/*.md`):
- Use plain, jargon-free language
- Write in active voice and present tense
- Use descriptive, meaningful link text
- Include accessibility considerations
- Document theme support (light/dark) and responsive behavior
- Use markdown formatting consistently
- Include visual examples or screenshots for UI elements

See [Documentation Guidelines](instructions/documentation-guidelines.instructions.md) for comprehensive writing standards.

## Design System Guidelines

When documenting UI components or design elements:
- Include accessibility considerations and axe-core testing requirements
- Document theme support (light/dark) and responsive behavior
- Provide code examples using Shoelace components and design tokens
- Include visual examples or screenshots when documenting UI elements

See [Design System Instructions](instructions/design-system.instructions.md) for details.

## Requirements and Testing

### Feature Hierarchy
Requirements follow an Agile hierarchy:
- **Features** (FEAT####) - High-level capabilities spanning multiple sprints
- **User Stories** (US####) - Specific deliverables completable in one sprint
- **Scenarios** - Individual acceptance criteria for each user story

### User Stories
- User stories are written in Gherkin syntax as `.feature` files
- Each user story file contains multiple scenarios (acceptance criteria)
- Place user story files in their parent feature directory
- Each feature should have a `@UserStory####` tag
- Each scenario should have a `@Issue###` tag for the GitHub issue number
- Scenarios ARE the acceptance criteria and are tested with Playwright and Cucumber

Example structure:
```gherkin
@UserStory0001
Feature: US0001 - User Authentication
  # User Story: US0001 - User Authentication
  # As a user
  # I want to log in securely
  # So that I can access my personal data

  @Issue123
  Scenario: Successful login
    Given I am on the login page
    When I enter my valid credentials
    Then I should be redirected to the dashboard
```

### Testing Guidelines
- Use Gherkin for acceptance criteria tests
- Write Gherkin steps in a step file (gather all available steps in a single file)
- Write Gherkin thens in a then file
- Write Gherkin givens in a given file
- Write Gherkin test scenarios in a scenario file
- Scenarios are tested with Playwright and Cucumber

## Third-Party Libraries

- Usage of third-party libraries happens in a separate file
- Try to restrict the usage of a third-party library to a single file
- Every usage of a third-party library is wrapped in a try-catch block

## Configuration and Deployment

- Configuration happens via environment variables
- Follow the 12-factor app principles
- Use the commitizen command line tool to create commit messages (e.g., `cz commit`)

## Additional Instructions

For more detailed, context-specific instructions, refer to:
- [Project Workflow](instructions/project-workflow.instructions.md)
- [Development Standards](instructions/development-standards.instructions.md)
- [C# Instructions](instructions/csharp.instructions.md)
- [Docker Instructions](instructions/docker.instructions.md)
- [Documentation Guidelines](instructions/documentation-guidelines.instructions.md)
- [Design System Instructions](instructions/design-system.instructions.md)

## Key Documentation

- [Architecture Documentation](../Architecture/arc42-architecture-documentation.md)
- [Product Requirements Document](../Requirements/req42-product-requirments-document.md)
- [User Guide](../Guides/User.md)
- [Admin Guide](../Guides/Admin.md)
- [Contributing Guide](../CONTRIBUTING.md)
- [Security Policy](../SECURITY.md)
