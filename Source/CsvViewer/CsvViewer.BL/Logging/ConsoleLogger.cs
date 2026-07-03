namespace CsvViewer.BL.Logging;

public class ConsoleLogger : ILogger
{
    public void Info(string message) => Console.WriteLine($"[INFO] {message}");
    public void Error(string message) => Console.Error.WriteLine($"[ERROR] {message}");
}
