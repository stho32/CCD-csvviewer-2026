using CsvViewer.BL.Common;

namespace CsvViewer.BL.CommandLineInterpretation;

public static class ArgumentsParser
{
    public const int DefaultPageSize = 10;
    public const string Usage = "Usage: csvviewer <datei.csv> [seitengröße]";

    public static Result<ViewerArguments> Parse(string[]? args)
    {
        if (args is null || args.Length is < 1 or > 2 || string.IsNullOrWhiteSpace(args[0]))
        {
            return new Result<ViewerArguments>(null, false, Usage);
        }

        int pageSize = DefaultPageSize;
        if (args.Length == 2 &&
            (!int.TryParse(args[1], out pageSize) || pageSize <= 0))
        {
            return new Result<ViewerArguments>(
                null,
                false,
                "Die Seitengröße muss eine positive Ganzzahl sein.");
        }

        return new Result<ViewerArguments>(
            new ViewerArguments(args[0], pageSize),
            true,
            string.Empty);
    }
}
