from pathlib import Path
from typing import Dict, List, Set
import sys
from datetime import datetime

from program.models import ProcessingState, SongMetadata
from program.config import Config
from program.genre_processor import GenreProcessor
from program.playlist_manager import PlaylistManager
from program.string_cleaner import StringCleaner


class PlaylistProcessor:
    """Main playlist processing orchestrator"""
    MUSIC_EXTENSIONS = {'.mp3', '.flac', '.m4a', '.wav', '.wma', '.aac', '.ogg'}

    def __init__(self, config: Config):
        self.config = config
        self.playlist_manager = PlaylistManager(config)
        self.genre_processor = GenreProcessor(config)
        self.total_files = 0
        self.processed_files = 0
        self.log_file = None

    def update_progress(self) -> None:
        """Show only progress bar"""
        self.processed_files += 1
        percentage = (self.processed_files / self.total_files) * 100
        bar_width = 40
        filled = int(bar_width * self.processed_files // self.total_files)
        bar = '█' * filled + '░' * (bar_width - filled)
        sys.stdout.write(f'\rProcessing: [{bar}] {percentage:3.1f}%')
        sys.stdout.flush()

    def log(self, message: str) -> None:
        """Write message to log file"""
        if self.log_file:
            self.log_file.write(message + '\n')

    def process_playlist(self, playlist_path: Path) -> None:
        """Process all songs in a playlist"""
        processing_state = ProcessingState(set(), set(), set())
        self.processed_files = 0

        # Open log file for the entire process
        log_path = playlist_path.parent / "processing_results.txt"
        with open(log_path, 'w', encoding='utf-8') as self.log_file:
            metadata_lookup = {}
            spotdl_file = self.playlist_manager.find_spotdl_file(playlist_path)
            if spotdl_file:
                metadata_lookup, _ = self.playlist_manager.load_playlist_metadata(spotdl_file)

            self._process_music_files(playlist_path, metadata_lookup, processing_state)

            # Clear progress bar
            sys.stdout.write('\r' + ' ' * 100 + '\r')
            print(f"\nResults written to: {log_path}")

    def _process_music_files(self, playlist_path: Path, metadata_lookup: Dict[str, SongMetadata], processing_state: ProcessingState) -> None:
        """Process all music files in the playlist"""
        music_files = self._get_music_files(playlist_path)
        if not music_files:
            self.log("No music files found in the directory")
            return

        self.total_files = len(music_files)

        for music_path in music_files:
            metadata = self._try_get_spotdl_metadata(music_path, metadata_lookup)

            try:
                if metadata:
                    log_messages = self.genre_processor.fix_genres(music_path, metadata, processing_state)
                else:
                    log_messages = self.genre_processor.process_existing_metadata(music_path, processing_state)
                for message in log_messages:
                    self.log(message)
            except Exception as e:
                self.log(f"Error processing file {music_path.name}: {str(e)}")
                processing_state.songs_without_metadata.add(music_path.name)

            self.update_progress()

        # Write summary at end
        if processing_state.unmapped_styles:
            self.log("\nStyles without genre mappings:")
            for style in sorted(processing_state.unmapped_styles):
                self.log(f"- {style}")
            self.log("\nTo map these styles, add them to the genre_styles.json file.")

        if processing_state.songs_without_styles:
            self.log("\nSongs without any styles:")
            for song in sorted(processing_state.songs_without_styles):
                self.log(f"- {song}")

        if processing_state.songs_without_metadata:
            self.log("\nSongs without metadata:")
            for song in sorted(processing_state.songs_without_metadata):
                self.log(f"- {song}")

    def _get_music_files(self, playlist_path: Path) -> List[Path]:
        """Get all music files in the directory"""
        music_files = []
        for ext in self.MUSIC_EXTENSIONS:
            music_files.extend(playlist_path.rglob(f'*{ext}'))
        return sorted(music_files)

    def _try_get_spotdl_metadata(self, music_path: Path, metadata_lookup: Dict[str, SongMetadata]) -> SongMetadata | None:
        """Try to find spotdl metadata for a file"""
        if not metadata_lookup:
            return None

        filename = music_path.stem
        clean_filename = StringCleaner.clean_name(filename)
        clean_filename_alt = StringCleaner.clean_name(
            filename.lower()
            .replace('remastered', '')
            .replace('album version', '')
        )

        return metadata_lookup.get(clean_filename) or metadata_lookup.get(clean_filename_alt)