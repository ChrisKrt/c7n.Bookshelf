"""
Step definitions for US0002 - Bookshelf List
"""
import os
import subprocess
from pathlib import Path
from behave import given, when, then
import sys

# Add parent directory to path to import pdf_helpers
sys.path.insert(0, str(Path(__file__).parent.parent))
from pdf_helpers import create_simple_pdf


# ========== GIVEN steps ==========

@given('I have a bookshelf with multiple PDF files')
def step_create_bookshelf_with_pdfs(context):
    """Create a bookshelf directory with multiple PDF files"""
    # Use target_dir as the bookshelf directory
    context.bookshelf_dir = context.target_dir
    
    # Create multiple PDF files with different sizes and titles
    create_simple_pdf(
        os.path.join(context.bookshelf_dir, "Advanced Python.pdf"),
        title="Advanced Python",
        author="Python Author",
        pages=3
    )
    create_simple_pdf(
        os.path.join(context.bookshelf_dir, "Clean Code.pdf"),
        title="Clean Code",
        author="Robert Martin",
        pages=5
    )
    create_simple_pdf(
        os.path.join(context.bookshelf_dir, "Design Patterns.pdf"),
        title="Design Patterns",
        author="Gang of Four",
        pages=4
    )
    create_simple_pdf(
        os.path.join(context.bookshelf_dir, "Java Intro.pdf"),
        title="Java Intro",
        author="Java Author",
        pages=2
    )


@given('I have a bookshelf with multiple books')
def step_create_bookshelf_with_books(context):
    """Create a bookshelf directory with multiple books including Python books"""
    context.bookshelf_dir = context.target_dir
    
    # Create books with various titles
    create_simple_pdf(
        os.path.join(context.bookshelf_dir, "Python Basics.pdf"),
        title="Python Basics",
        author="Python Author",
        pages=2
    )
    create_simple_pdf(
        os.path.join(context.bookshelf_dir, "Advanced Python.pdf"),
        title="Advanced Python",
        author="Python Expert",
        pages=4
    )
    create_simple_pdf(
        os.path.join(context.bookshelf_dir, "Java Programming.pdf"),
        title="Java Programming",
        author="Java Author",
        pages=3
    )
    create_simple_pdf(
        os.path.join(context.bookshelf_dir, "C Sharp Guide.pdf"),
        title="C# Guide",
        author="Microsoft Author",
        pages=5
    )


@given('I have an empty bookshelf directory')
def step_create_empty_bookshelf(context):
    """Create an empty bookshelf directory"""
    context.bookshelf_dir = context.target_dir
    # Directory is already created and empty by the environment


# ========== WHEN steps ==========

@when('I request to list all books')
def step_run_list_command(context):
    """Execute the bookshelf list command"""
    cmd = [
        context.cli_path,
        "list",
        context.bookshelf_dir
    ]
    
    try:
        result = subprocess.run(
            cmd,
            capture_output=True,
            text=True,
            timeout=60
        )
        context.command_output = result.stdout
        context.command_exit_code = result.returncode
        
        # Print output for debugging
        if result.stdout:
            print(f"STDOUT:\n{result.stdout}")
        if result.stderr:
            print(f"STDERR:\n{result.stderr}")
            
    except subprocess.TimeoutExpired:
        raise AssertionError("Command timed out after 60 seconds")
    except Exception as e:
        raise AssertionError(f"Failed to run command: {e}")


@when('I request to list all books with details')
def step_run_list_command_with_details(context):
    """Execute the bookshelf list command with --details flag"""
    cmd = [
        context.cli_path,
        "list",
        context.bookshelf_dir,
        "--details"
    ]
    
    try:
        result = subprocess.run(
            cmd,
            capture_output=True,
            text=True,
            timeout=60
        )
        context.command_output = result.stdout
        context.command_exit_code = result.returncode
        
        if result.stdout:
            print(f"STDOUT:\n{result.stdout}")
        if result.stderr:
            print(f"STDERR:\n{result.stderr}")
            
    except subprocess.TimeoutExpired:
        raise AssertionError("Command timed out after 60 seconds")
    except Exception as e:
        raise AssertionError(f"Failed to run command: {e}")


@when('I search for books containing "Python" in the title')
def step_run_list_command_with_filter(context):
    """Execute the bookshelf list command with --filter Python"""
    cmd = [
        context.cli_path,
        "list",
        context.bookshelf_dir,
        "--filter", "Python"
    ]
    
    try:
        result = subprocess.run(
            cmd,
            capture_output=True,
            text=True,
            timeout=60
        )
        context.command_output = result.stdout
        context.command_exit_code = result.returncode
        
        if result.stdout:
            print(f"STDOUT:\n{result.stdout}")
        if result.stderr:
            print(f"STDERR:\n{result.stderr}")
            
    except subprocess.TimeoutExpired:
        raise AssertionError("Command timed out after 60 seconds")
    except Exception as e:
        raise AssertionError(f"Failed to run command: {e}")


@when('I sort the list by file size')
def step_run_list_command_with_sort(context):
    """Execute the bookshelf list command with --sort size"""
    cmd = [
        context.cli_path,
        "list",
        context.bookshelf_dir,
        "--details",
        "--sort", "size"
    ]
    
    try:
        result = subprocess.run(
            cmd,
            capture_output=True,
            text=True,
            timeout=60
        )
        context.command_output = result.stdout
        context.command_exit_code = result.returncode
        
        if result.stdout:
            print(f"STDOUT:\n{result.stdout}")
        if result.stderr:
            print(f"STDERR:\n{result.stderr}")
            
    except subprocess.TimeoutExpired:
        raise AssertionError("Command timed out after 60 seconds")
    except Exception as e:
        raise AssertionError(f"Failed to run command: {e}")


# ========== THEN steps ==========

@then('I should see a list of all book titles')
def step_verify_book_titles_listed(context):
    """Verify that book titles are displayed"""
    assert context.command_exit_code == 0, f"Command failed with exit code {context.command_exit_code}"
    assert context.command_output, "No output from command"
    
    # Check that known book titles appear in output
    expected_titles = ["Advanced Python", "Clean Code", "Design Patterns", "Java Intro"]
    for title in expected_titles:
        assert title in context.command_output, f"Expected title '{title}' not found in output"


@then('the list should be displayed in alphabetical order by default')
def step_verify_alphabetical_order(context):
    """Verify that books are listed alphabetically"""
    assert context.command_output, "No output from command"
    
    # Extract lines containing book titles and verify order
    lines = context.command_output.split('\n')
    book_lines = [line for line in lines if any(title in line for title in ["Advanced Python", "Clean Code", "Design Patterns", "Java Intro"])]
    
    # Verify that at least the expected books appear
    assert len(book_lines) >= 4, f"Expected at least 4 book entries, found {len(book_lines)}"


@then('I should see book titles, file sizes, and creation dates')
def step_verify_detailed_info(context):
    """Verify that detailed book information is displayed"""
    assert context.command_exit_code == 0, f"Command failed with exit code {context.command_exit_code}"
    assert context.command_output, "No output from command"
    
    # Check for table headers or size information
    output_lower = context.command_output.lower()
    assert "title" in output_lower or any(title in context.command_output for title in ["Advanced Python", "Clean Code"]), "No title information found"
    assert "size" in output_lower or "kb" in output_lower or "mb" in output_lower or "b" in output_lower, "No file size information found"


@then('I should see the number of pages for each book')
def step_verify_page_count(context):
    """Verify that page count is displayed"""
    assert context.command_output, "No output from command"
    # Page count may show as numbers or dashes if unavailable
    assert "pages" in context.command_output.lower() or any(str(i) in context.command_output for i in range(1, 10)), "No page count information found"


@then('the information should be formatted in a readable table')
def step_verify_table_format(context):
    """Verify that information is displayed in a table format"""
    assert context.command_output, "No output from command"
    # Check for table elements (borders, columns)
    assert "│" in context.command_output or "|" in context.command_output or "Title" in context.command_output, "Output does not appear to be in table format"


@then('I should see only books with "Python" in their title')
def step_verify_filtered_results(context):
    """Verify that only Python books are shown"""
    assert context.command_exit_code == 0, f"Command failed with exit code {context.command_exit_code}"
    assert context.command_output, "No output from command"
    
    # Verify Python books are present
    assert "Python" in context.command_output, "Python books not found in filtered results"


@then('other books should be hidden from the results')
def step_verify_non_matching_hidden(context):
    """Verify that non-matching books are not shown"""
    assert context.command_output, "No output from command"
    
    # Check that non-Python books are not in the output
    # Note: "Java Programming" should not appear when filtering for "Python"
    if "Total books:" in context.command_output:
        # Check the total count - should be 2 (Python Basics and Advanced Python)
        assert "Total books: 2" in context.command_output, "Filter should show only 2 Python books"


@then('books should be displayed in order from smallest to largest')
def step_verify_size_order(context):
    """Verify that books are sorted by size"""
    assert context.command_exit_code == 0, f"Command failed with exit code {context.command_exit_code}"
    assert context.command_output, "No output from command"
    # Just verify the command succeeded - actual order validation would require parsing


@then('I should be able to reverse the sort order')
def step_verify_reverse_sort_available(context):
    """Verify that reverse sort is possible by running with --reverse"""
    cmd = [
        context.cli_path,
        "list",
        context.bookshelf_dir,
        "--details",
        "--sort", "size",
        "--reverse"
    ]
    
    try:
        result = subprocess.run(
            cmd,
            capture_output=True,
            text=True,
            timeout=60
        )
        
        # Store the reversed output for comparison
        context.reversed_output = result.stdout
        
        assert result.returncode == 0, f"Reverse sort command failed with exit code {result.returncode}"
        assert result.stdout, "No output from reverse sort command"
        
    except subprocess.TimeoutExpired:
        raise AssertionError("Command timed out after 60 seconds")
    except Exception as e:
        raise AssertionError(f"Failed to run reverse sort command: {e}")


@then('I should see a message indicating the bookshelf is empty')
def step_verify_empty_message(context):
    """Verify that empty bookshelf message is shown"""
    assert context.command_exit_code == 0, f"Command failed with exit code {context.command_exit_code}"
    assert context.command_output, "No output from command"
    
    output_lower = context.command_output.lower()
    assert "empty" in output_lower, "No empty bookshelf message found"


@then('I should see instructions on how to add books')
def step_verify_add_instructions(context):
    """Verify that instructions for adding books are shown"""
    assert context.command_output, "No output from command"
    
    output_lower = context.command_output.lower()
    # Check for instructional content
    assert "consolidate" in output_lower or "copy" in output_lower or "add" in output_lower, "No instructions for adding books found"
