from pathlib import Path
from typing import Dict, List, Optional

from program.file_system_handler import FileSystemHandler
from program.models import ProcessingState, SongMetadata
from program.config import Config
from program.genre_processor import GenreProcessor
from program.playlist_manager import PlaylistManager
from program.string_cleaner import StringCleaner
from program.logger import PlaylistLogger
from program.progress_bar import ProgressDisplay


class PlaylistProcessor:
    """Handles processing of music playlists including metadata and genre management."""

    MUSIC_EXTENSIONS = {'.mp3', '.flac', '.m4a', '.wav', '.wma', '.aac', '.ogg'}

    def __init__(self, config: Config):
        """Initialize PlaylistProcessor with configuration."""
        self.config = config
        self.playlist_manager = PlaylistManager(config)
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
            metadata_lookup = self._load_metadata(playlist_path)

            # Process all music files
            self._process_music_files(
                playlist_path=playlist_path,
                metadata_lookup=metadata_lookup,
                processing_state=processing_state,
                logger=logger
            )

            # Log final processing summaries
            self._log_processing_summaries(processing_state, logger)
            print(f"\nResults written to: {logger.get_log_path()}")

    def _load_metadata(self, playlist_path: Path) -> Dict[str, SongMetadata]:
        """Load metadata from spotdl file if available."""
        metadata_lookup = {}
        spotdl_file = FileSystemHandler.find_spotdl_file(playlist_path)
        if spotdl_file:
            metadata_lookup = self.playlist_manager.load_playlist_metadata(spotdl_file)
        return metadata_lookup

    def _process_music_files(
            self,
            playlist_path: Path,
            metadata_lookup: Dict[str, SongMetadata],
            processing_state: ProcessingState,
            logger: PlaylistLogger
    ) -> None:
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
            self._process_single_file(
                music_path=music_path,
                metadata_lookup=metadata_lookup,
                processing_state=processing_state,
                logger=logger
            )
            progress.update()

        progress.clear()

    def _process_single_file(
            self,
            music_path: Path,
            metadata_lookup: Dict[str, SongMetadata],
            processing_state: ProcessingState,
            logger: PlaylistLogger
    ) -> None:
        """Process a single music file."""
        metadata = self._try_get_spotdl_metadata(music_path, metadata_lookup)

        try:
            if metadata:
                log_messages = self.genre_processor.fix_genres(
                    music_path, metadata, processing_state
                )
            else:
                log_messages = self.genre_processor.process_existing_metadata(
                    music_path, processing_state
                )

            for message in log_messages:
                logger.log(message)

        except Exception as e:
            logger.log(f"Error processing file {music_path.name}: {str(e)}")
            processing_state.songs_without_metadata.add(music_path.name)

    def _get_music_files(self, playlist_path: Path) -> List[Path]:
        """
        Get all music files in the directory and subdirectories.

        Args:
            playlist_path: Path to playlist directory

        Returns:
            List of paths to music files
        """
        music_files = []
        for ext in self.MUSIC_EXTENSIONS:
            music_files.extend(playlist_path.rglob(f'*{ext}'))
        return sorted(music_files)

    def _try_get_spotdl_metadata(
            self,
            music_path: Path,
            metadata_lookup: Dict[str, SongMetadata]
    ) -> Optional[SongMetadata]:
        """
        Try to find spotdl metadata for a file using various filename formats.

        Args:
            music_path: Path to music file
            metadata_lookup: Dictionary of metadata from spotdl

        Returns:
            SongMetadata if found, None otherwise
        """
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

    def _log_processing_summaries(
            self,
            processing_state: ProcessingState,
            logger: PlaylistLogger
    ) -> None:
        """Log final processing summaries."""
        logger.log_unmapped_styles(processing_state.unmapped_styles)
        logger.log_songs_without_styles(processing_state.songs_without_styles)
        logger.log_songs_without_metadata(processing_state.songs_without_metadata)
