import subprocess
from pathlib import Path
from typing import List


class SpotDLWrapper:
    """Wrapper for spotdl command-line operations"""

    @staticmethod
    def run_command(command: List[str]) -> None:
        """Run a spotdl command"""
        try:
            subprocess.run(command, check=True)
        except subprocess.CalledProcessError as e:
            print(f"Error running spotdl command: {e}")

    def new_sync(self, playlist_url: str, playlist_name: str, base_dir: Path) -> None:
        """Handle first-time playlist sync"""
        playlist_dir = base_dir / playlist_name
        playlist_dir.mkdir(exist_ok=True)

        self.run_command([
            "spotdl",
            playlist_url,
            "--output", str(playlist_dir),
            "--save-file", str(playlist_dir / f"{playlist_name}.spotdl")
        ])

    def update_sync(self, playlist_name: str, base_dir: Path) -> None:
        """Handle updating an existing playlist"""
        sync_file = base_dir / playlist_name / f"{playlist_name}.spotdl"

        if sync_file.exists():
            self.run_command([
                "spotdl",
                "sync", str(sync_file),
                "--output", str(base_dir / playlist_name)
            ])
        else:
            print(f"\nError: Sync file not found for playlist {playlist_name}")
