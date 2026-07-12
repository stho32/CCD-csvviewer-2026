using CsvViewer.BL.Common;

namespace CsvViewer.BL.CommandLineArguments;

public static class CommandLineArgumentsParser
{
    public static Result<ViewerArguments> Parse(string[]? args)
    {
        if (args is [("--file" or "-f"), var filePath])
        {
            return ArgumentsParser.Parse([filePath]);
        }

        return new Result<ViewerArguments>(null, false, ArgumentsParser.Usage);
    }
}
