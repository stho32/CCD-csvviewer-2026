using CommandLine;
using CommandLine.Text;
using CsvViewer.BL.Common;

namespace CsvViewer.BL.CommandLineArguments;

/// <summary>
/// Schreibt nichts auf die Konsole — Hilfe- und Fehlertexte kommen als Fehler-Result zurück.
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

        string helpText = HelpText.AutoBuild(parserResult).ToString();
        return new Result<CommandLineOptions>(null, false, helpText);
    }
}
