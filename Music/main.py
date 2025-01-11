import sys

from program.cli import CLI
from program.file_system_handler import FileSystemHandler


def main() -> None:
    """Main entry point"""
    try:
        base_dir = FileSystemHandler.get_base_dir()
        cli = CLI(base_dir)
        cli.run()
    except KeyboardInterrupt:
        print("\nExiting...")
    except Exception as e:
        print(f"Error: {e}")
        sys.exit(1)


if __name__ == "__main__":
    main()
