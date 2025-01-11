import re


class StringCleaner:
    @staticmethod
    def clean_name(name: str) -> str:
        """Clean and normalize the name of the song when searching the spotdl json file"""
        return name.replace('/', '')

    @staticmethod
    def capitalize_genre(genre: str) -> str:
        """Capitalize each word in a genre name - used when writing the genres of the song"""
        return ' '.join(word.capitalize() for word in genre.split())
