import json
import logging
from pathlib import Path
from typing import Dict, Optional

from program.config import Config
from program.file_system_handler import FileSystemHandler
from program.genre_processor import GenreProcessor
from program.models import ProcessingState, SongMetadata
from program.progress_bar import ProgressDisplay
from program.string_cleaner import StringCleaner


class PlaylistProcessor:
    """Handles all playlist operations including metadata management, processing, and genre fixes."""

    def __init__(self, config: Config):
        """Initialize PlaylistManager with configuration."""
        self.config = config
        self.genre_processor = GenreProcessor(config)

    def process_playlist(self, playlist_path: Path) -> None:
        """
        Process all songs in a playlist including metadata and genre fixes.

        Args:
            playlist_path: Path to the playlist directory
        """
        # Setup logging
        self.log_file = playlist_path / "processing_results.log"
        self.logger = self._setup_logger(playlist_path)

        processing_state = ProcessingState(set(), set(), set())

        # Load metadata from spotdl file if available
        metadata_lookup = self._load_playlist_metadata(playlist_path)

        # Process all music files
        self._process_music_files(
            playlist_path=playlist_path,
            metadata_lookup=metadata_lookup,
            processing_state=processing_state
        )

        # Log final processing summaries
        self._log_processing_summaries(processing_state)

        # Print final summary to console
        total_songs = len(FileSystemHandler.get_music_files(playlist_path))
        print("\nProcessing Complete!")
        print(f"- Detailed log saved to: {self.log_file}")
        print(f"- Total songs processed: {total_songs}")
        print(f"- Songs without styles: {len(processing_state.songs_without_styles)}")
        print(f"- Songs without metadata: {len(processing_state.songs_without_metadata)}")
        if processing_state.unmapped_styles:
            print(f"- Found {len(processing_state.unmapped_styles)} unmapped styles")
            print("  Check the log file for details")

    def _setup_logger(self, playlist_path: Path) -> logging.Logger:
        """Setup logger for file output only"""
        # Create logger
        logger = logging.getLogger(f'playlist_processor_{playlist_path.name}')
        logger.setLevel(logging.INFO)

        # Remove any existing handlers to avoid duplicates
        logger.handlers.clear()

        # Create file handler
        file_handler = logging.FileHandler(self.log_file, mode='w', encoding='utf-8')
        file_handler.setLevel(logging.INFO)

        # Create formatter and add it to the handler
        file_formatter = logging.Formatter('%(message)s')  # Simplified format for better readability
        file_handler.setFormatter(file_formatter)

        # Add handler to logger
        logger.addHandler(file_handler)

        return logger

    def _log_processing_summaries(self, processing_state: ProcessingState) -> None:
        """Log processing summary information"""
        if processing_state.unmapped_styles:
            self.logger.info("\nStyles without genre mappings:")
            for style in sorted(processing_state.unmapped_styles):
                self.logger.info(f"- {style}")
            self.logger.info("\nTo map these styles, add them to the genre_styles.json file.")

        if processing_state.songs_without_styles:
            self.logger.warning("\nSongs without any styles:")
            for song in sorted(processing_state.songs_without_styles):
                self.logger.warning(f"- {song}")

        if processing_state.songs_without_metadata:
            self.logger.error("\nSongs without metadata:")
            for song in sorted(processing_state.songs_without_metadata):
                self.logger.error(f"- {song}")

    def _load_playlist_metadata(self, playlist_path: Path) -> Dict[str, SongMetadata]:
        """Load and parse playlist metadata from spotdl file."""
        metadata_lookup = {}
        spotdl_file = FileSystemHandler.find_spotdl_file(playlist_path)

        if not spotdl_file:
            self.logger.warning("No spotdl file found")
            return metadata_lookup

        try:
            with open(spotdl_file, 'r', encoding='utf-8') as f:
                playlist_data = json.load(f)

            for song in playlist_data['songs']:
                metadata = self._get_song_metadata_from_spotdl(song)
                self._add_metadata_lookups(metadata_lookup, metadata, song)

            self.logger.info(f"Loaded metadata for {len(metadata_lookup)} songs from spotdl file")
            return metadata_lookup

        except Exception as e:
            self.logger.error(f"Error reading playlist file: {e}")
            return metadata_lookup

    def _get_song_metadata_from_spotdl(self, song: dict) -> SongMetadata:
        """Create SongMetadata from song dict."""
        return SongMetadata(
            artists=song['artist'],
            name=song['name'],
            genres=[StringCleaner.capitalize_genre(genre) for genre in song.get('genres', [])]
        )

    def _add_metadata_lookups(self, metadata_lookup: dict, metadata: SongMetadata, song: dict) -> None:
        """Add metadata lookups for different artist combinations."""
        key = f"{', '.join(StringCleaner.clean_name(artist) for artist in song['artists'])} - {StringCleaner.clean_name(song['name'])}"
        metadata_lookup[key] = metadata

    def _process_music_files(self, playlist_path: Path, metadata_lookup: Dict[str, SongMetadata], processing_state: ProcessingState) -> None:
        """Process all music files in the playlist directory."""
        music_files = FileSystemHandler.get_music_files(playlist_path)
        if not music_files:
            self.logger.warning("No music files found in the directory")
            return

        self.logger.info(f"\nProcessing {len(music_files)} music files")
        progress = ProgressDisplay(len(music_files))

        for music_path in music_files:
            self._process_single_file(
                music_path=music_path,
                metadata_lookup=metadata_lookup,
                processing_state=processing_state
            )
            progress.update()

        progress.clear()

    def _process_single_file(self, music_path: Path, metadata_lookup: Dict[str, SongMetadata], processing_state: ProcessingState) -> None:
        """Process a single music file."""
        self.logger.info(f"\nProcessing file: {music_path.name}")
        metadata = self._try_get_spotdl_metadata(music_path, metadata_lookup)

        try:
            if metadata:
                self.logger.info("Using spotdl metadata")
                self.genre_processor.fix_genres_spotdl_song(music_path, metadata, processing_state, logger=self.logger)
            else:
                self.logger.info("Using existing file metadata")
                self.genre_processor.fix_genres_external_song(music_path, processing_state, logger=self.logger)

        except Exception as e:
            self.logger.error(f"Error processing file {music_path.name}: {str(e)}")
            processing_state.songs_without_metadata.add(music_path.name)

    def _try_get_spotdl_metadata(self, music_path: Path, metadata_lookup: Dict[str, SongMetadata]) -> Optional[SongMetadata]:
        """Try to find spotdl metadata for a file using various filename formats."""
        if not metadata_lookup:
            return None

        filename = music_path.stem
        clean_filename = StringCleaner.clean_name(filename)

        metadata = metadata_lookup.get(clean_filename)
        if metadata is None:
            self.logger.debug(f"No metadata found for: {clean_filename}")

        return metadata
