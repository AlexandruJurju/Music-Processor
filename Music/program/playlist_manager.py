import json
from pathlib import Path
from typing import Tuple, Dict, Set

from program.config import Config
from program.models import SongMetadata
from program.string_cleaner import StringCleaner


class PlaylistManager:
    """Manages playlist operations"""

    def __init__(self, config: Config):
        self.config = config

    def load_playlist_metadata(self, playlist_file: Path) -> Tuple[Dict[str, SongMetadata]]:
        """Load and parse playlist metadata"""
        try:
            with open(playlist_file, 'r', encoding='utf-8') as f:
                playlist_data = json.load(f)

            metadata_lookup = {}

            for song in playlist_data['songs']:
                metadata = self._create_song_metadata(song)
                self._add_metadata_lookups(metadata_lookup, metadata, song)

            return metadata_lookup

        except Exception as e:
            print(f"\nError reading playlist file: {e}")
            return {}, set()

    def _create_song_metadata(self, song: dict) -> SongMetadata:
        """Create SongMetadata from song dict"""
        return SongMetadata(
            artist=song['artist'],
            name=song['name'],
            genres=[StringCleaner.capitalize_genre(genre) for genre in song.get('genres', [])]
        )

    def _add_metadata_lookups(self, metadata_lookup: dict, metadata: SongMetadata, song: dict) -> None:
        """Add metadata lookups for different artist combinations"""
        artists = song.get('artists', [song['artist']])

        # Single artist version
        clean_key = f"{StringCleaner.clean_name(song['artist'])} - {StringCleaner.clean_name(metadata.name)}"
        metadata_lookup[clean_key] = metadata
        clean_key_alt = StringCleaner.clean_name(
            f"{song['artist']} - {metadata.name}".replace('remastered', '').replace('album version', '')
        )
        metadata_lookup[clean_key_alt] = metadata

        # Multiple artists version
        if len(artists) > 1:
            artists_str = ', '.join(artists)
            collab_key = f"{StringCleaner.clean_name(artists_str)} - {StringCleaner.clean_name(metadata.name)}"
            metadata_lookup[collab_key] = metadata
            collab_key_alt = StringCleaner.clean_name(
                f"{artists_str} - {metadata.name}".replace('remastered', '').replace('album version', '')
            )
            metadata_lookup[collab_key_alt] = metadata
