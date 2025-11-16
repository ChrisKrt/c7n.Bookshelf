# Workflow Instructions
- We use trunk based development
- Every feature is developed in a feature branch
- The feature branch is created from the main branch
- The feature branch is named `feature/#<feature-ticket-number>/#<user-story-ticket-number>-<short-description>` (e.g. `feature/#123/#456-add-login`)
- The feature branch is pushed to the remote repository
- A pull request is created from the feature branch to the main branch (as early as possible)
- When you create a ToDo list, add the list to the pull request description

- We use behavior driven development (BDD) to define the behavior of the system
- add a screenshort to the pull request, if the feature has a UI
- We use commitizen [.cz.toml](.cz.toml) to automatically create a CHANGELOG.md
- We use `cz commit` to create commit messages

# Project Structure

## Code
The code resides in the Application folder.
If you create a new file or folder, please follow the existing structure.
```
Application/
├── Bookshelf.Cli/
├── Bookshelf.Application/
├── Bookshelf.Infrastructure/
```

## Architecture

The architecture documentation resides in the [Architecture Documentation](../../Architecture/arc42-architecture-documentation.md).
It folloed the arc42 template (similar to the C4 model).
Document any changes to the architecture in the architecture documentation.
```
Architecture/
├── arc42-architecture-documentation.md
```

## User Guide
The user guide resides in [Guides](../../Guides/User.md/).
If you create a new button or any user interaction, please document it in the user guide.
For UI elements add screenshots.

## Admin Guide
The guide for personell which administeres the software, resdies in [Guides](../../Guides/Admin.md).
Document installation requirements.
Document installation workflow.

## Requirements
The Product Requirements Document follows [req42](https://req42.de/req42-im-ueberblick).
See [Product Requirements Document](../../Requirements/req42-product-requirments-document.md) for details.

### Feature Hierarchy
Requirements follow an Agile hierarchy:
- **Features** (FEAT####) - High-level capabilities spanning multiple sprints
- **User Stories** (US####) - Specific deliverables completable in one sprint
- **Scenarios** - Individual acceptance criteria for each user story

### File Structure
```
Requirements/
  Features/
    FEAT####-Feature-Name/
      US####-User-Story-Name.feature
      US####-User-Story-Name.mockup.prompt (optional)
```

### Features
Features reside in the [Features](../../Requirements/Features/) folder.
Each feature has its own subdirectory (e.g., `FEAT0001-Bookshelf-Management/`).
The Features are also linked in the requirements document [req42-product-requirments-document.md](../../Requirements/req42-product-requirments-document.md).

### User Stories
User stories are written in Gherkin syntax as `.feature` files.
Each user story file contains multiple scenarios (acceptance criteria).
Place user story files in their parent feature directory.

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

  @Issue124
  Scenario: Failed login
    Given I am on the login page
    When I enter my invalid credentials
    Then I should see an error message
```

### Tagging Convention
- Each feature should have a `@UserStory####` tag matching the user story number
- Each scenario should have a `@Issue###` tag for the GitHub issue number
- Scenarios are tested with Playwright and Cucumber

### Key Principle
Scenarios ARE the acceptance criteria and are tested with Playwright and Cucumber.

## Design System
The design system resides in the [DesignSystem](../../DesignSystem) folder.
If you create a new component, please document it in the design system.
The accessibility of the components should be tested with axe-core.
