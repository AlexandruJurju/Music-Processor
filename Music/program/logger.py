from pathlib import Path
from typing import TextIO


class PlaylistLogger:
    """Handles logging of playlist processing messages to a file"""

    def __init__(self, playlist_path: Path, log_file_name: str):
        """Initialize logger with path to playlist directory"""
        self.log_path = playlist_path.parent / log_file_name
        self.log_file: TextIO | None = None

    def __enter__(self):
        """Context manager entry - opens log file"""
        self.log_file = open(self.log_path, 'w', encoding='utf-8')
        return self

    def __exit__(self, exc_type, exc_val, exc_tb):
        """Context manager exit - closes log file"""
        if self.log_file:
            self.log_file.close()
            self.log_file = None

    def log(self, message: str) -> None:
        """Write message to log file"""
        if self.log_file:
            self.log_file.write(message + '\n')

    def log_unmapped_styles(self, unmapped_styles: set[str]) -> None:
        """Log unmapped styles summary"""
        if unmapped_styles:
            self.log("\nStyles without genre mappings:")
            for style in sorted(unmapped_styles):
                self.log(f"- {style}")
            self.log("\nTo map these styles, add them to the genre_styles.json file.")

    def log_songs_without_styles(self, songs: set[str]) -> None:
        """Log songs without styles summary"""
        if songs:
            self.log("\nSongs without any styles:")
            for song in sorted(songs):
                self.log(f"- {song}")

    def log_songs_without_metadata(self, songs: set[str]) -> None:
        """Log songs without metadata summary"""
        if songs:
            self.log("\nSongs without metadata:")
            for song in sorted(songs):
                self.log(f"- {song}")

    def get_log_path(self) -> Path:
        """Return the path to the log file"""
        return self.log_path
