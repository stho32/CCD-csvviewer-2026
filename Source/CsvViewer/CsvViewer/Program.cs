using CsvViewer.BL.CommandLineInterpretation;
using CsvViewer.BL.Common;
using CsvViewer.BL.DocumentAcquisition;
using CsvViewer.BL.HostContracts;
using CsvViewer.BL.Interaction;
using CsvViewer.BL.PagePresentation;
using CsvViewer.HostSpecific.IO;
using CsvViewer.HostSpecific.Logging;

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

        IFileReader fileReader = new FileReader();
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

        Result<PagedDocument> pagedResult =
            Paginator.Paginate(parseResult.Value, argumentsResult.Value.PageSize);
        if (!pagedResult.IsSuccess)
        {
            logger.Error(pagedResult.Message);
            return 1;
        }

        var viewer = new InteractiveViewer(new SystemConsole(), new TableRenderer());
        Result viewerResult = viewer.Run(pagedResult.Value);
        if (!viewerResult.IsSuccess)
        {
            logger.Error(viewerResult.Message);
            return 1;
        }

        return 0;
    }
}
