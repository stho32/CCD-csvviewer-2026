using CsvViewer.BL.Common;

namespace CsvViewer.BL.Logging;

public class ConsoleLogger : ILogger
{
    public Result Info(string message)
    {
        return Write(Console.Out, "INFO", message);
    }

    public Result Error(string message)
    {
        return Write(Console.Error, "ERROR", message);
    }

    private static Result Write(TextWriter writer, string level, string message)
    {
        try
        {
            writer.WriteLine($"[{level}] {message}");
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
