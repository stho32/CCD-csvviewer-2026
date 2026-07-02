using CsvViewer.BL.CommandLineArguments;
using CsvViewer.BL.Common;
using CsvViewer.BL.Logging;

namespace CsvViewer;

public class Program
{
    public static int Main(string[] args)
    {
        ILogger logger = new ConsoleLogger();

        Result<CommandLineOptions> optionsResult = CommandLineArgumentsParser.Parse(args);
        if (!optionsResult.IsSuccess)
        {
            logger.Error(optionsResult.Message);
            return 1;
        }

        logger.Info("CsvViewer gestartet.");

        // Anwendungslogik folgt hier (CSV lesen und anzeigen).

        logger.Info("CsvViewer beendet.");
        return 0;
    }
}
