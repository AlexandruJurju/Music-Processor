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
            # Process the genres and styles
            genres, remaining_styles = self.extract_genres_and_styles(
                styles, processing_state.unmapped_styles)

            # Update tags based on file type
            if song_path.suffix.lower() == '.flac':
                self._update_flac_tags(song_path, genres, remaining_styles)
            else:
                # Handle MP3 and other ID3-compatible formats
                tags = self._get_or_create_id3_tags(song_path)
                tags.delall('TCON')  # Clear existing genre tags
                self._update_tags(tags, genres, remaining_styles)
                tags.save(song_path)

            self._print_processing_results(styles, genres, remaining_styles)

        except Exception as e:
            print(f"Error processing {song_path}: {e}")

    def _update_flac_tags(self, song_path: Path, genres: List[str], remaining_styles: List[str]) -> None:
        """Update FLAC file tags"""
        from mutagen.flac import FLAC
        try:
            audio = FLAC(song_path)

            # Clear existing genres and ratings
            audio.tags.clear()

            # Add genres
            if genres:
                audio['genre'] = genres

            # Add remaining styles as a custom tag
            if remaining_styles:
                audio['styles'] = remaining_styles

            # Clear rating
            if 'rating' in audio:
                del audio['rating']

            audio.save()
        except Exception as e:
            print(f"Error updating FLAC tags: {e}")

    def process_existing_metadata(self, song_path: Path, processing_state: ProcessingState) -> None:
        """Process genres from existing file metadata"""
        print(f"\nProcessing existing metadata for: {song_path.name}")
        try:
            existing_genres = set()

            if song_path.suffix.lower() == '.flac':
                # Handle FLAC files
                from mutagen.flac import FLAC
                audio = FLAC(song_path)
                if 'genre' in audio:
                    existing_genres.update(audio['genre'])
                if 'styles' in audio:
                    existing_genres.update(audio['styles'])
            else:
                # Handle MP3 and other ID3-compatible formats
                tags = self._get_or_create_id3_tags(song_path)
                existing_genres = self._extract_existing_genres(tags)

            if not existing_genres:
                print("No existing genres/styles found")
                processing_state.songs_without_styles.add(song_path.name)
                return

            # Create temporary metadata for processing
            temp_metadata = SongMetadata(
                artist="",  # Not needed for genre processing
                name="",  # Not needed for genre processing
                album_name=None,
                album_id=None,
                genres=list(existing_genres)
            )

            # Use existing fix_genres logic
            self.fix_genres(song_path, temp_metadata, processing_state)

        except Exception as e:
            print(f"Error processing existing metadata: {e}")
            processing_state.songs_without_metadata.add(song_path.name)

    def _extract_existing_genres(self, tags: ID3) -> Set[str]:
        """Extract existing genres from ID3 tags"""
        genres = set()

        # Get standard genre tags
        if 'TCON' in tags:
            genres.update(tags['TCON'].text)

        # Get styles from TXXX
        for tag in tags.getall('TXXX'):
            if tag.desc.lower() in ('styles', 'genre', 'genres'):
                if isinstance(tag.text, str):
                    genres.add(tag.text)
                else:
                    genres.update(tag.text)

        return genres

    def _get_or_create_id3_tags(self, song_path: Path) -> ID3:
        """Get existing ID3 tags or create new ones"""
        try:
            return ID3(song_path)
        except mutagen.id3.ID3NoHeaderError:
            # Create new ID3 tag and add it to the file
            tags = ID3()
            tags.save(song_path)  # This creates the ID3 header in the file
            return tags

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