from dataclasses import dataclass
from typing import Set, Optional, List


@dataclass
class ProcessingState:
    unmapped_styles: Set[str]
    songs_without_styles: Set[str]
    songs_without_metadata: Set[str]


@dataclass
class SongMetadata:
    artist: str
    name: str
    album_name: Optional[str]
    album_id: Optional[str]
    genres: List[str]
