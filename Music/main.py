import sys
from pathlib import Path

from program.config import Config
from program.file_system_handler import FileSystemHandler
from program.playlist_processor import PlaylistProcessor
from program.spotdl_wrapper import SpotDLWrapper


class CLI:
    """Command Line Interface handler"""

    def __init__(self, base_dir: Path):
        self.base_dir = base_dir
        self.config = Config(base_dir)
        self.fs_handler = FileSystemHandler()
        self.spotdl = SpotDLWrapper()
        self.processor = PlaylistProcessor(self.config)

    def run(self) -> None:
        """Main CLI loop"""
        while True:
            self._print_menu()
            choice = input("Choice (1-4): ").strip()

            try:
                self._handle_choice(choice)
            except KeyboardInterrupt:
                print("\nOperation cancelled by user")
            except Exception as e:
                print(f"\nError: {e}")

    def _print_menu(self) -> None:
        """Print the main menu"""
        print("\nSpotify Playlist Sync Tool")
        print("------------------------")
        print("1. First-time sync")
        print("2. Update existing sync")
        print("3. Fix genres")
        print("4. Exit\n")

    def _handle_choice(self, choice: str) -> None:
        """Handle menu choice"""
        actions = {
            "1": self._handle_new_sync,
            "2": self._handle_existing_sync,
            "3": self._handle_fix_genres,
            "4": sys.exit
        }

        if choice in actions:
            actions[choice]()
        else:
            print("Invalid choice!")

    def _handle_new_sync(self) -> None:
        """Handle new playlist sync"""
        playlist_url = input("\nEnter Spotify playlist URL: ").strip()
        playlist_name = input("Enter playlist name: ").strip()
        self.spotdl.new_sync(playlist_url, playlist_name, self.base_dir)

    def _handle_existing_sync(self) -> None:
        """Handle existing playlist sync"""
        playlists = self.fs_handler.get_available_playlists(self.base_dir)
        if playlists:
            print("\nAvailable playlists:\n" + "\n".join(playlists))

        playlist_name = input("\nEnter playlist name: ").strip()
        self.spotdl.update_sync(playlist_name, self.base_dir)

    def _handle_fix_genres(self) -> None:
        """Handle fixing genres"""
        playlists = self.fs_handler.get_available_playlists(self.base_dir)
        if playlists:
            print("\nAvailable playlists:\n" + "\n".join(playlists))

        playlist_name = input("\nEnter playlist name: ").strip()
        playlist_dir = self.base_dir / playlist_name

        if playlist_dir.exists():
            self.processor.process_playlist(playlist_dir)
        else:
            print(f"\nError: Folder not found: {playlist_dir}")


def main() -> None:
    """Main entry point"""
    try:
        base_dir = FileSystemHandler.get_base_dir()
        cli = CLI(base_dir)
        cli.run()
    except KeyboardInterrupt:
        print("\nExiting...")
    except Exception as e:
        print(f"Error: {e}")
        sys.exit(1)


if __name__ == "__main__":
    main()
