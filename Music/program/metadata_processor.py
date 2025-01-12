from pathlib import Path
import json
import logging
from typing import Dict, List, Set, Optional
from datetime import datetime
from dataclasses import dataclass, asdict

import mutagen
from mutagen.id3 import ID3
from mutagen.flac import FLAC

from program.file_system_handler import FileSystemHandler
from program.models import ProcessingState, SongMetadata
from program.progress_bar import ProgressDisplay


@dataclass
class ExtendedSongMetadata(SongMetadata):
    """Extends the base SongMetadata with additional fields for full metadata scanning"""
    filepath: str
    album: Optional[str] = None
    year: Optional[str] = None
    styles: List[str] = None

    def __post_init__(self):
        self.styles = self.styles or []

    def to_dict(self):
        return asdict(self)


class MetadataProcessor:
    """Scans music files for metadata and saves it to a file"""

    def __init__(self, playlist_path: Path):
        self.playlist_path = playlist_path
        self.supported_extensions = {'.mp3', '.flac', '.m4a', '.wav', '.wma', '.aac', '.ogg'}
        self.processing_state = ProcessingState(set(), set(), set())

        # Setup logging
        self.logger = self._setup_logger()

    def _setup_logger(self) -> logging.Logger:
        """Setup logger for file output only"""
        # Create logger
        logger = logging.getLogger(f'metadata_scanner_{self.playlist_path.name}')
        logger.setLevel(logging.INFO)

        # Remove any existing handlers to avoid duplicates
        logger.handlers.clear()

        # Create file handler
        self.log_file = self.playlist_path / 'metadata_scan.log'
        file_handler = logging.FileHandler(self.log_file, mode='w', encoding='utf-8')
        file_handler.setLevel(logging.INFO)

        # Create formatter and add it to the handler
        file_formatter = logging.Formatter('%(asctime)s - %(levelname)s - %(message)s')
        file_handler.setFormatter(file_formatter)

        # Add handler to logger
        logger.addHandler(file_handler)

        return logger

    def scan_directory(self) -> List[ExtendedSongMetadata]:
        """Scan directory and return metadata for all supported music files"""
        music_files = FileSystemHandler.get_music_files(self.playlist_path)
        if not music_files:
            self.logger.warning("No music files found in the directory")
            return []

        all_metadata = []
        progress = ProgressDisplay(len(music_files))

        for file_path in music_files:
            try:
                metadata = self._extract_metadata(file_path)
                if metadata:
                    all_metadata.append(metadata)
            except Exception as e:
                self.logger.error(f"Error processing {file_path}: {e}")
                self.processing_state.songs_without_metadata.add(file_path.name)

            progress.update()

        progress.clear()
        self._log_processing_summary()
        return all_metadata

    def _get_music_files(self) -> List[Path]:
        """Get all music files in the directory and subdirectories"""
        music_files = []
        for ext in self.supported_extensions:
            music_files.extend(self.playlist_path.rglob(f'*{ext}'))
        return sorted(music_files)

    def _extract_metadata(self, file_path: Path) -> Optional[ExtendedSongMetadata]:
        """Extract metadata from a music file"""
        self.logger.info(f"\nProcessing: {file_path.name}")

        if file_path.suffix.lower() == '.flac':
            return self._extract_flac_metadata(file_path)
        else:
            return self._extract_id3_metadata(file_path)

    def _extract_flac_metadata(self, file_path: Path) -> Optional[ExtendedSongMetadata]:
        """Extract metadata from FLAC file"""
        try:
            audio = FLAC(file_path)

            # Extract basic metadata
            artists = audio.get('artist', ['Unknown Artist'])
            if isinstance(artists, str):
                artists = [artists]

            title = audio.get('title', [''])[0] if audio.get('title') else file_path.stem
            album = audio.get('album', [''])[0] if audio.get('album') else None

            # Extract genres and styles
            genres = list(audio.get('genre', []))
            styles = list(audio.get('styles', []))

            # Extract year
            year = audio.get('date', [''])[0] if audio.get('date') else None

            self._log_metadata_found(genres, styles)

            return ExtendedSongMetadata(
                filepath=str(file_path.relative_to(self.playlist_path)),
                artists=artists,
                name=title,
                album=album,
                genres=genres,
                styles=styles,
                year=year
            )
        except Exception as e:
            self.logger.error(f"Error reading FLAC metadata from {file_path}: {e}")
            return None

    def _extract_id3_metadata(self, file_path: Path) -> Optional[ExtendedSongMetadata]:
        """Extract metadata from ID3 tags"""
        try:
            tags = ID3(file_path)

            # Extract basic metadata
            artists = []
            if 'TPE1' in tags:
                artist_text = str(tags['TPE1'])
                artists = [artist.strip() for artist in artist_text.split(';')]
            if not artists:
                artists = ['Unknown Artist']

            title = str(tags.get('TIT2', file_path.stem))
            album = str(tags.get('TALB', '')) or None

            # Extract genres
            genres = []
            if 'TCON' in tags:
                genres.extend(tags['TCON'].text)

            # Extract styles from TXXX
            styles = []
            for tag in tags.getall('TXXX'):
                if tag.desc.lower() == 'styles':
                    if isinstance(tag.text, str):
                        styles.append(tag.text)
                    else:
                        styles.extend(tag.text)

            # Extract year
            year = str(tags.get('TDRC', '')) or None

            self._log_metadata_found(genres, styles)

            return ExtendedSongMetadata(
                filepath=str(file_path.relative_to(self.playlist_path)),
                artists=artists,
                name=title,
                album=album,
                genres=genres,
                styles=styles,
                year=year
            )
        except Exception as e:
            self.logger.error(f"Error reading ID3 metadata from {file_path}: {e}")
            return None

    def _log_metadata_found(self, genres: List[str], styles: List[str]) -> None:
        """Log found metadata"""
        if genres:
            self.logger.info(f"Found genres: {', '.join(genres)}")
        if styles:
            self.logger.info(f"Found styles: {', '.join(styles)}")
        if not genres and not styles:
            self.logger.warning("No genres or styles found")

    def _log_processing_summary(self) -> None:
        """Log processing summary information"""
        if self.processing_state.unmapped_styles:
            self.logger.info("\nStyles without genre mappings:")
            for style in sorted(self.processing_state.unmapped_styles):
                self.logger.info(f"- {style}")

        if self.processing_state.songs_without_styles:
            self.logger.warning("\nSongs without any styles:")
            for song in sorted(self.processing_state.songs_without_styles):
                self.logger.warning(f"- {song}")

        if self.processing_state.songs_without_metadata:
            self.logger.error("\nSongs without metadata:")
            for song in sorted(self.processing_state.songs_without_metadata):
                self.logger.error(f"- {song}")

    def save_metadata_to_file(self, metadata_list: List[ExtendedSongMetadata], output_file: Optional[Path] = None) -> None:
        """Save metadata to a JSON file"""
        if output_file is None:
            output_file = self.playlist_path / "metadata_scan.json"

        # Convert to list of dictionaries
        metadata_dicts = [meta.to_dict() for meta in metadata_list]

        # Add scan information
        output_data = {
            "scan_time": datetime.now().isoformat(),
            "total_songs": len(metadata_dicts),
            "playlist_path": str(self.playlist_path),
            "songs": metadata_dicts
        }

        # Save to file
        with open(output_file, 'w', encoding='utf-8') as f:
            json.dump(output_data, f, indent=2, ensure_ascii=False)

        # Print final summary to console
        print("\nScan Complete!")
        print(f"- Metadata saved to: {output_file}")
        print(f"- Detailed log saved to: {self.log_file}")
        print(f"- Total songs processed: {len(metadata_dicts)}")
        print(f"- Songs without styles: {len(self.processing_state.songs_without_styles)}")
        print(f"- Songs without metadata: {len(self.processing_state.songs_without_metadata)}")
        if self.processing_state.unmapped_styles:
            print(f"- Found {len(self.processing_state.unmapped_styles)} unmapped styles")
            print("  Check the log file for details")
