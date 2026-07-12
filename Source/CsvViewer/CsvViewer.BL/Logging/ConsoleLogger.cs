using CsvViewer.BL.Common;

namespace CsvViewer.BL.Logging;

public sealed class ConsoleLogger : ILogger
{
    public Result Error(string message)
    {
        try
        {
            Console.Error.WriteLine($"[ERROR] {message}");
            return new Result(true, string.Empty);
        }
        catch (Exception ex) when (
            ex is IOException
            or ObjectDisposedException)
        {
            return new Result(false, $"Die Konsolenausgabe ist fehlgeschlagen: {ex.Message}");
        }
    }
}
