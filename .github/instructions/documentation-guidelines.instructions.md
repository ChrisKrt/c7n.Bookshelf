---
applyTo: "Architecture/*.md, Guides/*.md, DesignSystem/*.md"
---


# Documentation/Guidance Instructions
Apply these guidelines in markdown file to guide users on how to use the software. These guidelines are designed to ensure clarity, accessibility, and inclusivity in technical documentation.

# Tone Guidelines
- Use plain, jargon-free language.
- Write in active voice and present tense when possible.
- Use indicative and imperative moods; avoid subjunctive.
- Avoid anthropomorphisms (attributing human traits to objects).
- Use contractions like "isn't," "don't," and "can't" for a natural tone.
- Write precisely and clearly; place modifiers correctly.
- Avoid qualitative terms like "easy" or "simple," as these are subjective.

# Accessible Documentation Guidelines

## Writing Style
- Use plain language, short sentences, and simple paragraphs.
- Write inclusively and avoid biased or exclusive terms.
- Avoid jargon; define technical terms if necessary.
- Use lists (not paragraphs) for task instructions.

## Avoid Directional Language
- Avoid words like "above," "below," "left," or "right."
- Use time-based references such as "earlier" or "next."
- Link to relevant sections when needed.
- Refer to UI elements by label, not location.

## Use Multiple Identifiers
- Don’t rely on color alone to convey meaning.
- Use labels, shapes, patterns, and text alongside color.
- Fully describe visual elements for clarity.

## Introduce New Elements
- Lead into tables, lists, images, etc., with a full sentence.
- Set expectations for what users will see or do.

## Spell Out Words and Symbols
- Avoid symbols or shorthand; write words instead.
- Replace "&" with "and," "~" with "about," "+" with "plus."
- Use "then" for menu paths instead of angle brackets.
- Use hidden text (e.g., aria-label) for symbols in UI when needed.

## Precision in Language
- Clarify pronouns like "this" or "these" to avoid ambiguity.
- Ensure modifiers (“who,” “which,” “that”) refer to the correct noun.

# Structure and Formatting

## Headings
- Break content into headings for easier navigation.
- Use heading levels only down to H3; maintain hierarchy (no skipping).

## Links
- Use descriptive, meaningful link text.
- Match link text exactly to the target title/name.
- Avoid generic phrases like "click here."

## Tables
- Keep tables simple; avoid nested or merged cells.
- Use nonbreaking spaces if cells must be empty.
- Use “Yes”/“No” instead of “X” for status.

## Lists
- Keep list items and sublists simple and parallel.
- Limit list items to one idea/action each.

## Images
- Use images only to enhance understanding.
- Introduce images with descriptive text.
- Don’t convey new info only via images—describe it in text.
- Combine color with other indicators (shape, label).
- Ensure good contrast (check with tools like WebAIM).
- Use light theme and 100% zoom for screenshots.
- Always include alt text.

# Unbiased Documentation Guidelines

## General Principles
- Write for a global audience with inclusive and respectful language.
- Avoid idioms, slang, and culturally biased terms.
- Use gender-neutral pronouns and diverse examples.
- Meet accessibility standards in writing.
- Avoid biased, exclusionary, or harmful language.

## Identifying and Replacing Biased Language
- Avoid terms minimizing oppression or implying hierarchy  
  (e.g., use “deny list” instead of “blacklist”).
- Don’t assign positive/negative value to colors or races.
- Avoid euphemisms for disabilities; use person-first or identity-first language per context.
- When unsure, research terms and be open to feedback.

## Examples of Terms to Replace
| Avoid         | Use                                |
| ------------- | ---------------------------------- |
| Blacklist     | deny list                          |
| Master branch | main branch                        |
| Slave         | peer or replica                    |
| Mankind       | everyone                           |
| Sanity check  | review or verify                   |
| Flesh-colored | specify color (beige, cream, etc.) |

## Writing About Disability
- Use identity-first language if preferred by community.
- Use person-first language generally.
- Follow community preferences and recognized style guides (e.g., NCDJ).

## Device-Agnostic and Clear Instructions
- Use verbs focusing on the action, not the device or gesture.
- Prefer “select” over “click,” “tap,” or “press.”
- Use “enter” instead of “type” for text input.
- Use “open,” “check,” “uncheck,” “navigate to,” “remove” as appropriate.
- Avoid device-specific instructions unless necessary.
- Assume users may use keyboard, screen reader, voice, or mobile device.

# Capitalization Guidelines

## General Style
- Follow The Chicago Manual of Style.
- Use sentence style for most content (capitalize first word and proper nouns).
- Use headline style for guide names and documentation sets and table (headings).
- Maintain heading hierarchy and capitalization consistency.

## Specific Rules
- Capitalize the product names.
- Don’t capitalize general features or components.
- Use lowercase when referring to conceptual terms; capitalize UI elements.

# Formatting Documentation Elements
- Use monospaced font for code, commands, search queries.
- Use **bold** for UI elements, user inputs, and specific names.
- Use semantic tags appropriately (e.g., `<codeph>`, `<uicontrol>`, `<parmname>`).

# Abbreviations and Language Standards
- Spell out acronyms/initialisms at first use, then use abbreviation.
- Avoid common language abbreviations (e.g., “e.g.,”).
- Use American English with Merriam-Webster dictionary.
- Avoid foreign spellings or loanwords.
- Consult usage dictionaries for abbreviations and terminology.

# Clear and Direct Recommendations
- Avoid passive phrases like “it is recommended.”
- Use direct, imperative language for best practices.
- Explain why a method or option is preferred.
- Use callout boxes to highlight critical info or warnings.
- Keep notes and cautions concise and relevant; place them appropriately.



# Writing Best Practices

## Use Lists Effectively
- Use a list when presenting multiple steps, actions, or items.
- Avoid paragraphs for multi-step instructions; use a list instead.
- Start lists with a complete lead-in sentence that explains the purpose.
- Include more than one item per list.
- Limit each list item to one idea, item, or action.
- Use no more than two levels: a main list and one sublist.
- Use sublists only when necessary, and ensure they contain at least two items.
- Capitalize the first word of each list item.
- Follow proper punctuation rules:
  - Use periods if items are full sentences.
  - Omit punctuation if items are short phrases or words.
- Keep list structure parallel in grammar and tone.
- Avoid embedding links within task steps.
- Use tables instead of lists in tables only when essential.

## Use Tables Purposefully

### Do:
- Begin each table with a full introductory sentence and a colon.
- Include a header row with clear, capitalized headers.
- Capitalize the first letter of each cell entry.
- Keep entries in the same column structurally parallel.
- Use punctuation:
  - Include periods for full-sentence entries.
  - Omit punctuation for short phrases or single words.
- Use code snippets sparingly in tables.
- Place tables inside task steps only when appropriate.

### Don't:
- Avoid single-row tables—use a paragraph instead.
- Don’t use "X" for compatibility; use "Yes" or "No".
- Avoid nested tables and merged/split cells—use separate tables if needed.
- Don’t include lists inside table cells unless unavoidable.
- Don’t leave cells empty—insert a nonbreaking space (`&nbsp;`) if needed.
- Avoid including links unless cross-referencing.
- Don’t use footnotes or footnote-like symbols in or near tables.

---

# Formatting Guidelines

## General Principles
- Apply formatting consistently to aid readability.
- Choose formatting based on content structure (semantic tags vs. hard-coded text).

## Topic and Section Formatting
- Use sentence-style capitalization for titles and headings.
- Avoid using bold, italics, or ALL CAPS for emphasis.
- Enclose offset or quoted words in quotation marks.

---

# Element-Specific Formatting

## Computer Elements
- Use `<filepath>` for paths, directories, and filenames.
- Format tags and expressions in inline monospaced font.
- Leave file extensions unformatted.
- Use all caps for file types (e.g., TXT, JSON).
- Use bold for UI elements and menu paths; connect with "then".
- Format placeholders using `<placeholder>` or `<varname>`.
- Show user inputs in bold or `<userinput>`.
- Always display full URLs with `http` or `https`.

## Splunk Elements
- Use bold for index names, tokens, parameters, and roles.
- Format SPL/SPL2 commands, source types, and knowledge objects in monospaced font.
- Use monospaced blocks or search tables for example searches.
- Format Splexicon terms as bold links.

## Configuration Files
- Format attributes, values, arguments, and stanza names in monospaced font.
- Enclose stanza names in square brackets.
- Use monospaced font blocks for full configuration examples.

## Command-Line Interface (CLI)
- Format commands and short arguments in inline monospaced font.
- Use monospaced blocks for longer arguments or command lists.
- Format options or variables with italicized monospaced font: `<option>`.

## REST API
- Leave API names unformatted.
- Use bold for endpoint names, object names, and parameter names.
- Use `<filepath>` for endpoint paths.
- Format REST methods in all caps (e.g., GET, POST).
- Use monospaced blocks for full examples (requests/responses/JSON).
- Use inline monospaced font for request parameters and return values.

## Code and XML Formatting
- Format class names, methods, arguments, constants, and operators in monospaced font.
- Use bold for function names and user-created values.
- Use angle brackets for placeholders: `<variable>`.
- Use monospaced blocks for full code samples and usage examples.
- Format constants and hexadecimal values in all caps.
- Format Simple XML elements in monospaced font; use blocks for full examples.
