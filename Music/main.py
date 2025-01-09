﻿import json
import subprocess
import sys
from pathlib import Path

import mutagen
from mutagen.id3 import ID3, TXXX, TCON

# Map genres to their associated styles
GENRE_STYLES = {
    'Rock': ['rock', 'hard rock', 'metal rock'],
    'Synthwave': ['synthwave', 'darksynth', 'retrowave'],
    'Pop': ['pop', 'dance pop', 'electropop']
}

# Track various issues
unmapped_styles = set()
songs_without_styles = set()
songs_without_metadata = set()


def get_base_dir():
    return Path(sys.executable).parent if getattr(sys, 'frozen', False) else Path.cwd()


def capitalize_genre(genre):
    return ' '.join(word.capitalize() for word in genre.split())


def clean_name(name):
    cleaned = name.lower()
    cleaned = ' '.join(cleaned.split())
    return cleaned


def find_spotdl_file(folder_path):
    spotdl_files = list(folder_path.glob('*.spotdl'))
    if not spotdl_files:
        return None
    if len(spotdl_files) > 1:
        print(f"Warning: Multiple .spotdl files found, using: {spotdl_files[0].name}")
    return spotdl_files[0]


def load_playlist_metadata(playlist_file):
    try:
        with open(playlist_file, 'r', encoding='utf-8') as f:
            playlist_data = json.load(f)

        metadata_lookup = {}
        album_urls = set()

        for song in playlist_data['songs']:
            artist = song['artist']
            name = song['name']

            if 'album_id' in song:
                album_url = f"https://open.spotify.com/album/{song['album_id']}"
                album_urls.add((song['album_name'], album_url))

            if 'genres' in song:
                song['genres'] = [capitalize_genre(genre) for genre in song['genres']]

            clean_key = f"{clean_name(artist)} - {clean_name(name)}"
            metadata_lookup[clean_key] = song

            clean_key_no_remaster = clean_name(f"{artist} - {name}".lower().replace('remastered', '').replace('album version', ''))
            metadata_lookup[clean_key_no_remaster] = song

        return metadata_lookup, album_urls

    except Exception as e:
        print(f"Error reading playlist file: {e}")
        return None, set()


def extract_genres_and_remaining_styles(styles):
    """
    For each style, check if it belongs to a genre group. Keep all original styles except those
    that exactly match their genre name, and collect the mapped genres.
    Returns (genres, filtered_styles)
    """
    genres = set()
    remaining_styles = []

    # Create a lookup from style to genre
    style_to_genre = {style.lower(): genre
                      for genre, style_list in GENRE_STYLES.items()
                      for style in style_list}

    # Check each style to see if it maps to a genre
    for style in styles:
        style_lower = style.lower()
        mapped_genre = style_to_genre.get(style_lower)
        if mapped_genre:
            genres.add(mapped_genre)
            # Only keep the style if it's not exactly the same as its genre
            if style_lower != mapped_genre.lower():
                remaining_styles.append(style)
        else:
            unmapped_styles.add(style)
            remaining_styles.append(style)

    return list(genres), remaining_styles


def fix_genres(song_path, song_metadata):
    styles = song_metadata.get('genres', [])
    print(f"\nProcessing: {song_path.name}")
    print(f"Original styles: {', '.join(styles)}")

    if not styles:
        print("No styles found")
        songs_without_styles.add(song_path.name)
        return

    try:
        try:
            tags = ID3(song_path)
        except mutagen.id3.ID3NoHeaderError:
            tags = ID3()

        # Clear existing genre tags
        tags.delall('TCON')

        # Get genres and remaining styles
        genres, remaining_styles = extract_genres_and_remaining_styles(styles)

        # Set remaining styles
        if remaining_styles:
            frame = TXXX(encoding=3, desc='Styles', text=remaining_styles)
            tags.add(frame)

        # Set genres
        if genres:
            tags.add(TCON(encoding=3, text=genres))
            print(f"Mapped Genres: {', '.join(genres)}")
            print(f"Remaining Styles: {', '.join(remaining_styles)}")
        else:
            print("No genres mapped")
            print(f"Styles unchanged: {', '.join(styles)}")

        tags.save(song_path)

    except Exception as e:
        print(f"Error processing {song_path}: {e}")


def process_folder(folder_path):
    spotdl_file = find_spotdl_file(folder_path)
    if not spotdl_file:
        print(f"Error: No .spotdl file found in: {folder_path}")
        return

    metadata_lookup, _ = load_playlist_metadata(spotdl_file)
    if not metadata_lookup:
        return

    print(f"\nProcessing files in: {folder_path}")

    mp3_files = list(Path(folder_path).rglob('*.mp3'))
    for mp3_path in mp3_files:
        filename = mp3_path.stem
        clean_filename = clean_name(filename)
        clean_filename_no_remaster = clean_name(filename.lower().replace('remastered', '').replace('album version', ''))

        metadata = metadata_lookup.get(clean_filename) or metadata_lookup.get(clean_filename_no_remaster)

        if metadata:
            fix_genres(mp3_path, metadata)
        else:
            print(f"\nNo metadata found for: {mp3_path.name}")
            songs_without_metadata.add(mp3_path.name)

    # Print summary of all issues
    print("\n=== Processing Summary ===")

    if unmapped_styles:
        print("\nStyles without genre mappings:")
        for style in sorted(unmapped_styles):
            print(f"- {style}")
        print("\nTo map these styles, add them to the STYLE_TO_GENRE dictionary at the top of the script.")
    else:
        print("\nAll styles were successfully mapped to genres!")

    if songs_without_styles:
        print("\nSongs without any styles:")
        for song in sorted(songs_without_styles):
            print(f"- {song}")

    if songs_without_metadata:
        print("\nSongs without metadata in the JSON:")
        for song in sorted(songs_without_metadata):
            print(f"- {song}")

    if not (unmapped_styles or songs_without_styles or songs_without_metadata):
        print("\nNo issues found! All songs were processed successfully.")


def handle_new_sync(base_dir):
    playlist_url = input("\nEnter Spotify playlist URL: ").strip()
    playlist_name = input("Enter playlist name: ").strip()
    playlist_dir = base_dir / playlist_name
    playlist_dir.mkdir(exist_ok=True)

    subprocess.run(["spotdl", playlist_url, "--output", str(playlist_dir), "--save-file", str(playlist_dir / f"{playlist_name}.spotdl")])


def handle_existing_sync(base_dir):
    playlists = [p.name for p in base_dir.iterdir() if p.is_dir() and any(p.glob("*.spotdl"))]
    if playlists:
        print("\nAvailable playlists:\n" + "\n".join(playlists))

    playlist_name = input("\nEnter playlist name: ").strip()
    sync_file = base_dir / playlist_name / f"{playlist_name}.spotdl"

    if sync_file.exists():
        subprocess.run(["spotdl", "--sync", str(sync_file), "--output", str(base_dir / playlist_name)])
    else:
        print(f"\nError: Sync file not found for playlist {playlist_name}")


def fix_genres_in_folder(base_dir):
    playlists = [p.name for p in base_dir.iterdir() if p.is_dir() and any(p.glob("*.spotdl"))]
    if playlists:
        print("\nAvailable playlists:\n" + "\n".join(playlists))

    playlist_name = input("\nEnter playlist name: ").strip()
    playlist_dir = base_dir / playlist_name

    if playlist_dir.exists():
        process_folder(playlist_dir)
    else:
        print(f"\nError: Folder not found: {playlist_dir}")


def main():
    base_dir = get_base_dir()
    while True:
        print("\nSpotify Playlist Sync Tool")
        print("------------------------")
        print("1. First-time sync")
        print("2. Update existing sync")
        print("3. Fix genres")
        print("4. Exit\n")

        choice = input("Choice (1-4): ").strip()

        if choice == "1":
            handle_new_sync(base_dir)
        elif choice == "2":
            handle_existing_sync(base_dir)
        elif choice == "3":
            fix_genres_in_folder(base_dir)
        elif choice == "4":
            break
        else:
            print("Invalid choice!")


if __name__ == "__main__":
    main()