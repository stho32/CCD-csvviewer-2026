using CsvViewer.BL.CommandLineArguments;
using CsvViewer.BL.Common;
using CsvViewer.BL.Csv;
using CsvViewer.BL.IO;
using CsvViewer.BL.Logging;
using CsvViewer.BL.Paging;
using CsvViewer.BL.Rendering;
using CsvViewer.BL.Viewer;

namespace CsvViewer;

internal class Program
{
    public static int Main(string[] args)
    {
        ILogger logger = new ConsoleLogger();

        Result<ViewerArguments> argumentsResult = ArgumentsParser.Parse(args);
        if (!argumentsResult.IsSuccess)
        {
            logger.Error(argumentsResult.Message);
            return 1;
        }

        var fileReader = new FileReader();
        Result<IReadOnlyList<string>> readResult =
            fileReader.ReadLines(argumentsResult.Value!.FilePath);
        if (!readResult.IsSuccess)
        {
            logger.Error(readResult.Message);
            return 1;
        }

        Result<CsvDocument> parseResult = CsvParser.Parse(readResult.Value!);
        if (!parseResult.IsSuccess)
        {
            logger.Error(parseResult.Message);
            return 1;
        }

        Result<CsvPageCollection> pagesResult =
            Paginator.Paginate(parseResult.Value, argumentsResult.Value.PageSize);
        if (!pagesResult.IsSuccess)
        {
            logger.Error(pagesResult.Message);
            return 1;
        }

        var viewer = new InteractiveViewer(new SystemConsole(), new TableRenderer());
        Result viewerResult = viewer.Run(pagesResult.Value);
        if (!viewerResult.IsSuccess)
        {
            logger.Error(viewerResult.Message);
            return 1;
        }

        return 0;
    }
}
