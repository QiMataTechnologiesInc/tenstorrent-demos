"""Tests for the Demo class."""

from tenstorrent_demo.demo import Demo


def test_demo_initialization() -> None:
    """Test Demo class initialization."""
    demo = Demo()
    assert demo.message == "Hello from Tenstorrent Python Demo!"


def test_get_message() -> None:
    """Test get_message method."""
    demo = Demo()
    assert demo.get_message() == "Hello from Tenstorrent Python Demo!"


def test_add() -> None:
    """Test add method."""
    demo = Demo()
    assert demo.add(2, 3) == 5
    assert demo.add(-5, 3) == -2
    assert demo.add(0, 0) == 0


def test_multiply_array() -> None:
    """Test multiply_array method."""
    demo = Demo()
    result = demo.multiply_array([1.0, 2.0, 3.0], 2.0)
    assert result == [2.0, 4.0, 6.0]


def test_multiply_array_empty() -> None:
    """Test multiply_array with empty list."""
    demo = Demo()
    result = demo.multiply_array([], 5.0)
    assert result == []
