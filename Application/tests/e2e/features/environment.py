"""
Behave environment hooks for setting up and tearing down test fixtures
"""
import os
import shutil
import tempfile
from pathlib import Path


def before_all(context):
    """
    Setup before all tests
    """
    # Find the path to the built CLI executable
    context.cli_path = find_cli_executable()
    if not context.cli_path:
        raise RuntimeError("Could not find Bookshelf.Cli executable. Please build the project first.")
    
    print(f"Using CLI executable: {context.cli_path}")


def before_scenario(context, scenario):
    """
    Setup before each scenario
    """
    # Create temporary directories for test data
    context.temp_dir = tempfile.mkdtemp(prefix="bookshelf_test_")
    context.source_dir = os.path.join(context.temp_dir, "source")
    context.target_dir = os.path.join(context.temp_dir, "target")
    
    os.makedirs(context.source_dir, exist_ok=True)
    os.makedirs(context.target_dir, exist_ok=True)
    
    # Store test data
    context.created_files = []
    context.command_output = None
    context.command_exit_code = None


def after_scenario(context, scenario):
    """
    Cleanup after each scenario
    """
    # Clean up temporary directory
    if hasattr(context, 'temp_dir') and os.path.exists(context.temp_dir):
        try:
            shutil.rmtree(context.temp_dir)
        except Exception as e:
            print(f"Warning: Could not clean up temporary directory: {e}")


def find_cli_executable():
    """
    Finds the built CLI executable
    """
    # Search for the executable in common build output locations
    possible_paths = [
        Path(__file__).parent.parent.parent.parent / "Bookshelf.Cli" / "bin" / "Debug" / "net9.0" / "Bookshelf.Cli",
        Path(__file__).parent.parent.parent.parent / "Bookshelf.Cli" / "bin" / "Release" / "net9.0" / "Bookshelf.Cli",
    ]
    
    for path in possible_paths:
        if path.exists():
            return str(path.absolute())
    
    return None
