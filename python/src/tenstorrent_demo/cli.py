"""Command-line interface for Tenstorrent demo."""

import sys

from tenstorrent_demo.demo import Demo


def main() -> int:
    """Main CLI entry point.

    Returns:
        Exit code (0 for success).
    """
    demo = Demo()
    print(demo.get_message())
    print(f"42 + 8 = {demo.add(42, 8)}")
    print(f"Array multiplication: {demo.multiply_array([1.0, 2.0, 3.0], 2.5)}")
    return 0


if __name__ == "__main__":
    sys.exit(main())
