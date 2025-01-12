import sys
from pathlib import Path
from typing import List, Optional

from program.constants import MUSIC_EXTENSIONS


class FileSystemHandler:
    """Handles file system operations"""

    @staticmethod
    def get_base_dir() -> Path:
        """Get the base directory for the application"""
        return Path(sys.executable).parent if getattr(sys, 'frozen', False) else Path.cwd()

    @staticmethod
    def get_available_playlists(base_dir: Path) -> List[str]:
        """Get list of available spotdl playlists"""
        return [
            p.name for p in base_dir.iterdir()
            if p.is_dir() and any(p.glob("*.spotdl"))
        ]

    @staticmethod
    def write_songs_list(playlist_path: Path) -> None:
        """Write the list of all music files in the directory to a separate txt file"""
        music_extensions = {'.mp3', '.flac', '.m4a', '.wav', '.wma', '.aac', '.ogg'}

        try:
            music_files = []
            for ext in music_extensions:
                music_files.extend(playlist_path.rglob(f'*{ext}'))

            output_file = playlist_path / "music_files.txt"
            with open(output_file, 'w', encoding='utf-8') as f:
                for file in sorted(music_files):
                    f.write(f"{file.name}\n")

            print(f"\nMusic files list written to: {output_file}")
            print(f"Total music files: {len(music_files)}")

        except Exception as e:
            print(f"\nError writing music files list: {e}")

    @staticmethod
    def find_spotdl_file(folder_path: Path) -> Optional[Path]:
        """Find the spotdl file for the playlist"""
        spotdl_files = list(folder_path.glob('*.spotdl'))
        if not spotdl_files:
            return None
        if len(spotdl_files) > 1:
            print(f"Warning: Multiple .spotdl files found, using: {spotdl_files[0].name}")
        return spotdl_files[0]

    @staticmethod
    def get_music_files(playlist_path: Path) -> List[Path]:
        """Get all music files in the directory and subdirectories."""
        music_files = []
        for ext in MUSIC_EXTENSIONS:
            music_files.extend(playlist_path.rglob(f'*{ext}'))
        return sorted(music_files)
