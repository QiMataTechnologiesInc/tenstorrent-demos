"""Tests for the CLI module."""

from tenstorrent_demo.cli import main


def test_main(capsys: object) -> None:
    """Test main CLI function."""
    result = main()
    assert result == 0

    captured = capsys.readouterr()  # type: ignore
    assert "Hello from Tenstorrent Python Demo!" in captured.out
    assert "42 + 8 = 50" in captured.out
