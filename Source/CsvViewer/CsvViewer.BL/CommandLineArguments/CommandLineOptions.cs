using CommandLine;

namespace CsvViewer.BL.CommandLineArguments;

public class CommandLineOptions
{
    [Option('f', "file", Required = true, HelpText = "Pfad zur CSV-Datei, die angezeigt werden soll.")]
    public string File { get; init; } = string.Empty;

    [Option('v', "verbose", Required = false, HelpText = "Ausführliche Ausgabe aktivieren.")]
    public bool Verbose { get; init; }
}
