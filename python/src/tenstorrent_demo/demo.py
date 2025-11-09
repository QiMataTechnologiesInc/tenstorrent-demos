"""Demo module for Tenstorrent."""


class Demo:
    """A simple demo class."""

    def __init__(self) -> None:
        """Initialize the Demo."""
        self.message = "Hello from Tenstorrent Python Demo!"

    def get_message(self) -> str:
        """Get the demo message.

        Returns:
            The demo message string.
        """
        return self.message

    def add(self, a: int, b: int) -> int:
        """Add two numbers.

        Args:
            a: First number.
            b: Second number.

        Returns:
            The sum of a and b.
        """
        return a + b

    def multiply_array(self, arr: list[float], factor: float) -> list[float]:
        """Multiply all elements in an array by a factor.

        Args:
            arr: List of numbers.
            factor: Multiplication factor.

        Returns:
            New list with multiplied values.
        """
        return [x * factor for x in arr]
