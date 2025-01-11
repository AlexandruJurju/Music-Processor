from pathlib import Path
from typing import TextIO, List

from program.models import ProcessingState


class PlaylistLogger:
    """Handles logging of playlist processing messages to a file"""

    def __init__(self, playlist_path: Path, log_file_name: str):
        """Initialize logger with path to playlist directory"""
        self.log_path = playlist_path.parent / log_file_name
        self.log_file: TextIO | None = None
        self.messages: list[str] = []

    def __enter__(self):
        """Context manager entry"""
        return self

    def __exit__(self, exc_type, exc_val, exc_tb):
        """Context manager exit - writes accumulated messages and closes log file"""
        with open(self.log_path, 'w', encoding='utf-8') as log_file:
            log_file.write('\n'.join(self.messages))
            if self.messages:  # Add final newline only if there are messages
                log_file.write('\n')

    def log(self, message: str) -> None:
        """Add message to accumulated messages"""
        self.messages.append(message)

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

    def log_genre_processing_results(self, original_styles: List[str], genres: List[str], remaining_styles: List[str]) -> None:
        """Log processing results"""
        if genres:
            self.log(f"Mapped Genres: {', '.join(genres)}")
            self.log(f"Remaining Styles: {', '.join(remaining_styles)}")

            removed_styles = [s for s in original_styles if s not in remaining_styles and s.lower() not in [g.lower() for g in genres]]
            if removed_styles:
                self.log(f"Removed styles: {', '.join(removed_styles)}")
        else:
            self.log("No genres mapped")
            self.log(f"Styles unchanged: {', '.join(original_styles)}")

    def log_processing_summaries(self, processing_state: ProcessingState) -> None:
        """Log final processing summaries."""
        self.log_unmapped_styles(processing_state.unmapped_styles)
        self.log_songs_without_styles(processing_state.songs_without_styles)
        self.log_songs_without_metadata(processing_state.songs_without_metadata)
