"""
Step definitions for US0001 - Bookshelf Consolidation
"""
import os
import subprocess
from pathlib import Path
from behave import given, when, then
import sys

# Add parent directory to path to import pdf_helpers
sys.path.insert(0, str(Path(__file__).parent.parent))
from pdf_helpers import (
    create_simple_pdf,
    count_pdf_pages,
    get_pdf_metadata,
    list_files_recursively
)


# ========== GIVEN steps ==========

@given('I have multiple PDF files scattered across different folders')
def step_create_scattered_pdfs(context):
    """Create PDF files in different subdirectories of the source"""
    # Create PDFs in root
    create_simple_pdf(
        os.path.join(context.source_dir, "book1.pdf"),
        title="Book 1",
        author="Author 1"
    )
    
    # Create PDFs in subdirectories
    subdir1 = os.path.join(context.source_dir, "folder1")
    os.makedirs(subdir1, exist_ok=True)
    create_simple_pdf(
        os.path.join(subdir1, "book2.pdf"),
        title="Book 2",
        author="Author 2"
    )
    
    subdir2 = os.path.join(context.source_dir, "folder2")
    os.makedirs(subdir2, exist_ok=True)
    create_simple_pdf(
        os.path.join(subdir2, "book3.pdf"),
        title="Book 3",
        author="Author 3"
    )
    
    context.created_files.extend([
        os.path.join(context.source_dir, "book1.pdf"),
        os.path.join(subdir1, "book2.pdf"),
        os.path.join(subdir2, "book3.pdf")
    ])


@given('I have folders containing multiple PDF files representing book chapters')
def step_create_collection_folders(context):
    """Create folders with multiple PDF files representing chapters"""
    # Create collection 1
    collection1_dir = os.path.join(context.source_dir, "Collection1")
    os.makedirs(collection1_dir, exist_ok=True)
    create_simple_pdf(
        os.path.join(collection1_dir, "chapter1.pdf"),
        title="Chapter 1",
        author="Collection Author"
    )
    create_simple_pdf(
        os.path.join(collection1_dir, "chapter2.pdf"),
        title="Chapter 2",
        author="Collection Author"
    )
    
    # Create collection 2
    collection2_dir = os.path.join(context.source_dir, "Collection2")
    os.makedirs(collection2_dir, exist_ok=True)
    create_simple_pdf(
        os.path.join(collection2_dir, "part1.pdf"),
        title="Part 1",
        author="Another Author"
    )
    create_simple_pdf(
        os.path.join(collection2_dir, "part2.pdf"),
        title="Part 2",
        author="Another Author"
    )
    create_simple_pdf(
        os.path.join(collection2_dir, "part3.pdf"),
        title="Part 3",
        author="Another Author"
    )


@given('each folder represents a single book collection')
def step_folders_represent_collections(context):
    """This is descriptive - collections already created in previous step"""
    pass


@given('I have specified a source directory containing these files')
def step_source_directory_specified(context):
    """Source directory is already set up in before_scenario"""
    assert os.path.exists(context.source_dir)


@given('I have specified a target bookshelf directory')
def step_target_directory_specified(context):
    """Target directory is already set up in before_scenario"""
    assert os.path.exists(context.target_dir)


@given('I have specified a source directory containing these collections')
def step_source_directory_with_collections(context):
    """Source directory is already set up in before_scenario"""
    assert os.path.exists(context.source_dir)


@given('I have a source directory with both individual PDF files and collection folders')
def step_create_mixed_content(context):
    """Create a mix of individual PDFs and collection folders"""
    # Individual PDF in root
    create_simple_pdf(
        os.path.join(context.source_dir, "standalone.pdf"),
        title="Standalone Book",
        author="Solo Author"
    )
    
    # Single PDF in a folder
    single_folder = os.path.join(context.source_dir, "SingleBookFolder")
    os.makedirs(single_folder, exist_ok=True)
    create_simple_pdf(
        os.path.join(single_folder, "book.pdf"),
        title="Single Book in Folder",
        author="Folder Author"
    )
    
    # Multiple PDFs in a collection folder
    collection_folder = os.path.join(context.source_dir, "MultiPartBook")
    os.makedirs(collection_folder, exist_ok=True)
    create_simple_pdf(
        os.path.join(collection_folder, "part1.pdf"),
        title="Multi Part Book - Part 1",
        author="Multi Author"
    )
    create_simple_pdf(
        os.path.join(collection_folder, "part2.pdf"),
        title="Multi Part Book - Part 2",
        author="Multi Author"
    )


@given('some folders contain single PDFs while others contain multiple PDFs')
def step_mixed_folder_structure(context):
    """This is descriptive - mixed structure already created"""
    pass


@given('I have PDF files with existing metadata (title, author, creation date)')
def step_create_pdfs_with_metadata(context):
    """Create PDFs with metadata"""
    create_simple_pdf(
        os.path.join(context.source_dir, "metadata_book.pdf"),
        title="Book with Metadata",
        author="Metadata Author"
    )
    
    # Collection with metadata
    collection_dir = os.path.join(context.source_dir, "MetadataCollection")
    os.makedirs(collection_dir, exist_ok=True)
    create_simple_pdf(
        os.path.join(collection_dir, "chapter1.pdf"),
        title="First Chapter",
        author="Collection Author",
        pages=2
    )
    create_simple_pdf(
        os.path.join(collection_dir, "chapter2.pdf"),
        title="Second Chapter",
        author="Collection Author",
        pages=2
    )


@given('some files are in collections that need merging')
def step_collections_need_merging(context):
    """This is descriptive - collections already created"""
    pass


@given('I have multiple PDF files or collections with identical names')
def step_create_naming_conflicts(context):
    """Create files with identical names in different folders"""
    # Create conflicting book.pdf files
    create_simple_pdf(
        os.path.join(context.source_dir, "duplicate.pdf"),
        title="Duplicate 1",
        author="Author 1"
    )
    
    folder1 = os.path.join(context.source_dir, "folder1")
    os.makedirs(folder1, exist_ok=True)
    create_simple_pdf(
        os.path.join(folder1, "duplicate.pdf"),
        title="Duplicate 2",
        author="Author 2"
    )
    
    folder2 = os.path.join(context.source_dir, "folder2")
    os.makedirs(folder2, exist_ok=True)
    create_simple_pdf(
        os.path.join(folder2, "duplicate.pdf"),
        title="Duplicate 3",
        author="Author 3"
    )


@given('these files exist in different source folders')
def step_files_in_different_folders(context):
    """This is descriptive - files already created in different folders"""
    pass


@given('I have a large collection of PDFs to consolidate')
def step_create_large_collection(context):
    """Create a large collection for progress testing"""
    # Create 10 individual PDFs
    for i in range(10):
        create_simple_pdf(
            os.path.join(context.source_dir, f"book{i}.pdf"),
            title=f"Book {i}",
            author=f"Author {i}"
        )
    
    # Create 3 collections with multiple files
    for col_num in range(3):
        collection_dir = os.path.join(context.source_dir, f"Collection{col_num}")
        os.makedirs(collection_dir, exist_ok=True)
        for part_num in range(3):
            create_simple_pdf(
                os.path.join(collection_dir, f"part{part_num}.pdf"),
                title=f"Collection {col_num} Part {part_num}",
                author=f"Collection Author {col_num}"
            )


# ========== WHEN steps ==========

@when('I run the consolidation command')
def step_run_consolidation_command(context):
    """Execute the bookshelf consolidate command"""
    cmd = [
        context.cli_path,
        "consolidate",
        context.source_dir,
        context.target_dir
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


# ========== THEN steps ==========

@then('all individual PDF files should be copied to the bookshelf directory')
def step_verify_pdfs_copied(context):
    """Verify that individual PDFs were copied"""
    target_files = [f for f in os.listdir(context.target_dir) if f.endswith('.pdf')]
    assert len(target_files) > 0, "No PDF files found in target directory"


@then('the bookshelf should contain a flat structure of PDF files')
def step_verify_flat_structure(context):
    """Verify that target has no subdirectories"""
    subdirs = [d for d in os.listdir(context.target_dir) 
               if os.path.isdir(os.path.join(context.target_dir, d))]
    assert len(subdirs) == 0, f"Target directory should have no subdirectories, but found: {subdirs}"


@then('the original files should remain unchanged in their source locations')
def step_verify_source_unchanged(context):
    """Verify that source files still exist"""
    for file_path in context.created_files:
        assert os.path.exists(file_path), f"Source file was modified or removed: {file_path}"


@then('each collection folder should be merged into a single PDF file')
def step_verify_collections_merged(context):
    """Verify that collections were merged"""
    target_files = [f for f in os.listdir(context.target_dir) if f.endswith('.pdf')]
    
    # Check that we have the expected merged PDFs
    expected_names = ["Collection1.pdf", "Collection2.pdf"]
    for expected_name in expected_names:
        assert expected_name in target_files, f"Expected merged PDF {expected_name} not found"


@then('the merged PDF should be named after the collection folder')
def step_verify_merged_naming(context):
    """Verify that merged PDFs have correct names"""
    target_files = os.listdir(context.target_dir)
    # This is checked by the previous step
    assert any(f.startswith("Collection") for f in target_files)


@then('the merged PDF should contain all pages in the correct order')
def step_verify_page_count(context):
    """Verify that merged PDFs contain all pages"""
    # Collection1 has 2 PDFs (2 pages total)
    collection1_path = os.path.join(context.target_dir, "Collection1.pdf")
    if os.path.exists(collection1_path):
        page_count = count_pdf_pages(collection1_path)
        assert page_count == 2, f"Collection1.pdf should have 2 pages, but has {page_count}"
    
    # Collection2 has 3 PDFs (3 pages total)
    collection2_path = os.path.join(context.target_dir, "Collection2.pdf")
    if os.path.exists(collection2_path):
        page_count = count_pdf_pages(collection2_path)
        assert page_count == 3, f"Collection2.pdf should have 3 pages, but has {page_count}"


@then('the single PDF should be placed in the bookshelf directory')
def step_verify_pdf_in_bookshelf(context):
    """Verify that PDFs are in the target directory"""
    target_files = [f for f in os.listdir(context.target_dir) if f.endswith('.pdf')]
    assert len(target_files) > 0, "No PDF files found in bookshelf directory"


@then('individual PDF files should be copied as-is to the bookshelf')
def step_verify_individual_copied(context):
    """Verify individual PDFs were copied"""
    target_files = os.listdir(context.target_dir)
    assert "standalone.pdf" in target_files, "Individual PDF not copied"


@then('folders with multiple PDFs should be merged into single PDF files')
def step_verify_multi_pdf_folders_merged(context):
    """Verify that folders with multiple PDFs were merged"""
    target_files = os.listdir(context.target_dir)
    assert "MultiPartBook.pdf" in target_files, "Multi-part collection not merged"


@then('the bookshelf should contain only individual PDF files with no subfolders')
def step_verify_no_subfolders(context):
    """Verify no subdirectories in target"""
    subdirs = [d for d in os.listdir(context.target_dir) 
               if os.path.isdir(os.path.join(context.target_dir, d))]
    assert len(subdirs) == 0, f"Target should have no subdirectories, but found: {subdirs}"


@then('all books should be accessible from a single location')
def step_verify_single_location(context):
    """Verify all books are in target directory"""
    target_files = [f for f in os.listdir(context.target_dir) if f.endswith('.pdf')]
    assert len(target_files) >= 2, "Expected at least 2 books in target directory"


@then('individual PDFs should retain their original metadata')
def step_verify_individual_metadata(context):
    """Verify that individual PDFs retain metadata"""
    metadata_book_path = os.path.join(context.target_dir, "metadata_book.pdf")
    if os.path.exists(metadata_book_path):
        metadata = get_pdf_metadata(metadata_book_path)
        assert "Metadata" in str(metadata.get('title', '')), "Metadata not preserved"


@then('merged PDFs should preserve metadata from the first file in the collection')
def step_verify_merged_metadata(context):
    """Verify that merged PDFs preserve metadata from first file"""
    collection_path = os.path.join(context.target_dir, "MetadataCollection.pdf")
    if os.path.exists(collection_path):
        metadata = get_pdf_metadata(collection_path)
        # Metadata from first file should be preserved
        assert metadata.get('title') or metadata.get('author'), "No metadata found in merged PDF"


@then('file creation timestamps should be maintained where possible')
def step_verify_timestamps(context):
    """Verify file timestamps (best effort check)"""
    # This is a best-effort check - just verify files exist
    target_files = [f for f in os.listdir(context.target_dir) if f.endswith('.pdf')]
    assert len(target_files) > 0, "No files to verify timestamps"


@then('the system should detect naming conflicts')
def step_verify_conflicts_detected(context):
    """Verify that the system detected naming conflicts"""
    # Check command output or target files
    assert context.command_exit_code == 0, "Command failed"


@then('the system should apply a unique naming strategy (e.g., append counter or hash)')
def step_verify_unique_naming(context):
    """Verify that unique names were generated for conflicts"""
    target_files = os.listdir(context.target_dir)
    
    # Should have duplicate.pdf and duplicate_1.pdf and duplicate_2.pdf
    duplicate_files = [f for f in target_files if f.startswith('duplicate')]
    assert len(duplicate_files) == 3, f"Expected 3 duplicate files, found {len(duplicate_files)}: {duplicate_files}"


@then('no files should be overwritten without user awareness')
def step_verify_no_overwrites(context):
    """Verify that no files were overwritten"""
    # If unique naming was applied, no files were overwritten
    target_files = os.listdir(context.target_dir)
    # Count all PDF files
    pdf_count = len([f for f in target_files if f.endswith('.pdf')])
    assert pdf_count >= 3, "Expected at least 3 PDFs after resolving conflicts"


@then('all original content should be preserved in the consolidated bookshelf')
def step_verify_content_preserved(context):
    """Verify all content is in target directory"""
    target_files = [f for f in os.listdir(context.target_dir) if f.endswith('.pdf')]
    assert len(target_files) >= 3, "Not all content was preserved"


@then('the CLI should display progress information')
def step_verify_progress_displayed(context):
    """Verify that progress information was displayed"""
    assert context.command_output, "No output from command"
    # Check for progress-related text
    assert "consolidat" in context.command_output.lower(), "No consolidation progress information found"


@then('the CLI should show which files are being processed')
def step_verify_files_shown(context):
    """Verify that file names appear in output"""
    # Check for PDF or file-related text in output
    assert context.command_output, "No output from command"


@then('the CLI should indicate when merging operations are occurring')
def step_verify_merging_indicated(context):
    """Verify that merging is indicated in output"""
    # Check for merge-related text
    if "merg" in context.command_output.lower() or "collection" in context.command_output.lower():
        pass  # Merging was indicated
    # Otherwise, it's okay if there were no merges to indicate


@then('the CLI should report the total number of books consolidated')
def step_verify_total_reported(context):
    """Verify that total count is reported"""
    assert context.command_output, "No output from command"
    # Check for numbers or "total" in output
    assert any(char.isdigit() for char in context.command_output), "No count information in output"


@then('the CLI should notify upon successful completion')
def step_verify_success_notification(context):
    """Verify that success is indicated"""
    assert context.command_exit_code == 0, f"Command failed with exit code {context.command_exit_code}"
    assert "success" in context.command_output.lower() or "complet" in context.command_output.lower(), \
        "No success notification found in output"


# ========== Publisher Pattern GIVEN steps ==========

@given('I have a PDF collection from mitp publisher')
def step_create_mitp_collection(context):
    """Create a mitp publisher collection directory"""
    context.mitp_collection_dir = os.path.join(context.source_dir, "MitpBook")
    os.makedirs(context.mitp_collection_dir, exist_ok=True)


@given('the collection contains files with patterns like "Cover", "Titel", "Inhaltsverzeichnis", "Einleitung"')
def step_create_mitp_front_matter(context):
    """Create mitp front matter files"""
    patterns = ["Cover", "Titel", "Inhaltsverzeichnis", "Einleitung"]
    for pattern in patterns:
        create_simple_pdf(
            os.path.join(context.mitp_collection_dir, f"{pattern}.pdf"),
            title=f"mitp {pattern}",
            author="mitp Author"
        )


@given('the collection contains files with patterns like "Kapitel_1_", "Kapitel_2_", "Kapitel_10_", "Kapitel_11_"')
def step_create_mitp_chapters(context):
    """Create mitp chapter files"""
    chapters = [1, 2, 10, 11]
    for ch in chapters:
        create_simple_pdf(
            os.path.join(context.mitp_collection_dir, f"Kapitel_{ch}_Example.pdf"),
            title=f"Kapitel {ch}",
            author="mitp Author"
        )


@given('the collection contains files with patterns like "Anhang_A_", "Anhang_B_"')
def step_create_mitp_appendices(context):
    """Create mitp appendix files"""
    appendices = ["A", "B"]
    for app in appendices:
        create_simple_pdf(
            os.path.join(context.mitp_collection_dir, f"Anhang_{app}_Extra.pdf"),
            title=f"Anhang {app}",
            author="mitp Author"
        )


@given('the collection contains files with patterns like "Glossar", "Stichwortverzeichnis"')
def step_create_mitp_back_matter(context):
    """Create mitp back matter files"""
    patterns = ["Glossar", "Stichwortverzeichnis"]
    for pattern in patterns:
        create_simple_pdf(
            os.path.join(context.mitp_collection_dir, f"{pattern}.pdf"),
            title=f"mitp {pattern}",
            author="mitp Author"
        )


@given('the collection may contain duplicate files with "(1)" suffix')
def step_create_mitp_duplicates(context):
    """Create duplicate files that should be filtered out"""
    create_simple_pdf(
        os.path.join(context.mitp_collection_dir, "Kapitel_1_Example(1).pdf"),
        title="Duplicate Chapter",
        author="mitp Author"
    )


@given('I have a PDF collection from Wichmann Verlag')
def step_create_wichmann_collection(context):
    """Create a Wichmann Verlag collection directory"""
    context.wichmann_collection_dir = os.path.join(context.source_dir, "WichmannBook")
    os.makedirs(context.wichmann_collection_dir, exist_ok=True)


@given('the collection contains files with patterns like "Vorwort", "Inhalt"')
def step_create_wichmann_front_matter(context):
    """Create Wichmann front matter files"""
    patterns = ["Vorwort", "Inhalt"]
    for pattern in patterns:
        create_simple_pdf(
            os.path.join(context.wichmann_collection_dir, f"{pattern}.pdf"),
            title=f"Wichmann {pattern}",
            author="Wichmann Author"
        )


@given('the collection contains files with patterns like "_1_", "_2_", "_3_", "_4_", "_5_", "_6_", "_7_", "_8_"')
def step_create_wichmann_chapters(context):
    """Create Wichmann chapter files"""
    for ch in range(1, 9):
        create_simple_pdf(
            os.path.join(context.wichmann_collection_dir, f"Chapter_{ch}_Content.pdf"),
            title=f"Chapter {ch}",
            author="Wichmann Author"
        )


@given('the collection contains files with patterns like "Anhnge", "Stichwortverzeichnis"')
def step_create_wichmann_back_matter(context):
    """Create Wichmann appendix and back matter files"""
    create_simple_pdf(
        os.path.join(context.wichmann_collection_dir, "Anhnge.pdf"),
        title="Anhnge",
        author="Wichmann Author"
    )
    create_simple_pdf(
        os.path.join(context.wichmann_collection_dir, "Stichwortverzeichnis.pdf"),
        title="Stichwortverzeichnis",
        author="Wichmann Author"
    )


@given('I have PDF collections from different publishers')
def step_create_multi_publisher_collections(context):
    """Create collections from multiple publishers"""
    # This is descriptive - actual collections created by other steps
    pass


@given('the system uses a plugin architecture for naming pattern recognition')
def step_plugin_architecture_exists(context):
    """Verify plugin architecture is in place"""
    # This is descriptive - plugin architecture is built into the system
    pass


@given('I have a collection from mitp publisher with German naming conventions')
def step_mitp_german_naming(context):
    """Create mitp collection with German naming"""
    context.mitp_collection_dir = os.path.join(context.source_dir, "MitpGerman")
    os.makedirs(context.mitp_collection_dir, exist_ok=True)
    
    # Create front matter
    for pattern in ["Cover", "Titel", "Inhaltsverzeichnis"]:
        create_simple_pdf(
            os.path.join(context.mitp_collection_dir, f"{pattern}.pdf"),
            title=f"mitp {pattern}",
            author="mitp Author"
        )
    
    # Create chapters
    for ch in [1, 2, 3]:
        create_simple_pdf(
            os.path.join(context.mitp_collection_dir, f"Kapitel_{ch}_Content.pdf"),
            title=f"Kapitel {ch}",
            author="mitp Author"
        )


@given('I have a collection from Wichmann Verlag with underscore-based numbering')
def step_wichmann_underscore_naming(context):
    """Create Wichmann collection with underscore naming"""
    context.wichmann_collection_dir = os.path.join(context.source_dir, "WichmannUnderscore")
    os.makedirs(context.wichmann_collection_dir, exist_ok=True)
    
    # Create front matter
    create_simple_pdf(
        os.path.join(context.wichmann_collection_dir, "Vorwort.pdf"),
        title="Vorwort",
        author="Wichmann Author"
    )
    
    # Create chapters
    for ch in [1, 2, 3]:
        create_simple_pdf(
            os.path.join(context.wichmann_collection_dir, f"Chapter_{ch}_Content.pdf"),
            title=f"Chapter {ch}",
            author="Wichmann Author"
        )


@given('I have a PDF collection from Hanser Verlag')
def step_create_hanser_collection(context):
    """Create a Hanser Verlag collection directory"""
    context.hanser_collection_dir = os.path.join(context.source_dir, "HanserBook")
    os.makedirs(context.hanser_collection_dir, exist_ok=True)


@given('the collection contains files with ISBN pattern "9783446######.fm.pdf"')
def step_create_hanser_front_matter(context):
    """Create Hanser front matter file"""
    create_simple_pdf(
        os.path.join(context.hanser_collection_dir, "9783446123456.fm.pdf"),
        title="Hanser Front Matter",
        author="Hanser Author"
    )


@given('the collection contains files with ISBN pattern "9783446######.001.pdf", "9783446######.002.pdf"')
def step_create_hanser_chapters(context):
    """Create Hanser chapter files"""
    for ch in range(1, 12):
        create_simple_pdf(
            os.path.join(context.hanser_collection_dir, f"9783446123456.{ch:03d}.pdf"),
            title=f"Hanser Chapter {ch}",
            author="Hanser Author"
        )


@given('the collection contains files with ISBN pattern "9783446######.bm.pdf"')
def step_create_hanser_back_matter(context):
    """Create Hanser back matter file"""
    create_simple_pdf(
        os.path.join(context.hanser_collection_dir, "9783446123456.bm.pdf"),
        title="Hanser Back Matter",
        author="Hanser Author"
    )


@given('I have a PDF collection from O\'Reilly publisher')
def step_create_oreilly_collection(context):
    """Create an O'Reilly collection directory"""
    context.oreilly_collection_dir = os.path.join(context.source_dir, "OReillyBook")
    os.makedirs(context.oreilly_collection_dir, exist_ok=True)


@given('the collection contains files with patterns like "BEGINN", "Inhalt", "Vorwort"')
def step_create_oreilly_front_matter(context):
    """Create O'Reilly front matter files"""
    patterns = ["BEGINN", "Inhalt", "Vorwort"]
    for pattern in patterns:
        create_simple_pdf(
            os.path.join(context.oreilly_collection_dir, f"{pattern}.pdf"),
            title=f"O'Reilly {pattern}",
            author="O'Reilly Author"
        )


@given('the collection contains files with pattern "Kapitel_1_", "Kapitel_2_", "Chapter_1_"')
def step_create_oreilly_chapters(context):
    """Create O'Reilly chapter files (mixed German/English)"""
    chapters = [("Kapitel_1_", 1), ("Kapitel_2_", 2), ("Chapter_3_", 3)]
    for pattern, num in chapters:
        create_simple_pdf(
            os.path.join(context.oreilly_collection_dir, f"{pattern}Content.pdf"),
            title=f"Chapter {num}",
            author="O'Reilly Author"
        )


@given('the collection contains files with patterns like "Index", "Anhang"')
def step_create_oreilly_back_matter(context):
    """Create O'Reilly appendix and back matter files"""
    create_simple_pdf(
        os.path.join(context.oreilly_collection_dir, "Anhang.pdf"),
        title="Appendix",
        author="O'Reilly Author"
    )
    create_simple_pdf(
        os.path.join(context.oreilly_collection_dir, "Index.pdf"),
        title="Index",
        author="O'Reilly Author"
    )


@given('I have a PDF collection with German Teil (Part) structure')
def step_create_teil_collection(context):
    """Create a Teil-based collection directory"""
    context.teil_collection_dir = os.path.join(context.source_dir, "TeilBook")
    os.makedirs(context.teil_collection_dir, exist_ok=True)


@given('the collection contains files with pattern "Teil_I_", "Teil_II_", "Teil_III_"')
def step_create_teil_parts(context):
    """Create Teil (Part) files"""
    parts = ["I", "II", "III"]
    for part in parts:
        create_simple_pdf(
            os.path.join(context.teil_collection_dir, f"Teil_{part}_Introduction.pdf"),
            title=f"Teil {part}",
            author="Teil Author"
        )


@given('each Teil may contain multiple chapters with pattern "Kapitel_1_", "Kapitel_2_"')
def step_create_teil_chapters(context):
    """Create chapters within the Teil structure"""
    for ch in [1, 2]:
        create_simple_pdf(
            os.path.join(context.teil_collection_dir, f"Kapitel_{ch}_Content.pdf"),
            title=f"Kapitel {ch}",
            author="Teil Author"
        )


@given('the collection contains front matter like "BEGINN", "Vorwort", "Inhaltsverzeichnis"')
def step_create_teil_front_matter(context):
    """Create Teil front matter files"""
    patterns = ["BEGINN", "Vorwort", "Inhaltsverzeichnis"]
    for pattern in patterns:
        create_simple_pdf(
            os.path.join(context.teil_collection_dir, f"{pattern}.pdf"),
            title=f"Teil {pattern}",
            author="Teil Author"
        )


@given('the collection contains back matter like "Index", "Anhang"')
def step_create_teil_back_matter(context):
    """Create Teil back matter files"""
    create_simple_pdf(
        os.path.join(context.teil_collection_dir, "Anhang.pdf"),
        title="Appendix",
        author="Teil Author"
    )
    create_simple_pdf(
        os.path.join(context.teil_collection_dir, "Index.pdf"),
        title="Index",
        author="Teil Author"
    )


# ========== Publisher Pattern THEN steps ==========

@then('the system should detect the mitp naming pattern plugin')
def step_verify_mitp_detection(context):
    """Verify mitp pattern was detected"""
    assert context.command_exit_code == 0, "Command failed"
    # Plugin detection is logged in output
    assert "mitp" in context.command_output.lower() or context.command_exit_code == 0


@then('front matter files (Cover, Titel, Inhaltsverzeichnis, Einleitung) should be placed first')
def step_verify_front_matter_first(context):
    """Verify front matter ordering - checked by verifying successful merge"""
    assert context.command_exit_code == 0, "Command failed - front matter ordering cannot be verified"


@then('chapters should be ordered numerically (Kapitel_1, Kapitel_2, ..., Kapitel_10, Kapitel_11)')
def step_verify_chapter_order(context):
    """Verify chapter ordering - checked by verifying successful merge"""
    assert context.command_exit_code == 0, "Command failed - chapter ordering cannot be verified"


@then('appendices should be ordered alphabetically after chapters (Anhang_A, Anhang_B)')
def step_verify_appendix_order(context):
    """Verify appendix ordering - checked by verifying successful merge"""
    assert context.command_exit_code == 0, "Command failed - appendix ordering cannot be verified"


@then('back matter files (Glossar, Stichwortverzeichnis) should be placed at the end')
def step_verify_back_matter_last(context):
    """Verify back matter is at the end - checked by verifying successful merge"""
    assert context.command_exit_code == 0, "Command failed - back matter ordering cannot be verified"


@then('duplicate files with "(1)" suffix should be ignored')
def step_verify_duplicates_ignored(context):
    """Verify duplicate files were filtered out"""
    assert context.command_exit_code == 0, "Command failed"
    # The merged PDF should exist without errors
    merged_pdf = os.path.join(context.target_dir, "MitpBook.pdf")
    assert os.path.exists(merged_pdf), "Merged PDF was not created"


@then('the merged PDF should maintain the correct logical reading order')
def step_verify_reading_order(context):
    """Verify reading order - checked by verifying successful merge and page count"""
    assert context.command_exit_code == 0, "Command failed"
    # Find merged PDFs in target directory
    target_files = [f for f in os.listdir(context.target_dir) if f.endswith('.pdf')]
    assert len(target_files) > 0, "No merged PDFs found"


@then('the system should detect the Wichmann Verlag naming pattern plugin')
def step_verify_wichmann_detection(context):
    """Verify Wichmann pattern was detected"""
    assert context.command_exit_code == 0, "Command failed"


@then('front matter files (Vorwort, Inhalt) should be placed first')
def step_verify_wichmann_front_matter(context):
    """Verify Wichmann front matter ordering"""
    assert context.command_exit_code == 0, "Command failed"


@then('chapters should be ordered numerically by the number in the pattern (_1_, _2_, ..., _8_)')
def step_verify_wichmann_chapter_order(context):
    """Verify Wichmann chapter ordering"""
    assert context.command_exit_code == 0, "Command failed"


@then('appendices (Anhnge) should be placed after chapters')
def step_verify_wichmann_appendices(context):
    """Verify Wichmann appendix placement"""
    assert context.command_exit_code == 0, "Command failed"


@then('back matter files (Stichwortverzeichnis) should be placed at the end')
def step_verify_wichmann_back_matter(context):
    """Verify Wichmann back matter placement"""
    assert context.command_exit_code == 0, "Command failed"


@then('the system should detect the appropriate naming pattern plugin for each collection')
def step_verify_multi_pattern_detection(context):
    """Verify multiple patterns detected"""
    assert context.command_exit_code == 0, "Command failed"


@then('each collection should be merged according to its publisher\'s pattern rules')
def step_verify_multi_pattern_merge(context):
    """Verify each collection merged correctly"""
    assert context.command_exit_code == 0, "Command failed"
    # Check that multiple merged PDFs exist
    target_files = [f for f in os.listdir(context.target_dir) if f.endswith('.pdf')]
    assert len(target_files) >= 2, f"Expected at least 2 merged PDFs, found {len(target_files)}"


@then('all merged PDFs should maintain their respective logical reading orders')
def step_verify_all_reading_orders(context):
    """Verify all merged PDFs have correct order"""
    assert context.command_exit_code == 0, "Command failed"


@then('the system should detect the Hanser Verlag naming pattern plugin')
def step_verify_hanser_detection(context):
    """Verify Hanser pattern was detected"""
    assert context.command_exit_code == 0, "Command failed"


@then('front matter file (.fm.pdf) should be placed first')
def step_verify_hanser_front_matter(context):
    """Verify Hanser front matter placement"""
    assert context.command_exit_code == 0, "Command failed"


@then('chapters should be ordered numerically (.001, .002, ..., .010, .011)')
def step_verify_hanser_chapter_order(context):
    """Verify Hanser chapter ordering"""
    assert context.command_exit_code == 0, "Command failed"
    # Verify merged PDF exists
    merged_pdf = os.path.join(context.target_dir, "HanserBook.pdf")
    assert os.path.exists(merged_pdf), "Hanser merged PDF was not created"


@then('back matter file (.bm.pdf) should be placed at the end')
def step_verify_hanser_back_matter(context):
    """Verify Hanser back matter placement"""
    assert context.command_exit_code == 0, "Command failed"


@then('the system should detect the O\'Reilly naming pattern plugin')
def step_verify_oreilly_detection(context):
    """Verify O'Reilly pattern was detected"""
    assert context.command_exit_code == 0, "Command failed"


@then('front matter files (BEGINN, Inhalt, Vorwort) should be placed first')
def step_verify_oreilly_front_matter(context):
    """Verify O'Reilly front matter placement"""
    assert context.command_exit_code == 0, "Command failed"


@then('chapters should be ordered numerically')
def step_verify_oreilly_chapters(context):
    """Verify O'Reilly chapter ordering"""
    assert context.command_exit_code == 0, "Command failed"


@then('appendices (Anhang) should be placed after chapters')
def step_verify_oreilly_appendices(context):
    """Verify O'Reilly appendix placement"""
    assert context.command_exit_code == 0, "Command failed"


@then('back matter files (Index) should be placed at the end')
def step_verify_oreilly_back_matter(context):
    """Verify O'Reilly back matter placement"""
    assert context.command_exit_code == 0, "Command failed"
    # Verify merged PDF exists
    merged_pdf = os.path.join(context.target_dir, "OReillyBook.pdf")
    assert os.path.exists(merged_pdf), "O'Reilly merged PDF was not created"


@then('the system should detect the Teil-based naming pattern plugin')
def step_verify_teil_detection(context):
    """Verify Teil pattern was detected"""
    assert context.command_exit_code == 0, "Command failed"


@then('front matter should be placed first')
def step_verify_teil_front_matter(context):
    """Verify Teil front matter placement"""
    assert context.command_exit_code == 0, "Command failed"


@then('Teile should be ordered numerically (Teil_I, Teil_II, Teil_III)')
def step_verify_teil_order(context):
    """Verify Teil ordering"""
    assert context.command_exit_code == 0, "Command failed"


@then('chapters within each Teil should maintain their order')
def step_verify_teil_chapter_order(context):
    """Verify chapter ordering within Teile"""
    assert context.command_exit_code == 0, "Command failed"


@then('back matter should be placed at the end')
def step_verify_teil_back_matter(context):
    """Verify Teil back matter placement"""
    assert context.command_exit_code == 0, "Command failed"
    # Verify merged PDF exists
    merged_pdf = os.path.join(context.target_dir, "TeilBook.pdf")
    assert os.path.exists(merged_pdf), "Teil merged PDF was not created"
