import json
from pathlib import Path


class Config:
    """Configuration management class"""
    DEFAULT_GENRES = {
        'Rock': ['rock', 'alternative rock'],
        'Synthwave': ['synthwave', 'darksynth'],
        'Pop': ['pop', 'dance pop']
    }

    DEFAULT_REMOVE_STYLES = [
        'britpop',
        'madchester'
    ]

    def __init__(self, base_dir: Path):
        self.base_dir = base_dir
        self.genre_mappings = self._load_json('genre_styles.json', self.DEFAULT_GENRES)
        self.remove_styles = self._load_json('remove_styles_only.json', self.DEFAULT_REMOVE_STYLES)

    def _load_json(self, filename: str, default_value: dict | list) -> dict | list:
        """Generic JSON loader with default value handling"""
        file_path = self.base_dir / filename
        print(f"\nSearching for {filename} in: {file_path}")

        try:
            if not file_path.exists():
                with open(file_path, 'w', encoding='utf-8') as f:
                    json.dump(default_value, f, indent=4)
                print(f"Created default {filename}")
                return default_value

            with open(file_path, 'r', encoding='utf-8') as f:
                return json.load(f)

        except Exception as e:
            print(f"\nError loading {filename}: {e}")
            print("Using default values")
            return default_value
