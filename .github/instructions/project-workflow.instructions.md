- We use specification driven development (SDD) with the usage of GitHub SpecKit (https://github.com/github/spec-kit).
- You find everything necessary in .specify
- When implementing a feature or userstory follow .github\prompts\implement.prompt.md (REQUIRED!)
- After implementing you should update the documentation. Please follow .github\prompts\update-docs.prompt.md (REQUIRED!)


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
The Product Requirement Document is based on [req42](https://req42.de/req42-im-ueberblick)
The Product Requirments Document is [Product Requirments Document](../../Requirements/req42-product-requirments-document.md).
The features reside in the [Features](../../Requirements/Features/) folder.
A feature is a collection of user stories that belong together.
The user stories are acceptance criteria for the feature.
If you create a new feature, please document it in the features folder.
Every feature is a .feature file.
The user stories are written in Gherkin syntax.
Every user story is a scenario in a feature file.

Example:

```gherkin
Feature: User authentication

  Scenario: Successful login
    Given I am on the login page
    When I enter my valid credentials
    Then I should be redirected to the dashboard

  Scenario: Failed login
    Given I am on the login page
    When I enter my invalid credentials
    Then I should see an error message
```

Every user story (scenario) should have a tag with the ticket number, e.g. `@TICKET-123`.
The scenarios are tested with Playwright and Cucumber.

## Design System
The design system resides in the [DesignSystem](../../DesignSystem) folder.
If you create a new component, please document it in the design system.
The accessibility of the components should be tested with axe-core.
