using CommandLine;
using CommandLine.Text;
using CsvViewer.BL.Common;

namespace CsvViewer.BL.CommandLineArguments;

/// <summary>
/// Schreibt nichts auf die Konsole — Hilfe- und Fehlertexte kommen als Fehler-Result zurück.
/// </summary>
public static class CommandLineArgumentsParser
{
    public static Result<ViewerArguments> Parse(string[] args)
    {
        using var parser = new Parser(settings => settings.HelpWriter = null);
        ParserResult<CommandLineOptions> parserResult =
            parser.ParseArguments<CommandLineOptions>(args);

        if (parserResult is Parsed<CommandLineOptions> parsed)
        {
            var viewerArguments = new ViewerArguments(parsed.Value.File);
            return new Result<ViewerArguments>(viewerArguments, true, string.Empty);
        }

        string helpText = HelpText.AutoBuild(parserResult).ToString();
        return new Result<ViewerArguments>(null, false, helpText);
    }
}
