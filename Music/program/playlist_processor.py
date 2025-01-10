from pathlib import Path
from typing import Dict

from program.models import ProcessingState, SongMetadata
from program.config import Config
from program.genre_processor import GenreProcessor
from program.playlist_manager import PlaylistManager
from program.string_cleaner import StringCleaner


class PlaylistProcessor:
    """Main playlist processing orchestrator"""

    def __init__(self, config: Config):
        self.config = config
        self.playlist_manager = PlaylistManager(config)
        self.genre_processor = GenreProcessor(config)

    def process_playlist(self, playlist_path: Path) -> None:
        """Process all songs in a playlist"""
        spotdl_file = self.playlist_manager.find_spotdl_file(playlist_path)
        if not spotdl_file:
            print(f"\nError: No .spotdl file found in: {playlist_path}")
            return

        metadata_lookup, _ = self.playlist_manager.load_playlist_metadata(spotdl_file)
        if not metadata_lookup:
            return

        processing_state = ProcessingState(set(), set(), set())
        print(f"\nProcessing files in: {playlist_path}")

        self._process_mp3_files(playlist_path, metadata_lookup, processing_state)
        self._print_summary(processing_state)

    def _process_mp3_files(self, playlist_path: Path, metadata_lookup: Dict[str, SongMetadata],
                           processing_state: ProcessingState) -> None:
        """Process all MP3 files in the playlist"""
        for mp3_path in playlist_path.rglob('*.mp3'):
            filename = mp3_path.stem
            clean_filename = StringCleaner.clean_name(filename)
            clean_filename_alt = StringCleaner.clean_name(
                filename.lower()
                .replace('remastered', '')
                .replace('album version', '')
            )

            metadata = metadata_lookup.get(clean_filename) or metadata_lookup.get(clean_filename_alt)

            if metadata:
                self.genre_processor.fix_genres(mp3_path, metadata, processing_state)
            else:
                print(f"\nNo metadata found for: {mp3_path.name}")
                processing_state.songs_without_metadata.add(mp3_path.name)

    def _print_summary(self, state: ProcessingState) -> None:
        """Print summary of processing issues"""
        print("\n=== Processing Summary ===")

        if state.unmapped_styles:
            print("\nStyles without genre mappings:")
            for style in sorted(state.unmapped_styles):
                print(f"- {style}")
            print("\nTo map these styles, add them to the genre_styles.json file.")
        else:
            print("\nAll styles were successfully mapped to genres!")

        if state.songs_without_styles:
            print("\nSongs without any styles:")
            for song in sorted(state.songs_without_styles):
                print(f"- {song}")

        if state.songs_without_metadata:
            print("\nSongs without metadata in the JSON:")
            for song in sorted(state.songs_without_metadata):
                print(f"- {song}")

        if not (state.unmapped_styles or state.songs_without_styles or state.songs_without_metadata):
            print("\nNo issues found! All songs were processed successfully.")
