import sys
from pathlib import Path
from typing import List


class FileSystemHandler:
    """Handles file system operations"""

    @staticmethod
    def get_base_dir() -> Path:
        """Get the base directory for the application"""
        return Path(sys.executable).parent if getattr(sys, 'frozen', False) else Path.cwd()

    @staticmethod
    def get_available_playlists(base_dir: Path) -> List[str]:
        """Get list of available playlists"""
        return [
            p.name for p in base_dir.iterdir()
            if p.is_dir() and any(p.glob("*.spotdl"))
        ]

    @staticmethod
    def write_songs_list(playlist_path: Path) -> None:
        """Write list of all music files in the directory"""
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
