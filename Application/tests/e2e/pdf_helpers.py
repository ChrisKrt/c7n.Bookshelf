"""
Helper utilities for creating test PDF files
"""
import os
from pathlib import Path
from reportlab.pdfgen import canvas
from reportlab.lib.pagesizes import letter
from pypdf import PdfReader, PdfWriter


def create_simple_pdf(file_path: str, title: str = "", author: str = "", pages: int = 1):
    """
    Creates a simple PDF file with the given number of pages
    
    Args:
        file_path: The path where the PDF should be created
        title: The PDF title metadata
        author: The PDF author metadata
        pages: The number of pages to create
    """
    os.makedirs(os.path.dirname(file_path), exist_ok=True)
    
    c = canvas.Canvas(file_path, pagesize=letter)
    
    # Set metadata
    if title:
        c.setTitle(title)
    if author:
        c.setAuthor(author)
    
    # Create pages
    for i in range(pages):
        c.drawString(100, 750, f"Page {i + 1}")
        c.drawString(100, 700, f"Title: {title}")
        c.drawString(100, 650, f"Author: {author}")
        if i < pages - 1:
            c.showPage()
    
    c.save()


def count_pdf_pages(file_path: str) -> int:
    """
    Counts the number of pages in a PDF file
    
    Args:
        file_path: The path to the PDF file
        
    Returns:
        The number of pages in the PDF
    """
    reader = PdfReader(file_path)
    return len(reader.pages)


def get_pdf_metadata(file_path: str) -> dict:
    """
    Gets metadata from a PDF file
    
    Args:
        file_path: The path to the PDF file
        
    Returns:
        A dictionary containing the PDF metadata
    """
    reader = PdfReader(file_path)
    metadata = reader.metadata
    
    if metadata:
        return {
            'title': metadata.get('/Title', ''),
            'author': metadata.get('/Author', ''),
        }
    return {'title': '', 'author': ''}


def list_files_recursively(directory: str) -> list:
    """
    Lists all files recursively in a directory
    
    Args:
        directory: The directory to search
        
    Returns:
        A list of file paths
    """
    files = []
    for root, dirs, filenames in os.walk(directory):
        for filename in filenames:
            files.append(os.path.join(root, filename))
    return sorted(files)
