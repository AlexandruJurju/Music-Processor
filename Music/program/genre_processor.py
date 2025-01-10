from pathlib import Path
from typing import Dict, List, Set, Tuple

import mutagen
from mutagen.id3 import ID3, TXXX, TCON

from program.models import ProcessingState, SongMetadata
from program.config import Config


class GenreProcessor:
    """Handles genre processing and tag manipulation"""

    def __init__(self, config: Config):
        self.config = config
        self.style_to_genre = self._create_style_to_genre_lookup()
        self.remove_styles_set = {style.lower() for style in config.remove_styles}

    def _create_style_to_genre_lookup(self) -> Dict[str, str]:
        """Create style to genre lookup dictionary"""
        return {
            style.lower(): genre
            for genre, style_list in self.config.genre_mappings.items()
            for style in style_list
        }

    def extract_genres_and_styles(self, styles: List[str], unmapped_styles: Set[str]) -> Tuple[List[str], List[str]]:
        """Extract genres and remaining styles"""
        genres = set()
        remaining_styles = []

        for style in styles:
            style_lower = style.lower()

            if style_lower in self.remove_styles_set:
                continue

            mapped_genre = self.style_to_genre.get(style_lower)

            if mapped_genre:
                genres.add(mapped_genre)
                if style_lower != mapped_genre.lower():
                    remaining_styles.append(style)
            else:
                unmapped_styles.add(style)
                remaining_styles.append(style)

        return list(genres), remaining_styles

    def fix_genres(self, song_path: Path, metadata: SongMetadata, processing_state: ProcessingState) -> None:
        """Update genre and style tags for a song"""
        styles = metadata.genres or []
        print(f"\nProcessing: {song_path.name}")
        print(f"Original styles: {', '.join(styles)}")

        if not styles:
            print("No styles found")
            processing_state.songs_without_styles.add(song_path.name)
            return

        try:
            tags = self._get_or_create_id3_tags(song_path)
            tags.delall('TCON')  # Clear existing genre tags

            genres, remaining_styles = self.extract_genres_and_styles(
                styles, processing_state.unmapped_styles)

            self._update_tags(tags, genres, remaining_styles)
            self._print_processing_results(styles, genres, remaining_styles)

            tags.save(song_path)

        except Exception as e:
            print(f"Error processing {song_path}: {e}")

    def _get_or_create_id3_tags(self, song_path: Path) -> ID3:
        """Get existing ID3 tags or create new ones"""
        try:
            return ID3(song_path)
        except mutagen.id3.ID3NoHeaderError:
            return ID3()

    def _update_tags(self, tags: ID3, genres: List[str], remaining_styles: List[str]) -> None:
        """Update ID3 tags with genres and styles"""
        if remaining_styles:
            tags.add(TXXX(encoding=3, desc='Styles', text=remaining_styles))

        if genres:
            tags.add(TCON(encoding=3, text=genres))

    def _print_processing_results(self, original_styles: List[str], genres: List[str], remaining_styles: List[str]) -> None:
        """Print processing results"""
        if genres:
            print(f"Mapped Genres: {', '.join(genres)}")
            print(f"Remaining Styles: {', '.join(remaining_styles)}")

            removed_styles = [s for s in original_styles if s not in remaining_styles and s.lower() not in [g.lower() for g in genres]]
            if removed_styles:
                print(f"Removed styles: {', '.join(removed_styles)}")
        else:
            print("No genres mapped")
            print(f"Styles unchanged: {', '.join(original_styles)}")
