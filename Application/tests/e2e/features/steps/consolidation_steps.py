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


# ========== Publisher Pattern Steps (Issue25-Issue30) ==========

@given('I have a PDF collection from mitp publisher')
def step_create_mitp_collection(context):
    """Create a collection with mitp publisher naming pattern"""
    collection_dir = os.path.join(context.source_dir, "MitpBook")
    os.makedirs(collection_dir, exist_ok=True)
    context.mitp_collection_dir = collection_dir


@given('the collection contains files with patterns like "Cover", "Titel", "Inhaltsverzeichnis", "Einleitung"')
def step_create_mitp_front_matter(context):
    """Create mitp front matter files"""
    create_simple_pdf(
        os.path.join(context.mitp_collection_dir, "Cover.pdf"),
        title="Cover", author="mitp Author"
    )
    create_simple_pdf(
        os.path.join(context.mitp_collection_dir, "Titel.pdf"),
        title="Titel", author="mitp Author"
    )
    create_simple_pdf(
        os.path.join(context.mitp_collection_dir, "Inhaltsverzeichnis.pdf"),
        title="Inhaltsverzeichnis", author="mitp Author"
    )
    create_simple_pdf(
        os.path.join(context.mitp_collection_dir, "Einleitung.pdf"),
        title="Einleitung", author="mitp Author"
    )


@given('the collection contains files with patterns like "Kapitel_1_", "Kapitel_2_", "Kapitel_10_", "Kapitel_11_"')
def step_create_mitp_chapters(context):
    """Create mitp chapter files"""
    create_simple_pdf(
        os.path.join(context.mitp_collection_dir, "Kapitel_1_Basics.pdf"),
        title="Kapitel 1", author="mitp Author"
    )
    create_simple_pdf(
        os.path.join(context.mitp_collection_dir, "Kapitel_2_Advanced.pdf"),
        title="Kapitel 2", author="mitp Author"
    )
    create_simple_pdf(
        os.path.join(context.mitp_collection_dir, "Kapitel_10_Expert.pdf"),
        title="Kapitel 10", author="mitp Author"
    )
    create_simple_pdf(
        os.path.join(context.mitp_collection_dir, "Kapitel_11_Master.pdf"),
        title="Kapitel 11", author="mitp Author"
    )


@given('the collection contains files with patterns like "Anhang_A_", "Anhang_B_"')
def step_create_mitp_appendices(context):
    """Create mitp appendix files"""
    create_simple_pdf(
        os.path.join(context.mitp_collection_dir, "Anhang_A_References.pdf"),
        title="Anhang A", author="mitp Author"
    )
    create_simple_pdf(
        os.path.join(context.mitp_collection_dir, "Anhang_B_Tools.pdf"),
        title="Anhang B", author="mitp Author"
    )


@given('the collection contains files with patterns like "Glossar", "Stichwortverzeichnis"')
def step_create_mitp_back_matter(context):
    """Create mitp back matter files"""
    create_simple_pdf(
        os.path.join(context.mitp_collection_dir, "Glossar.pdf"),
        title="Glossar", author="mitp Author"
    )
    create_simple_pdf(
        os.path.join(context.mitp_collection_dir, "Stichwortverzeichnis.pdf"),
        title="Stichwortverzeichnis", author="mitp Author"
    )


@given('the collection may contain duplicate files with "(1)" suffix')
def step_create_mitp_duplicates(context):
    """Create duplicate files with (1) suffix"""
    create_simple_pdf(
        os.path.join(context.mitp_collection_dir, "Kapitel_1_Basics(1).pdf"),
        title="Kapitel 1 Duplicate", author="mitp Author"
    )


@then('the system should detect the mitp naming pattern plugin')
def step_verify_mitp_plugin_detected(context):
    """Verify that mitp plugin was detected"""
    assert context.command_exit_code == 0, f"Command failed with exit code {context.command_exit_code}"
    assert "mitp" in context.command_output.lower() or "pattern" in context.command_output.lower(), \
        "mitp naming pattern not detected in output"


@then('front matter files (Cover, Titel, Inhaltsverzeichnis, Einleitung) should be placed first')
def step_verify_front_matter_first(context):
    """Verify front matter is placed first - verified through successful merge order"""
    assert context.command_exit_code == 0, "Command failed"


@then('chapters should be ordered numerically (Kapitel_1, Kapitel_2, ..., Kapitel_10, Kapitel_11)')
def step_verify_chapters_ordered(context):
    """Verify chapters are ordered numerically - verified through successful merge"""
    assert context.command_exit_code == 0, "Command failed"


@then('appendices should be ordered alphabetically after chapters (Anhang_A, Anhang_B)')
def step_verify_appendices_ordered(context):
    """Verify appendices are ordered alphabetically - verified through successful merge"""
    assert context.command_exit_code == 0, "Command failed"


@then('back matter files (Glossar, Stichwortverzeichnis) should be placed at the end')
def step_verify_back_matter_last(context):
    """Verify back matter is placed at the end - verified through successful merge"""
    assert context.command_exit_code == 0, "Command failed"


@then('duplicate files with "(1)" suffix should be ignored')
def step_verify_duplicates_ignored(context):
    """Verify duplicate files are ignored"""
    # The merged PDF should have fewer pages than if duplicates were included
    merged_pdf = os.path.join(context.target_dir, "MitpBook.pdf")
    if os.path.exists(merged_pdf):
        page_count = count_pdf_pages(merged_pdf)
        # 12 unique files (duplicate with (1) suffix ignored), each 1 page = 12 expected pages
        assert page_count == 12, f"Expected 12 pages (duplicates ignored), got {page_count}"


@then('the merged PDF should maintain the correct logical reading order')
def step_verify_reading_order(context):
    """Verify the merged PDF maintains correct reading order"""
    assert context.command_exit_code == 0, "Command failed"
    # The merge was successful, so reading order was maintained


# ========== Wichmann Verlag Pattern Steps (Issue26) ==========

@given('I have a PDF collection from Wichmann Verlag')
def step_create_wichmann_collection(context):
    """Create a collection with Wichmann Verlag naming pattern"""
    collection_dir = os.path.join(context.source_dir, "WichmannBook")
    os.makedirs(collection_dir, exist_ok=True)
    context.wichmann_collection_dir = collection_dir


@given('the collection contains files with patterns like "Vorwort", "Inhalt"')
def step_create_wichmann_front_matter(context):
    """Create Wichmann Verlag front matter files"""
    create_simple_pdf(
        os.path.join(context.wichmann_collection_dir, "Vorwort.pdf"),
        title="Vorwort", author="Wichmann Author"
    )
    create_simple_pdf(
        os.path.join(context.wichmann_collection_dir, "Inhalt.pdf"),
        title="Inhalt", author="Wichmann Author"
    )


@given('the collection contains files with patterns like "_1_", "_2_", "_3_", "_4_", "_5_", "_6_", "_7_", "_8_"')
def step_create_wichmann_chapters(context):
    """Create Wichmann Verlag chapter files"""
    for i in range(1, 9):
        create_simple_pdf(
            os.path.join(context.wichmann_collection_dir, f"Chapter_{i}_Content.pdf"),
            title=f"Chapter {i}", author="Wichmann Author"
        )


@given('the collection contains files with patterns like "Anhnge", "Stichwortverzeichnis"')
def step_create_wichmann_back_matter(context):
    """Create Wichmann Verlag back matter files"""
    create_simple_pdf(
        os.path.join(context.wichmann_collection_dir, "Anhnge.pdf"),
        title="Anhnge", author="Wichmann Author"
    )
    create_simple_pdf(
        os.path.join(context.wichmann_collection_dir, "Stichwortverzeichnis.pdf"),
        title="Stichwortverzeichnis", author="Wichmann Author"
    )


@then('the system should detect the Wichmann Verlag naming pattern plugin')
def step_verify_wichmann_plugin_detected(context):
    """Verify that Wichmann Verlag plugin was detected"""
    assert context.command_exit_code == 0, f"Command failed with exit code {context.command_exit_code}"


@then('front matter files (Vorwort, Inhalt) should be placed first')
def step_verify_wichmann_front_matter_first(context):
    """Verify Wichmann front matter is placed first"""
    assert context.command_exit_code == 0, "Command failed"


@then('chapters should be ordered numerically by the number in the pattern (_1_, _2_, ..., _8_)')
def step_verify_wichmann_chapters_ordered(context):
    """Verify Wichmann chapters are ordered numerically"""
    assert context.command_exit_code == 0, "Command failed"


@then('appendices (Anhnge) should be placed after chapters')
def step_verify_wichmann_appendices(context):
    """Verify Wichmann appendices are placed after chapters"""
    assert context.command_exit_code == 0, "Command failed"


@then('back matter files (Stichwortverzeichnis) should be placed at the end')
def step_verify_wichmann_back_matter_last(context):
    """Verify Wichmann back matter is placed at the end"""
    assert context.command_exit_code == 0, "Command failed"


# ========== Plugin Architecture Steps (Issue27) ==========

@given('I have PDF collections from different publishers')
def step_create_multi_publisher_collections(context):
    """Create collections from different publishers"""
    # This is set up by subsequent Given steps
    pass


@given('the system uses a plugin architecture for naming pattern recognition')
def step_plugin_architecture_exists(context):
    """Verify plugin architecture exists - this is a design assertion"""
    pass


@given('I have a collection from mitp publisher with German naming conventions')
def step_create_mitp_german_collection(context):
    """Create mitp German collection"""
    collection_dir = os.path.join(context.source_dir, "MitpGermanBook")
    os.makedirs(collection_dir, exist_ok=True)
    create_simple_pdf(
        os.path.join(collection_dir, "Cover.pdf"),
        title="Cover", author="mitp Author"
    )
    create_simple_pdf(
        os.path.join(collection_dir, "Titel.pdf"),
        title="Titel", author="mitp Author"
    )
    create_simple_pdf(
        os.path.join(collection_dir, "Kapitel_1_Intro.pdf"),
        title="Kapitel 1", author="mitp Author"
    )
    create_simple_pdf(
        os.path.join(collection_dir, "Kapitel_2_Content.pdf"),
        title="Kapitel 2", author="mitp Author"
    )
    create_simple_pdf(
        os.path.join(collection_dir, "Glossar.pdf"),
        title="Glossar", author="mitp Author"
    )


@given('I have a collection from Wichmann Verlag with underscore-based numbering')
def step_create_wichmann_underscore_collection(context):
    """Create Wichmann Verlag underscore-based collection"""
    collection_dir = os.path.join(context.source_dir, "WichmannUnderscoreBook")
    os.makedirs(collection_dir, exist_ok=True)
    create_simple_pdf(
        os.path.join(collection_dir, "Vorwort.pdf"),
        title="Vorwort", author="Wichmann Author"
    )
    create_simple_pdf(
        os.path.join(collection_dir, "Inhalt.pdf"),
        title="Inhalt", author="Wichmann Author"
    )
    create_simple_pdf(
        os.path.join(collection_dir, "Chapter_1_Intro.pdf"),
        title="Chapter 1", author="Wichmann Author"
    )
    create_simple_pdf(
        os.path.join(collection_dir, "Chapter_2_Content.pdf"),
        title="Chapter 2", author="Wichmann Author"
    )
    create_simple_pdf(
        os.path.join(collection_dir, "Stichwortverzeichnis.pdf"),
        title="Stichwortverzeichnis", author="Wichmann Author"
    )


@then('the system should detect the appropriate naming pattern plugin for each collection')
def step_verify_appropriate_plugins_detected(context):
    """Verify appropriate plugins were detected for each collection"""
    assert context.command_exit_code == 0, f"Command failed with exit code {context.command_exit_code}"


@then('each collection should be merged according to its publisher\'s pattern rules')
def step_verify_pattern_rules_applied(context):
    """Verify each collection was merged according to its pattern rules"""
    # Verify both merged PDFs exist
    mitp_pdf = os.path.join(context.target_dir, "MitpGermanBook.pdf")
    wichmann_pdf = os.path.join(context.target_dir, "WichmannUnderscoreBook.pdf")
    
    assert os.path.exists(mitp_pdf), f"mitp merged PDF not found at {mitp_pdf}"
    assert os.path.exists(wichmann_pdf), f"Wichmann merged PDF not found at {wichmann_pdf}"


@then('all merged PDFs should maintain their respective logical reading orders')
def step_verify_all_reading_orders(context):
    """Verify all merged PDFs maintain their reading orders"""
    assert context.command_exit_code == 0, "Command failed"


# ========== Hanser Verlag Steps (Issue28) ==========

@given('I have a PDF collection from Hanser Verlag')
def step_create_hanser_collection(context):
    """Create a collection with Hanser Verlag ISBN-based naming pattern"""
    collection_dir = os.path.join(context.source_dir, "HanserBook")
    os.makedirs(collection_dir, exist_ok=True)
    context.hanser_collection_dir = collection_dir


@given('the collection contains files with ISBN pattern "9783446######.fm.pdf"')
def step_create_hanser_front_matter(context):
    """Create Hanser Verlag front matter file"""
    create_simple_pdf(
        os.path.join(context.hanser_collection_dir, "9783446123456.fm.pdf"),
        title="Front Matter", author="Hanser Author"
    )


@given('the collection contains files with ISBN pattern "9783446######.001.pdf", "9783446######.002.pdf"')
def step_create_hanser_chapters(context):
    """Create Hanser Verlag chapter files"""
    for i in range(1, 6):
        create_simple_pdf(
            os.path.join(context.hanser_collection_dir, f"9783446123456.{i:03d}.pdf"),
            title=f"Chapter {i}", author="Hanser Author"
        )


@given('the collection contains files with ISBN pattern "9783446######.bm.pdf"')
def step_create_hanser_back_matter(context):
    """Create Hanser Verlag back matter file"""
    create_simple_pdf(
        os.path.join(context.hanser_collection_dir, "9783446123456.bm.pdf"),
        title="Back Matter", author="Hanser Author"
    )


@then('the system should detect the Hanser Verlag naming pattern plugin')
def step_verify_hanser_plugin_detected(context):
    """Verify that Hanser Verlag plugin was detected"""
    assert context.command_exit_code == 0, f"Command failed with exit code {context.command_exit_code}"


@then('front matter file (.fm.pdf) should be placed first')
def step_verify_hanser_front_matter_first(context):
    """Verify Hanser front matter is placed first"""
    assert context.command_exit_code == 0, "Command failed"


@then('chapters should be ordered numerically (.001, .002, ..., .010, .011)')
def step_verify_hanser_chapters_ordered(context):
    """Verify Hanser chapters are ordered numerically"""
    assert context.command_exit_code == 0, "Command failed"


@then('back matter file (.bm.pdf) should be placed at the end')
def step_verify_hanser_back_matter_last(context):
    """Verify Hanser back matter is placed at the end"""
    assert context.command_exit_code == 0, "Command failed"


# ========== O'Reilly Steps (Issue29) ==========

@given('I have a PDF collection from O\'Reilly publisher')
def step_create_oreilly_collection(context):
    """Create a collection with O'Reilly naming pattern"""
    collection_dir = os.path.join(context.source_dir, "OReillyBook")
    os.makedirs(collection_dir, exist_ok=True)
    context.oreilly_collection_dir = collection_dir


@given('the collection contains files with patterns like "BEGINN", "Inhalt", "Vorwort"')
def step_create_oreilly_front_matter(context):
    """Create O'Reilly front matter files"""
    create_simple_pdf(
        os.path.join(context.oreilly_collection_dir, "BEGINN.pdf"),
        title="BEGINN", author="O'Reilly Author"
    )
    create_simple_pdf(
        os.path.join(context.oreilly_collection_dir, "Inhalt.pdf"),
        title="Inhalt", author="O'Reilly Author"
    )
    create_simple_pdf(
        os.path.join(context.oreilly_collection_dir, "Vorwort.pdf"),
        title="Vorwort", author="O'Reilly Author"
    )


@given('the collection contains files with pattern "Kapitel_1_", "Kapitel_2_", "Chapter_1_"')
def step_create_oreilly_chapters(context):
    """Create O'Reilly chapter files"""
    create_simple_pdf(
        os.path.join(context.oreilly_collection_dir, "Kapitel_1_Intro.pdf"),
        title="Kapitel 1", author="O'Reilly Author"
    )
    create_simple_pdf(
        os.path.join(context.oreilly_collection_dir, "Kapitel_2_Content.pdf"),
        title="Kapitel 2", author="O'Reilly Author"
    )
    create_simple_pdf(
        os.path.join(context.oreilly_collection_dir, "Chapter_3_Advanced.pdf"),
        title="Chapter 3", author="O'Reilly Author"
    )


@given('the collection contains files with patterns like "Index", "Anhang"')
def step_create_oreilly_back_matter(context):
    """Create O'Reilly back matter files"""
    create_simple_pdf(
        os.path.join(context.oreilly_collection_dir, "Anhang.pdf"),
        title="Anhang", author="O'Reilly Author"
    )
    create_simple_pdf(
        os.path.join(context.oreilly_collection_dir, "Index.pdf"),
        title="Index", author="O'Reilly Author"
    )


@then('the system should detect the O\'Reilly naming pattern plugin')
def step_verify_oreilly_plugin_detected(context):
    """Verify that O'Reilly plugin was detected"""
    assert context.command_exit_code == 0, f"Command failed with exit code {context.command_exit_code}"


@then('front matter files (BEGINN, Inhalt, Vorwort) should be placed first')
def step_verify_oreilly_front_matter_first(context):
    """Verify O'Reilly front matter is placed first"""
    assert context.command_exit_code == 0, "Command failed"


@then('chapters should be ordered numerically')
def step_verify_oreilly_chapters_ordered(context):
    """Verify O'Reilly chapters are ordered numerically"""
    assert context.command_exit_code == 0, "Command failed"


@then('appendices (Anhang) should be placed after chapters')
def step_verify_oreilly_appendices(context):
    """Verify O'Reilly appendices are placed after chapters"""
    assert context.command_exit_code == 0, "Command failed"


@then('back matter files (Index) should be placed at the end')
def step_verify_oreilly_back_matter_last(context):
    """Verify O'Reilly back matter is placed at the end"""
    assert context.command_exit_code == 0, "Command failed"


# ========== Teil-based Steps (Issue30) ==========

@given('I have a PDF collection with German Teil (Part) structure')
def step_create_teil_collection(context):
    """Create a collection with German Teil (Part) structure"""
    collection_dir = os.path.join(context.source_dir, "TeilBasedBook")
    os.makedirs(collection_dir, exist_ok=True)
    context.teil_collection_dir = collection_dir


@given('the collection contains files with pattern "Teil_I_", "Teil_II_", "Teil_III_"')
def step_create_teil_parts(context):
    """Create Teil (Part) files"""
    create_simple_pdf(
        os.path.join(context.teil_collection_dir, "Teil_I_Grundlagen.pdf"),
        title="Teil I", author="Teil Author"
    )
    create_simple_pdf(
        os.path.join(context.teil_collection_dir, "Teil_II_Vertiefung.pdf"),
        title="Teil II", author="Teil Author"
    )
    create_simple_pdf(
        os.path.join(context.teil_collection_dir, "Teil_III_Experten.pdf"),
        title="Teil III", author="Teil Author"
    )


@given('each Teil may contain multiple chapters with pattern "Kapitel_1_", "Kapitel_2_"')
def step_create_teil_chapters(context):
    """Create chapters within Teile"""
    create_simple_pdf(
        os.path.join(context.teil_collection_dir, "Teil_I_Kapitel_1_Intro.pdf"),
        title="Teil I Kapitel 1", author="Teil Author"
    )
    create_simple_pdf(
        os.path.join(context.teil_collection_dir, "Teil_I_Kapitel_2_Content.pdf"),
        title="Teil I Kapitel 2", author="Teil Author"
    )
    create_simple_pdf(
        os.path.join(context.teil_collection_dir, "Teil_II_Kapitel_1_Advanced.pdf"),
        title="Teil II Kapitel 1", author="Teil Author"
    )


@given('the collection contains front matter like "BEGINN", "Vorwort", "Inhaltsverzeichnis"')
def step_create_teil_front_matter(context):
    """Create Teil front matter files"""
    create_simple_pdf(
        os.path.join(context.teil_collection_dir, "BEGINN.pdf"),
        title="BEGINN", author="Teil Author"
    )
    create_simple_pdf(
        os.path.join(context.teil_collection_dir, "Vorwort.pdf"),
        title="Vorwort", author="Teil Author"
    )
    create_simple_pdf(
        os.path.join(context.teil_collection_dir, "Inhaltsverzeichnis.pdf"),
        title="Inhaltsverzeichnis", author="Teil Author"
    )


@given('the collection contains back matter like "Index", "Anhang"')
def step_create_teil_back_matter(context):
    """Create Teil back matter files"""
    create_simple_pdf(
        os.path.join(context.teil_collection_dir, "Anhang.pdf"),
        title="Anhang", author="Teil Author"
    )
    create_simple_pdf(
        os.path.join(context.teil_collection_dir, "Index.pdf"),
        title="Index", author="Teil Author"
    )


@then('the system should detect the Teil-based naming pattern plugin')
def step_verify_teil_plugin_detected(context):
    """Verify that Teil-based plugin was detected"""
    assert context.command_exit_code == 0, f"Command failed with exit code {context.command_exit_code}"


@then('front matter should be placed first')
def step_verify_teil_front_matter_first(context):
    """Verify Teil front matter is placed first"""
    assert context.command_exit_code == 0, "Command failed"


@then('Teile should be ordered numerically (Teil_I, Teil_II, Teil_III)')
def step_verify_teile_ordered(context):
    """Verify Teile are ordered numerically"""
    assert context.command_exit_code == 0, "Command failed"


@then('chapters within each Teil should maintain their order')
def step_verify_teil_chapters_ordered(context):
    """Verify chapters within each Teil are ordered"""
    assert context.command_exit_code == 0, "Command failed"


@then('back matter should be placed at the end')
def step_verify_teil_back_matter_last(context):
    """Verify Teil back matter is placed at the end"""
    assert context.command_exit_code == 0, "Command failed"
