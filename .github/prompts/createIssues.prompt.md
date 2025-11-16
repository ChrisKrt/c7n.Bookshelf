# Create GitHub Issues from Feature Documentation

## Objective
Automatically create a hierarchical issue structure in GitHub for all documented features, user stories, and test scenarios, ensuring proper linking and traceability between requirements and implementation.

## Input Arguments

### Required
- `repository`: ChrisKrt/c7n.Bookshelf
- `assignee`: Copilot
- `source_dir`: Requirements/Features/
- `requirements_doc`: Requirements/req42-product-requirments-document.md

### Templates Location
- `.github/ISSUE_TEMPLATE/feature.md` - For FEAT#### issues
- `.github/ISSUE_TEMPLATE/user-story.md` - For US#### issues
- `.github/ISSUE_TEMPLATE/test-case.md` - For scenario/test case issues

## Process Steps

### Step 1: Analyze Requirements Structure
1. Read `Requirements/req42-product-requirments-document.md` to understand feature hierarchy
2. Identify all features (FEAT####) in the Product Backlog section
3. For each feature, extract:
   - Feature ID and name
   - Feature description
   - List of associated user stories (US####)

### Step 2: Process Feature Files
For each feature directory in `Requirements/Features/FEAT####-Feature-Name/`:
1. Read all `.feature` files (Gherkin format)
2. Extract from each user story file:
   - User story ID (US####)
   - User story title and description (from comments)
   - User role, goal, and benefit (from "As a... I want... So that..." format)
   - All scenarios with their descriptions
   - Tags (@UserStory####, @Issue###)

### Step 3: Create Feature Issues
For each feature (FEAT####):
1. Create GitHub issue using `feature.md` template
2. Set title: `[FEATURE] FEAT#### - <Feature Name>`
3. Set labels: `['feature']`
4. Set assignee: `Copilot`
5. Fill template fields:
   - **Feature Description**: Extract from PRD
   - **User Stories**: Leave as checklist (will be filled in Step 5)
   - **Acceptance Criteria**: All user stories must be completed
   - **Dependencies**: Extract from feature context
6. Capture created issue number (#FEAT_ISSUE)

### Step 4: Create User Story Issues
For each user story (US####) within a feature:
1. Create GitHub issue using `user-story.md` template
2. Set title: `[USER STORY] US#### - <Story Title>`
3. Set labels: `['user-story']`
4. Set assignee: `Copilot`
5. Fill template fields:
   - **User Story**: "As a [role] I want [goal] So that [benefit]" from .feature comments
   - **Related Feature**: Link to parent feature issue (#FEAT_ISSUE)
   - **Acceptance Criteria**: List all scenarios from the .feature file
   - **Sub Tasks**: Reference test case issues (will be created in Step 5)
   - **Definition of Done**: Use defaults from template
6. Capture created issue number (#US_ISSUE)

### Step 5: Create Test Case Issues
For each Scenario in each .feature file:
1. Create GitHub issue using `test-case.md` template
2. Set title: `[TEST] <Scenario Description>`
3. Set labels: `['test-case']`
4. Set assignee: `Copilot`
5. Fill template fields:
   - **Test ID**: Generate as TC-US####-##
   - **Priority**: Medium (default)
   - **Test Type**: E2E/Acceptance
   - **Related User Story/Feature**: Link to parent user story (#US_ISSUE)
   - **Gherkin Scenario**: Copy complete scenario from .feature file
   - **Test Status**: [ ] Not Run
   - **Preconditions**: Extract from Given clauses
6. Capture created issue number (#TC_ISSUE)

### Step 6: Update Issue Cross-References
1. Update feature issues:
   - Fill "User Stories" checklist with created US issue numbers
2. Update user story issues:
   - Fill "Sub Tasks" or "Acceptance Criteria" with TC issue numbers
3. Add sub-issue relationships where supported by GitHub

### Step 7: Update Documentation
Update the following files with created issue numbers:
1. `Requirements/req42-product-requirments-document.md`:
   - Replace `[#TBD]` placeholders with actual feature issue numbers
   - Format: `[#<FEAT_ISSUE>](https://github.com/ChrisKrt/c7n.Bookshelf/issues/<FEAT_ISSUE>)`
2. Each `.feature` file:
   - Update `@Issue###` tags with actual test case issue numbers
   - Add issue references in feature comments

## Hierarchy and Linking Rules

### Issue Hierarchy
```
Feature Issue (FEAT####)
├── User Story Issue (US####)
│   ├── Test Case Issue (Scenario 1)
│   ├── Test Case Issue (Scenario 2)
│   └── Test Case Issue (Scenario N)
└── User Story Issue (US####)
    └── Test Case Issues...
```

### Link Types
- **Feature → User Stories**: Checklist items in "User Stories" section
- **User Story → Feature**: "Related Feature: #FEAT_ISSUE" reference
- **Test Case → User Story**: "Related to: #US_ISSUE" reference
- **User Story → Test Cases**: Checklist in "Acceptance Criteria" or "Sub Tasks"

## Validation Checklist

After completion, verify:
- [ ] All features from PRD have corresponding GitHub issues
- [ ] All user stories have corresponding GitHub issues
- [ ] All scenarios have corresponding test case issues
- [ ] All feature issues link to their user story issues
- [ ] All user story issues link back to their parent feature
- [ ] All test case issues link to their parent user story
- [ ] PRD document updated with actual issue numbers (no #TBD remaining)
- [ ] All .feature files updated with test case issue numbers
- [ ] All issues assigned to "Copilot"
- [ ] All issues have correct labels applied

## Expected Output

### Summary Report
```
Created Issues:
- Features: <count> (FEAT0001-FEAT000N)
- User Stories: <count> (US0001-US000N)  
- Test Cases: <count> (TC-US####-## format)

Updated Documents:
- Requirements/req42-product-requirments-document.md
- Requirements/Features/**/*.feature files

Issue Links:
- Feature→User Story links: <count>
- User Story→Test Case links: <count>
```

### Example Issue Structure
```
#1 [FEATURE] FEAT0001 - Bookshelf Management
  ├─ #2 [USER STORY] US0001 - Bookshelf Consolidation
  │   ├─ #3 [TEST] Consolidate scattered individual PDF files
  │   ├─ #4 [TEST] Merge PDF collections into single books
  │   ├─ #5 [TEST] Handle mixed content
  │   ├─ #6 [TEST] Preserve book metadata
  │   ├─ #7 [TEST] Handle naming conflicts
  │   └─ #8 [TEST] Provide progress feedback
  ├─ #9 [USER STORY] US0002 - Bookshelf List
  └─ #10 [USER STORY] US0003 - Bookshelf Reordering
```

## Error Handling

If errors occur during processing:
1. Log which step failed and why
2. List successfully created issues
3. Identify remaining items to process
4. Provide specific commands to resume or fix issues
5. Do not proceed if template files are missing or malformed

## Notes

- Maintain idempotency: Check if issues already exist before creating duplicates
- Preserve existing issue content if re-running the process
- Use consistent naming conventions across all issues
- Ensure all Markdown links are properly formatted
- Validate that all referenced files exist before processing