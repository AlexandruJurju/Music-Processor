import json
import re
import subprocess
import sys
from collections import namedtuple
from pathlib import Path
from typing import Dict, List, Optional, Set, Tuple

import mutagen
from mutagen.id3 import ID3, TXXX, TCON

# Type definitions
SongMetadata = namedtuple('SongMetadata', ['artist', 'name', 'album_name', 'album_id', 'genres'])
ProcessingState = namedtuple('ProcessingState', ['unmapped_styles', 'songs_without_styles', 'songs_without_metadata'])

# Default mappings
DEFAULT_GENRES = {
    'Rock': ['rock', 'alternative rock'],
    'Synthwave': ['synthwave', 'darksynth'],
    'Pop': ['pop', 'dance pop']
}

# Default styles to remove unconditionally (regardless of genre)
DEFAULT_REMOVE_STYLES_ONLY = [
    'britpop',
    'madchester'
]


def get_base_dir() -> Path:
    """Get the base directory for the application"""
    return Path(sys.executable).parent if getattr(sys, 'frozen', False) else Path.cwd()


def clean_name(name: str) -> str:
    """Clean and normalize a name string by keeping only alphanumeric and spaces"""
    cleaned = name.lower().replace('&', 'and')
    cleaned = re.sub(r'[^a-z0-9\s]', '', cleaned)
    return ' '.join(cleaned.split())


def capitalize_genre(genre: str) -> str:
    """Capitalize each word in a genre name"""
    return ' '.join(word.capitalize() for word in genre.split())


def load_genre_mappings(base_dir: Path) -> Dict[str, List[str]]:
    """Load genre mappings from JSON file or create default if not exists"""
    mapping_file = base_dir / "genre_styles.json"
    print(f"\nSearching for genre mappings in: {mapping_file}")

    try:
        if not mapping_file.exists():
            with open(mapping_file, 'w', encoding='utf-8') as f:
                json.dump(DEFAULT_GENRES, f, indent=4)
            print(f"Created default genre mappings in: {mapping_file}")
            return DEFAULT_GENRES

        with open(mapping_file, 'r', encoding='utf-8') as f:
            return json.load(f)

    except Exception as e:
        print(f"\nError loading genre mappings: {e}")
        print("Using default mappings")
        return DEFAULT_GENRES


def load_remove_styles_only(base_dir: Path) -> List[str]:
    """Load remove styles only list from JSON file or create default if not exists"""
    mapping_file = base_dir / "remove_styles_only.json"
    print(f"\nSearching for remove styles only in: {mapping_file}")

    try:
        if not mapping_file.exists():
            with open(mapping_file, 'w', encoding='utf-8') as f:
                json.dump(DEFAULT_REMOVE_STYLES_ONLY, f, indent=4)
            print(f"Created default remove styles only in: {mapping_file}")
            return DEFAULT_REMOVE_STYLES_ONLY

        with open(mapping_file, 'r', encoding='utf-8') as f:
            return json.load(f)

    except Exception as e:
        print(f"\nError loading remove styles only: {e}")
        print("Using default mappings")
        return DEFAULT_REMOVE_STYLES_ONLY


def find_spotdl_file(folder_path: Path) -> Optional[Path]:
    """Find the spotdl file for the playlist"""
    spotdl_files = list(folder_path.glob('*.spotdl'))
    if not spotdl_files:
        return None
    if len(spotdl_files) > 1:
        print(f"Warning: Multiple .spotdl files found, using: {spotdl_files[0].name}")
    return spotdl_files[0]


def load_playlist_metadata(playlist_file: Path) -> Tuple[Dict[str, SongMetadata], Set[Tuple[str, str]]]:
    """Load and parse playlist metadata"""
    try:
        with open(playlist_file, 'r', encoding='utf-8') as f:
            playlist_data = json.load(f)

        metadata_lookup = {}
        album_urls = set()

        for song in playlist_data['songs']:
            artists = song.get('artists', [song['artist']])
            name = song['name']
            album_name = song.get('album_name')
            album_id = song.get('album_id')
            genres = [capitalize_genre(genre) for genre in song.get('genres', [])]

            metadata = SongMetadata(
                artist=song['artist'],
                name=name,
                album_name=album_name,
                album_id=album_id,
                genres=genres
            )

            if album_id:
                album_url = f"https://open.spotify.com/album/{album_id}"
                album_urls.add((album_name, album_url))

            # Store both single artist and multiple artists versions
            # Single artist version
            clean_key = f"{clean_name(song['artist'])} - {clean_name(name)}"
            metadata_lookup[clean_key] = metadata
            clean_key_alt = clean_name(f"{song['artist']} - {name}".replace('remastered', '').replace('album version', ''))
            metadata_lookup[clean_key_alt] = metadata

            # Multiple artists version (if exists)
            if len(artists) > 1:
                artists_str = ', '.join(artists)
                collab_key = f"{clean_name(artists_str)} - {clean_name(name)}"
                metadata_lookup[collab_key] = metadata
                collab_key_alt = clean_name(f"{artists_str} - {name}".replace('remastered', '').replace('album version', ''))
                metadata_lookup[collab_key_alt] = metadata

        return metadata_lookup, album_urls

    except Exception as e:
        print(f"\nError reading playlist file: {e}")
        return {}, set()


def extract_genres_and_styles(styles: List[str], genre_mappings: Dict[str, List[str]],
                            remove_styles_only: List[str],
                            unmapped_styles: Set[str]) -> Tuple[List[str], List[str]]:
    """Extract genres and remaining styles from a list of styles"""
    genres = set()
    remaining_styles = []

    # Create style to genre lookup
    style_to_genre = {
        style.lower(): genre
        for genre, style_list in genre_mappings.items()
        for style in style_list
    }

    # Convert remove_styles_only to lowercase set for efficient lookup
    remove_styles_only_set = {style.lower() for style in remove_styles_only}

    for style in styles:
        style_lower = style.lower()

        # Skip if style is in remove_styles_only list
        if style_lower in remove_styles_only_set:
            continue

        mapped_genre = style_to_genre.get(style_lower)

        if mapped_genre:
            genres.add(mapped_genre)
            # Keep style if it's not the same as its genre
            if style_lower != mapped_genre.lower():
                remaining_styles.append(style)
        else:
            unmapped_styles.add(style)
            remaining_styles.append(style)

    return list(genres), remaining_styles


def fix_genres(song_path: Path, metadata: SongMetadata, genre_mappings: Dict[str, List[str]],
               remove_styles_only: List[str],
               processing_state: ProcessingState) -> None:
    """Update genre and style tags for a song"""
    styles = metadata.genres or []
    print(f"\nProcessing: {song_path.name}")
    print(f"Original styles: {', '.join(styles)}")

    if not styles:
        print("No styles found")
        processing_state.songs_without_styles.add(song_path.name)
        return

    try:
        try:
            tags = ID3(song_path)
        except mutagen.id3.ID3NoHeaderError:
            tags = ID3()

        tags.delall('TCON')  # Clear existing genre tags

        genres, remaining_styles = extract_genres_and_styles(
            styles, genre_mappings, remove_styles_only,
            processing_state.unmapped_styles)

        if remaining_styles:
            tags.add(TXXX(encoding=3, desc='Styles', text=remaining_styles))

        if genres:
            tags.add(TCON(encoding=3, text=genres))
            print(f"Mapped Genres: {', '.join(genres)}")
            print(f"Remaining Styles: {', '.join(remaining_styles)}")

            # Show which styles were removed
            removed_styles = [s for s in styles if s not in remaining_styles and s.lower() not in [g.lower() for g in genres]]
            if removed_styles:
                print(f"Removed styles: {', '.join(removed_styles)}")
        else:
            print("No genres mapped")
            print(f"Styles unchanged: {', '.join(styles)}")

        tags.save(song_path)

    except Exception as e:
        print(f"Error processing {song_path}: {e}")


def process_playlist(playlist_path: Path, genre_mappings: Dict[str, List[str]]) -> None:
    """Process all songs in a playlist"""
    
    write_songs_list(playlist_path)
    
    spotdl_file = find_spotdl_file(playlist_path)
    if not spotdl_file:
        print(f"\nError: No .spotdl file found in: {playlist_path}")
        return

    metadata_lookup, _ = load_playlist_metadata(spotdl_file)
    if not metadata_lookup:
        return

    remove_styles_only = load_remove_styles_only(get_base_dir())
    processing_state = ProcessingState(set(), set(), set())

    print(f"\nProcessing files in: {playlist_path}")

    for mp3_path in playlist_path.rglob('*.mp3'):
        filename = mp3_path.stem
        clean_filename = clean_name(filename)
        clean_filename_alt = clean_name(
            filename.lower()
            .replace('remastered', '')
            .replace('album version', '')
        )

        metadata = metadata_lookup.get(clean_filename) or metadata_lookup.get(clean_filename_alt)

        if metadata:
            fix_genres(mp3_path, metadata, genre_mappings, remove_styles_only, processing_state)
        else:
            print(f"\nNo metadata found for: {mp3_path.name}")
            processing_state.songs_without_metadata.add(mp3_path.name)

    print_summary(processing_state)


def print_summary(state: ProcessingState) -> None:
    """Print summary of processing issues"""
    print("\n=== Processing Summary ===")

    if state.unmapped_styles:
        print("\nStyles without genre mappings:")
        for style in sorted(state.unmapped_styles):
            print(f"- {style}")
        print("\nTo map these styles, add them to the genre_styles.json file.")
    else:
        print("\nAll styles were successfully mapped to genres!")

    if state.songs_without_styles:
        print("\nSongs without any styles:")
        for song in sorted(state.songs_without_styles):
            print(f"- {song}")

    if state.songs_without_metadata:
        print("\nSongs without metadata in the JSON:")
        for song in sorted(state.songs_without_metadata):
            print(f"- {song}")

    if not (state.unmapped_styles or state.songs_without_styles or state.songs_without_metadata):
        print("\nNo issues found! All songs were processed successfully.")


def run_spotdl_command(command: List[str]) -> None:
    """Run a spotdl command"""
    try:
        subprocess.run(command, check=True)
    except subprocess.CalledProcessError as e:
        print(f"Error running spotdl command: {e}")


def get_available_playlists(base_dir: Path) -> List[str]:
    """Get list of available playlists"""
    return [
        p.name for p in base_dir.iterdir()
        if p.is_dir() and any(p.glob("*.spotdl"))
    ]


def handle_new_sync(base_dir: Path) -> None:
    """Handle first-time playlist sync"""
    playlist_url = input("\nEnter Spotify playlist URL: ").strip()
    playlist_name = input("Enter playlist name: ").strip()
    playlist_dir = base_dir / playlist_name
    playlist_dir.mkdir(exist_ok=True)

    run_spotdl_command([
        "spotdl",
        playlist_url,
        "--output", str(playlist_dir),
        "--save-file", str(playlist_dir / f"{playlist_name}.spotdl")
    ])


def handle_existing_sync(base_dir: Path) -> None:
    """Handle updating an existing playlist"""
    playlists = get_available_playlists(base_dir)
    if playlists:
        print("\nAvailable playlists:\n" + "\n".join(playlists))

    playlist_name = input("\nEnter playlist name: ").strip()
    sync_file = base_dir / playlist_name / f"{playlist_name}.spotdl"

    if sync_file.exists():
        run_spotdl_command([
            "spotdl",
            "--sync", str(sync_file),
            "--output", str(base_dir / playlist_name)
        ])
    else:
        print(f"\nError: Sync file not found for playlist {playlist_name}")


def handle_fix_genres(base_dir: Path, genre_mappings: Dict[str, List[str]]) -> None:
    """Handle fixing genres for a playlist"""
    playlists = get_available_playlists(base_dir)
    if playlists:
        print("\nAvailable playlists:\n" + "\n".join(playlists))

    playlist_name = input("\nEnter playlist name: ").strip()
    playlist_dir = base_dir / playlist_name

    if playlist_dir.exists():
        process_playlist(playlist_dir, genre_mappings)
    else:
        print(f"\nError: Folder not found: {playlist_dir}")

def write_songs_list(playlist_path: Path) -> None:
    """Write list of all music files in the directory"""
    # Common audio file extensions
    music_extensions = {'.mp3', '.flac', '.m4a', '.wav', '.wma', '.aac', '.ogg'}
    
    try:
        # Get all music files
        music_files = []
        for ext in music_extensions:
            music_files.extend(playlist_path.rglob(f'*{ext}'))
        
        # Write to file
        output_file = playlist_path / "music_files.txt"
        with open(output_file, 'w', encoding='utf-8') as f:
            for file in sorted(music_files):
                f.write(f"{file.name}\n")
                
        print(f"\nMusic files list written to: {output_file}")
        print(f"Total music files: {len(music_files)}")
        
    except Exception as e:
        print(f"\nError writing music files list: {e}")

def main() -> None:
    """Main entry point for the application"""
    base_dir = get_base_dir()
    genre_mappings = load_genre_mappings(base_dir)

    while True:
        print("\nSpotify Playlist Sync Tool")
        print("------------------------")
        print("1. First-time sync")
        print("2. Update existing sync")
        print("3. Fix genres")
        print("4. Exit\n")

        choice = input("Choice (1-4): ").strip()

        actions = {
            "1": lambda: handle_new_sync(base_dir),
            "2": lambda: handle_existing_sync(base_dir),
            "3": lambda: handle_fix_genres(base_dir, genre_mappings),
            "4": sys.exit
        }

        if choice in actions:
            actions[choice]()
        else:
            print("Invalid choice!")


if __name__ == "__main__":
    try:
        main()
    except KeyboardInterrupt:
        print("\nExiting...")
    except Exception as e:
        print(f"Error: {e}")
        sys.exit(1)