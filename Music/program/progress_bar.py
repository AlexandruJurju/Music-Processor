import sys


class ProgressDisplay:
    """Handles displaying progress bar for playlist processing."""

    def __init__(self, total_files: int):
        """Initialize progress display with total number of files."""
        self.total_files = total_files
        self.processed_files = 0
        self.bar_width = 40

    def update(self) -> None:
        """Update and display the progress bar."""
        self.processed_files += 1
        percentage = (self.processed_files / self.total_files) * 100
        filled = int(self.bar_width * self.processed_files // self.total_files)
        bar = '█' * filled + '░' * (self.bar_width - filled)
        sys.stdout.write(f'\rProcessing: [{bar}] {percentage:3.1f}%')
        sys.stdout.flush()

    def clear(self) -> None:
        """Clear the progress bar from the console."""
        sys.stdout.write('\r' + ' ' * 100 + '\r')
        sys.stdout.flush()
