import json
import mutagen
from mutagen.id3 import ID3, TCON, TXXX
from pathlib import Path
import sys
import os

def capitalize_genre(genre):
    """
    Capitalize each word in a genre name.
    """
    return ' '.join(word.capitalize() for word in genre.split())

def find_spotdl_file(folder_path):
    """
    Find a .spotdl file in the given folder.
    Returns the path to the first .spotdl file found, or None if none exist.
    """
    spotdl_files = list(folder_path.glob('*.spotdl'))
    if not spotdl_files:
        return None
    if len(spotdl_files) > 1:
        print(f"Warning: Multiple .spotdl files found, using: {spotdl_files[0].name}")
    return spotdl_files[0]

def load_playlist_metadata(playlist_file):
    """
    Load metadata from the playlist's .spotdl file.
    Returns a dictionary mapping song names and artists to their metadata.
    """
    try:
        with open(playlist_file, 'r', encoding='utf-8') as f:
            playlist_data = json.load(f)
            
        # Create a lookup dictionary mapping "artist - name" to metadata
        metadata_lookup = {}
        for song in playlist_data['songs']:
            # Create keys in different formats to increase matching chances
            artist = song['artist']
            name = song['name']
            
            # Capitalize all genres
            if 'genres' in song:
                song['genres'] = [capitalize_genre(genre) for genre in song['genres']]
            
            # Store with different key formats
            clean_key = f"{clean_name(artist)} - {clean_name(name)}"
            metadata_lookup[clean_key] = song
            
            # Also store a version without 'remastered' for better matching
            clean_key_no_remaster = clean_name(f"{artist} - {name}".lower().replace('remastered', '').replace('album version', ''))
            metadata_lookup[clean_key_no_remaster] = song
            
        return metadata_lookup
            
    except FileNotFoundError:
        print(f"Error: Could not find playlist file: {playlist_file}")
        return None
    except json.JSONDecodeError:
        print(f"Error: Invalid JSON in playlist file: {playlist_file}")
        return None
    except KeyError as e:
        print(f"Error: Unexpected playlist file format - missing key: {e}")
        return None

def fix_genres(song_path, song_metadata):
    """
    Remove genre metadata while preserving styles for a single song using the provided metadata.
    """
    # Get genres from metadata
    genres = song_metadata.get('genres', [])
    if not genres:
        print(f"No genres found for: {song_path.name}")
        return

    try:
        # Load or create ID3 tags
        try:
            tags = ID3(song_path)
        except mutagen.id3.ID3NoHeaderError:
            tags = ID3()

        # Remove existing genre tags but keep styles
        tags.delall('TCON')
        
        # Store all genres as styles
        if genres:
            # Join with semicolons for display purposes only
            display_styles = '; '.join(genres)
            
            # Add styles as separate text values
            frame = TXXX(encoding=3, desc='Styles', text=genres)
            tags.add(frame)
            
            print(f"Successfully updated metadata for: {song_path.name}")
            print(f"Main Genre: None")
            print(f"Styles: {display_styles}")
        else:
            print(f"No styles to set for: {song_path.name}")
            print(f"Main Genre: None")
            print(f"Styles: None")

        # Save changes
        tags.save(song_path)

    except Exception as e:
        print(f"Error processing {song_path}: {e}")
        
def process_folder(folder_path):
    """
    Process all MP3 files in the given folder using the playlist metadata.
    """
    folder_path = Path(folder_path)
    if not folder_path.exists():
        print(f"Error: Folder not found: {folder_path}")
        return

    # Find .spotdl file in the folder
    spotdl_file = find_spotdl_file(folder_path)
    if not spotdl_file:
        print(f"Error: No .spotdl file found in: {folder_path}")
        return

    print(f"\nProcessing folder: {folder_path}")
    print(f"Using playlist file: {spotdl_file.name}\n")

    # Load the playlist metadata
    metadata_lookup = load_playlist_metadata(spotdl_file)
    if not metadata_lookup:
        return

    # Get all MP3 files
    mp3_files = list(folder_path.glob('*.mp3'))
    if not mp3_files:
        print("No MP3 files found in the folder.")
        return

    processed = 0
    skipped = 0
    not_found = []
    
    for mp3_path in mp3_files:
        # Get the song name without extension
        filename = mp3_path.stem
        clean_filename = clean_name(filename)
        clean_filename_no_remaster = clean_name(filename.lower().replace('remastered', '').replace('album version', ''))
        
        # Try to find metadata for this song
        metadata = metadata_lookup.get(clean_filename) or metadata_lookup.get(clean_filename_no_remaster)
        
        if metadata:
            fix_genres(mp3_path, metadata)
            processed += 1
        else:
            skipped += 1
            not_found.append(mp3_path.name)
            print(f"Skipped {mp3_path.name}: No matching metadata found in playlist file")
    
    print(f"\nSummary:")
    print(f"Processed: {processed} files")
    print(f"Skipped: {skipped} files")
    
    if skipped > 0:
        print("\nUnmatched files:")
        for name in not_found:
            clean = clean_name(name)
            print(f"Looking for: '{clean}'")
        print("\nAvailable metadata keys:")
        for key in sorted(metadata_lookup.keys())[:10]:  # Show first 10 keys
            print(f"Have metadata for: '{key}'")

def clean_name(name):
    """
    Clean a song name for comparison by removing special characters and converting to lowercase.
    """
    # Convert to lowercase
    cleaned = name.lower()
    # Remove extra spaces
    cleaned = ' '.join(cleaned.split())
    return cleaned

def main():
    if len(sys.argv) != 2:
        print("Usage: py fix_genres.py <folder_path>")
        print("Example: py fix_genres.py \"X:\\Downloads\\Music\\Rock\"")
        print("\nNote: A .spotdl file must exist in the specified folder")
        sys.exit(1)

    folder_path = sys.argv[1]
    process_folder(folder_path)

if __name__ == "__main__":
    main()