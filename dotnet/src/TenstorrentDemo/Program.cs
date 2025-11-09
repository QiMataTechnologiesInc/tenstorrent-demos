namespace TenstorrentDemo;

/// <summary>
/// Main program entry point.
/// </summary>
public static class Program
{
    /// <summary>
    /// Application entry point.
    /// </summary>
    public static void Main()
    {
        var demo = new Demo();
        Console.WriteLine(demo.GetMessage());
        Console.WriteLine($"42 + 8 = {demo.Add(42, 8)}");
    }
}
