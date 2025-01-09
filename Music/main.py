import os
import sys
from pathlib import Path
import subprocess


def get_base_dir():
    return Path(sys.executable).parent if getattr(sys, 'frozen', False) else Path.cwd()


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


def main():
    base_dir = get_base_dir()
    while True:
        print("\nSpotify Playlist Sync Tool")
        print("------------------------")
        print("1. First-time sync")
        print("2. Update existing sync")
        print("3. Exit\n")

        choice = input("Choice (1-3): ").strip()

        if choice == "1":
            handle_new_sync(base_dir)
        elif choice == "2":
            handle_existing_sync(base_dir)
        elif choice == "3":
            break
        else:
            print("Invalid choice!")


if __name__ == "__main__":
    main()
