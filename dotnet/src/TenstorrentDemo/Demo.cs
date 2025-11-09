namespace TenstorrentDemo;

/// <summary>
/// A simple demo class for Tenstorrent.
/// </summary>
public class Demo
{
    private readonly string message;

    /// <summary>
    /// Initializes a new instance of the <see cref="Demo"/> class.
    /// </summary>
    public Demo()
    {
        this.message = "Hello from Tenstorrent .NET Demo!";
    }

    /// <summary>
    /// Gets the demo message.
    /// </summary>
    /// <returns>The demo message string.</returns>
    public string GetMessage()
    {
        return this.message;
    }

    /// <summary>
    /// Adds two numbers.
    /// </summary>
    /// <param name="a">First number.</param>
    /// <param name="b">Second number.</param>
    /// <returns>The sum of a and b.</returns>
    public int Add(int a, int b)
    {
        return a + b;
    }
}
