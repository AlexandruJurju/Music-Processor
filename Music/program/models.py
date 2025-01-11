from dataclasses import dataclass
from typing import Set, Optional, List


@dataclass
class ProcessingState:
    unmapped_styles: Set[str]
    songs_without_styles: Set[str]
    songs_without_metadata: Set[str]


@dataclass
class SongMetadata:
    artists: List[str]
    name: str
    genres: List[str]
