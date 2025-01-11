from pathlib import Path
from typing import Dict, List, Optional
import json

from program.file_system_handler import FileSystemHandler
from program.models import ProcessingState, SongMetadata
from program.config import Config
from program.genre_processor import GenreProcessor
from program.string_cleaner import StringCleaner
from program.logger import PlaylistLogger
from program.progress_bar import ProgressDisplay


class PlaylistProcessor:
    """Handles all playlist operations including metadata management, processing, and genre fixes."""

    MUSIC_EXTENSIONS = {'.mp3', '.flac', '.m4a', '.wav', '.wma', '.aac', '.ogg'}

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
        processing_state = ProcessingState(set(), set(), set())

        with PlaylistLogger(playlist_path, "processing_results.txt") as logger:
            # Load metadata from spotdl file if available
            metadata_lookup = self._load_playlist_metadata(playlist_path)

            # Process all music files
            self._process_music_files(playlist_path=playlist_path, metadata_lookup=metadata_lookup, processing_state=processing_state, logger=logger)

            # Log final processing summaries
            logger.log_processing_summaries(processing_state)
            print(f"\nResults written to: {logger.get_log_path()}")

    def _load_playlist_metadata(self, playlist_path: Path) -> Dict[str, SongMetadata]:
        """Load and parse playlist metadata from spotdl file."""
        metadata_lookup = {}
        spotdl_file = FileSystemHandler.find_spotdl_file(playlist_path)

        if not spotdl_file:
            return metadata_lookup

        try:
            with open(spotdl_file, 'r', encoding='utf-8') as f:
                playlist_data = json.load(f)

            for song in playlist_data['songs']:
                metadata = self._get_song_metadata_from_spotdl(song)
                self._add_metadata_lookups(metadata_lookup, metadata, song)

            return metadata_lookup

        except Exception as e:
            print(f"\nError reading playlist file: {e}")
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

        # Single artist version
        clean_key = f"{StringCleaner.clean_name(song['artist'])} - {StringCleaner.clean_name(metadata.name)}"
        metadata_lookup[clean_key] = metadata

        ## Multiple artists version
        # if len(artists) > 1:
        #     artists_str = ', '.join(artists)
        #     collab_key = f"{StringCleaner.clean_name(artists_str)} - {StringCleaner.clean_name(metadata.name)}"
        #     metadata_lookup[collab_key] = metadata
        #     collab_key_alt = StringCleaner.clean_name(
        #         f"{artists_str} - {metadata.name}".replace('remastered', '').replace('album version', '')
        #     )
        #     metadata_lookup[collab_key_alt] = metadata

    def _process_music_files(self, playlist_path: Path, metadata_lookup: Dict[str, SongMetadata], processing_state: ProcessingState, logger: PlaylistLogger) -> None:
        """
        Process all music files in the playlist directory.

        Args:
            playlist_path: Path to playlist directory
            metadata_lookup: Dictionary of song metadata from spotdl
            processing_state: Current processing state
            logger: Logger instance for recording results
        """
        music_files = self._get_music_files(playlist_path)
        if not music_files:
            logger.log("No music files found in the directory")
            return

        progress = ProgressDisplay(len(music_files))

        for music_path in music_files:
            self._process_single_file(music_path=music_path, metadata_lookup=metadata_lookup, processing_state=processing_state, logger=logger)
            progress.update()

        progress.clear()

    def _get_music_files(self, playlist_path: Path) -> List[Path]:
        """Get all music files in the directory and subdirectories."""
        music_files = []
        for ext in self.MUSIC_EXTENSIONS:
            music_files.extend(playlist_path.rglob(f'*{ext}'))
        return sorted(music_files)

    def _process_single_file(self, music_path: Path, metadata_lookup: Dict[str, SongMetadata], processing_state: ProcessingState, logger: PlaylistLogger) -> None:
        """Process a single music file."""
        metadata = self._try_get_spotdl_metadata(music_path, metadata_lookup)

        try:
            if metadata:
                self.genre_processor.fix_genres(music_path, metadata, processing_state, logger=logger)
            else:
                self.genre_processor.process_existing_metadata(music_path, processing_state, logger=logger)

        except Exception as e:
            logger.log(f"Error processing file {music_path.name}: {str(e)}")
            processing_state.songs_without_metadata.add(music_path.name)

    def _try_get_spotdl_metadata(self, music_path: Path, metadata_lookup: Dict[str, SongMetadata]) -> Optional[SongMetadata]:
        """Try to find spotdl metadata for a file using various filename formats."""
        if not metadata_lookup:
            return None

        filename = music_path.stem
        clean_filename = StringCleaner.clean_name(filename)

        return metadata_lookup.get(clean_filename)
