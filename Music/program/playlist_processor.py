from pathlib import Path
from typing import Dict, List, Set

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

    def process_playlist(self, playlist_path: Path) -> None:
        """Process all songs in a playlist"""
        # Initialize processing state
        processing_state = ProcessingState(set(), set(), set())
        print(f"\nProcessing files in: {playlist_path}")

        # Try to get spotdl metadata if available
        metadata_lookup = {}
        spotdl_file = self.playlist_manager.find_spotdl_file(playlist_path)
        if spotdl_file:
            metadata_lookup, _ = self.playlist_manager.load_playlist_metadata(spotdl_file)
        else:
            print("No .spotdl file found - will process existing file metadata only")

        # Process all music files
        self._process_music_files(playlist_path, metadata_lookup, processing_state)
        self._print_summary(processing_state, playlist_path)

    def show_missing_files(self, playlist_path: Path) -> None:
        """Show files that spotdl failed to download"""
        spotdl_file = self.playlist_manager.find_spotdl_file(playlist_path)
        if not spotdl_file:
            print(f"\nError: No .spotdl file found in: {playlist_path}")
            return

        # Get expected files from spotdl
        try:
            metadata_lookup, _ = self.playlist_manager.load_playlist_metadata(spotdl_file)
            expected_files = {
                f"{metadata.artist} - {metadata.name}"
                for metadata in metadata_lookup.values()
            }
        except Exception as e:
            print(f"\nError reading spotdl file: {e}")
            return

        # Get actual files from directory
        music_files = []
        for ext in self.MUSIC_EXTENSIONS:
            music_files.extend(playlist_path.rglob(f'*{ext}'))

        # Clean actual filenames for comparison
        actual_files = {
            StringCleaner.clean_name(file.stem)
            for file in music_files
        }

        # Find missing files
        missing_files = expected_files - actual_files

        # Display results
        print(f"\nAnalyzing playlist in: {playlist_path}")
        print(f"Total expected files: {len(expected_files)}")
        print(f"Total files found: {len(actual_files)}")

        if missing_files:
            print(f"\nMissing {len(missing_files)} files:")
            for file in sorted(missing_files):
                print(f"- {file}")
        else:
            print("\nAll files were downloaded successfully!")

    def _process_music_files(self, playlist_path: Path, metadata_lookup: Dict[str, SongMetadata], processing_state: ProcessingState) -> None:
        """Process all music files in the playlist"""
        music_files = self._get_music_files(playlist_path)
        if not music_files:
            print("No music files found in the directory")
            return

        print(f"Found {len(music_files)} music files")

        for music_path in music_files:
            # First try to find spotdl metadata
            metadata = self._try_get_spotdl_metadata(music_path, metadata_lookup)

            if metadata:
                print(f"\nProcessing {music_path.name} with spotdl metadata")
                self.genre_processor.fix_genres(music_path, metadata, processing_state)
            else:
                # If no spotdl metadata, process existing file metadata
                print(f"\nProcessing {music_path.name} with existing metadata")
                try:
                    self.genre_processor.process_existing_metadata(music_path, processing_state)
                except Exception as e:
                    print(f"Error processing existing metadata: {e}")
                    processing_state.songs_without_metadata.add(music_path.name)

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

    def _print_summary(self, state: ProcessingState, playlist_path: Path) -> None:
        self.show_missing_files(playlist_path)

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
