using CommandLine;
using CommandLine.Text;
using CsvViewer.BL.Common;

namespace CsvViewer.BL.CommandLineArguments;

/// <summary>
/// Operation nach IODA: wandelt rohe Kommandozeilen-Argumente in
/// <see cref="CommandLineOptions"/> um. Schreibt selbst nichts auf die Konsole —
/// Hilfe- und Fehlertexte werden als Fehler-<see cref="Result{T}"/> zurückgegeben,
/// die Ausgabe entscheidet der Entry Point.
/// </summary>
public static class CommandLineArgumentsParser
{
    public static Result<CommandLineOptions> Parse(string[] args)
    {
        using var parser = new Parser(settings => settings.HelpWriter = null);
        ParserResult<CommandLineOptions> parserResult =
            parser.ParseArguments<CommandLineOptions>(args);

        if (parserResult is Parsed<CommandLineOptions> parsed)
        {
            return new Result<CommandLineOptions>(parsed.Value, true, string.Empty);
        }

        string hilfetext = HelpText.AutoBuild(parserResult).ToString();
        return new Result<CommandLineOptions>(null, false, hilfetext);
    }
}
