using CsvViewer.BL.CommandLineArguments;
using CsvViewer.BL.Logging;

namespace CsvViewer;

public class Program
{
    public static int Main(string[] args)
    {
        var options = CommandLineArgumentsParser.Parse(args);
        if (options is null)
            return 1;

        ILogger logger = new ConsoleLogger();
        logger.Info("CsvViewer gestartet.");

        // Anwendungslogik folgt hier (CSV lesen und anzeigen).

        logger.Info("CsvViewer beendet.");
        return 0;
    }
}
