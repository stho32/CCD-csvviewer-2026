using CommandLine;

namespace CsvViewer.BL.CommandLineArguments;

public class CommandLineOptions
{
    [Option('f', "file", Required = true, HelpText = "Pfad zur CSV-Datei, die angezeigt werden soll.")]
    public string File { get; set; } = string.Empty;

    [Option('d', "delimiter", Required = false, Default = ',', HelpText = "Feldtrennzeichen der CSV-Datei.")]
    public char Delimiter { get; set; } = ',';

    [Option('v', "verbose", Required = false, HelpText = "Ausfuehrliche Ausgabe aktivieren.")]
    public bool Verbose { get; set; }
}
