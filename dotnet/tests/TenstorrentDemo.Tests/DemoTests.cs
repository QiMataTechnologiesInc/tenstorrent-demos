namespace TenstorrentDemo.Tests;

/// <summary>
/// Tests for the Demo class.
/// </summary>
public class DemoTests
{
    /// <summary>
    /// Tests that GetMessage returns the expected message.
    /// </summary>
    [Fact]
    public void GetMessage_ReturnsExpectedMessage()
    {
        // Arrange
        var demo = new Demo();

        // Act
        var message = demo.GetMessage();

        // Assert
        Assert.Equal("Hello from Tenstorrent .NET Demo!", message);
    }

    /// <summary>
    /// Tests that Add correctly adds two positive numbers.
    /// </summary>
    [Fact]
    public void Add_PositiveNumbers_ReturnsSum()
    {
        // Arrange
        var demo = new Demo();

        // Act
        var result = demo.Add(2, 3);

        // Assert
        Assert.Equal(5, result);
    }

    /// <summary>
    /// Tests that Add correctly adds negative numbers.
    /// </summary>
    [Fact]
    public void Add_NegativeNumbers_ReturnsSum()
    {
        // Arrange
        var demo = new Demo();

        // Act
        var result = demo.Add(-5, -3);

        // Assert
        Assert.Equal(-8, result);
    }

    /// <summary>
    /// Tests that Add correctly handles zero.
    /// </summary>
    [Fact]
    public void Add_WithZero_ReturnsOtherNumber()
    {
        // Arrange
        var demo = new Demo();

        // Act & Assert
        Assert.Equal(0, demo.Add(0, 0));
        Assert.Equal(42, demo.Add(42, 0));
    }
}
