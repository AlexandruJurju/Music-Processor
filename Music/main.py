import os
import sys
from pathlib import Path
import subprocess
import json
import mutagen
from mutagen.id3 import ID3, TCON, TXXX


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


def fix_genres(song_path, song_metadata):
    genres = song_metadata.get('genres', [])
    if not genres:
        print(f"No genres found for: {song_path.name}")
        return

    try:
        try:
            tags = ID3(song_path)
        except mutagen.id3.ID3NoHeaderError:
            tags = ID3()

        tags.delall('TCON')

        if genres:
            display_styles = '; '.join(genres)
            frame = TXXX(encoding=3, desc='Styles', text=genres)
            tags.add(frame)
            print(f"Updated metadata for: {song_path.name}")
            print(f"Styles: {display_styles}")
        else:
            print(f"No styles to set for: {song_path.name}")

        tags.save(song_path)

    except Exception as e:
        print(f"Error processing {song_path}: {e}")


def process_folder(folder_path):
    spotdl_file = find_spotdl_file(folder_path)
    if not spotdl_file:
        print(f"Error: No .spotdl file found in: {folder_path}")
        return

    metadata_lookup, album_urls = load_playlist_metadata(spotdl_file)
    if not metadata_lookup:
        return

    mp3_files = list(folder_path.glob('*.mp3'))
    if not mp3_files:
        print("No MP3 files found in the folder.")
        return

    for mp3_path in mp3_files:
        filename = mp3_path.stem
        clean_filename = clean_name(filename)
        clean_filename_no_remaster = clean_name(filename.lower().replace('remastered', '').replace('album version', ''))

        metadata = metadata_lookup.get(clean_filename) or metadata_lookup.get(clean_filename_no_remaster)

        if metadata:
            fix_genres(mp3_path, metadata)
        else:
            print(f"Skipped {mp3_path.name}: No matching metadata found")


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