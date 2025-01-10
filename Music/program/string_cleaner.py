import re


class StringCleaner:
    """String cleaning and normalization utilities"""

    @staticmethod
    def clean_name(name: str) -> str:
        """Clean and normalize a name string"""
        cleaned = name.lower().replace('&', 'and')
        cleaned = re.sub(r'[^a-z0-9\s]', '', cleaned)
        return ' '.join(cleaned.split())

    @staticmethod
    def capitalize_genre(genre: str) -> str:
        """Capitalize each word in a genre name"""
        return ' '.join(word.capitalize() for word in genre.split())
