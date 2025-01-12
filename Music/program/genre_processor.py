from pathlib import Path
from typing import Dict, List, Set, Tuple
from collections import defaultdict
import logging

import mutagen
from mutagen.id3 import ID3, TXXX, TCON
from mutagen.flac import FLAC

from program.models import ProcessingState, SongMetadata


class GenreProcessor:
    def __init__(self, config):
        self.config = config
        self.style_to_genres = self._create_style_to_genres_lookup()
        self.remove_styles_set = {style.lower() for style in config.remove_styles}

    def fix_genres_external_song(self, song_path: Path, processing_state: ProcessingState, logger: logging.Logger) -> None:
        """Process genres from existing file metadata"""
        logger.info(f"\nProcessing existing metadata for: {song_path.name}")

        try:
            existing_genres = set()

            if song_path.suffix.lower() == '.flac':
                audio = FLAC(song_path)
                if 'genre' in audio:
                    existing_genres.update(audio['genre'])
                if 'styles' in audio:
                    existing_genres.update(audio['styles'])
            else:
                tags = self._get_or_create_id3_tags(song_path)
                existing_genres = self._extract_existing_genres(tags)

            if not existing_genres:
                logger.warning("No existing genres/styles found")
                processing_state.songs_without_styles.add(song_path.name)
                return

            temp_metadata = SongMetadata(
                artists=[""],
                name="",
                genres=list(existing_genres)
            )

            self.fix_genres_spotdl_song(song_path, temp_metadata, processing_state, logger)

        except Exception as e:
            logger.error(f"Error processing existing metadata: {e}")
            processing_state.songs_without_metadata.add(song_path.name)

    def fix_genres_spotdl_song(self, song_path: Path, metadata: SongMetadata, processing_state: ProcessingState, logger: logging.Logger) -> None:
        """Update genre and style tags for a song"""
        styles = metadata.genres or []
        logger.info(f"\nProcessing: {song_path.name}")
        logger.info(f"Original styles: {', '.join(styles)}")

        if not styles:
            logger.warning("No styles found")
            processing_state.songs_without_styles.add(song_path.name)
            return

        try:
            # Process the genres and styles
            genres, remaining_styles = self._extract_genres_and_styles(styles, processing_state.unmapped_styles)

            # Update tags based on file type
            if song_path.suffix.lower() == '.flac':
                self._update_flac_tags(song_path, genres, remaining_styles)
            else:
                # Handle MP3 and other ID3-compatible formats
                tags = self._get_or_create_id3_tags(song_path)
                tags.delall('TCON')  # Clear existing genre tags
                self._update_tags(tags, genres, remaining_styles)
                tags.save(song_path)

            self._log_genre_processing_results(logger, styles, genres, remaining_styles)

        except Exception as e:
            logger.error(f"Error processing {song_path}: {e}")

    def _log_genre_processing_results(self, logger: logging.Logger, original_styles: List[str], genres: List[str], remaining_styles: List[str]) -> None:
        """Log genre processing results"""
        if genres:
            logger.info(f"Mapped Genres: {', '.join(genres)}")
            logger.info(f"Remaining Styles: {', '.join(remaining_styles)}")

            removed_styles = [s for s in original_styles if s not in remaining_styles and s.lower() not in [g.lower() for g in genres]]
            if removed_styles:
                logger.info(f"Removed styles: {', '.join(removed_styles)}")
        else:
            logger.warning("No genres mapped")
            logger.info(f"Styles unchanged: {', '.join(original_styles)}")

    def _create_style_to_genres_lookup(self) -> Dict[str, Set[str]]:
        """Create style to genres lookup dictionary"""
        style_to_genres = defaultdict(set)
        for genre, style_list in self.config.genre_mappings.items():
            for style in style_list:
                style_to_genres[style.lower()].add(genre)
        return dict(style_to_genres)

    def _extract_genres_and_styles(self, styles: List[str], unmapped_styles: Set[str]) -> Tuple[List[str], List[str]]:
        """Extract genres and remaining styles"""
        genres = set()
        remaining_styles = []

        for style in styles:
            style_lower = style.lower()

            if style_lower in self.remove_styles_set:
                continue

            mapped_genres = self.style_to_genres.get(style_lower)

            if mapped_genres:
                genres.update(mapped_genres)
                if not any(style_lower == genre.lower() for genre in mapped_genres):
                    remaining_styles.append(style)
            else:
                unmapped_styles.add(style)
                remaining_styles.append(style)

        return list(genres), remaining_styles

    def _update_flac_tags(self, song_path: Path, genres: List[str], remaining_styles: List[str]) -> None:
        """Update FLAC file tags"""
        try:
            audio = FLAC(song_path)
            audio.tags.clear()
            if genres:
                audio['genre'] = genres
            if remaining_styles:
                audio['styles'] = remaining_styles
            audio.save()
        except Exception as e:
            raise Exception(f"Error updating FLAC tags: {e}")

    def _extract_existing_genres(self, tags: ID3) -> Set[str]:
        """Extract existing genres from ID3 tags"""
        genres = set()
        if 'TCON' in tags:
            genres.update(tags['TCON'].text)
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
            tags = ID3()
            tags.save(song_path)
            return tags

    def _update_tags(self, tags: ID3, genres: List[str], remaining_styles: List[str]) -> None:
        """Update ID3 tags with genres and styles"""
        if remaining_styles:
            tags.add(TXXX(encoding=3, desc='Styles', text=remaining_styles))
        if genres:
            tags.add(TCON(encoding=3, text=genres))