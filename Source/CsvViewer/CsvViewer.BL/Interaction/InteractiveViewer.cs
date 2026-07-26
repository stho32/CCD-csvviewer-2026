using CsvViewer.BL.Common;
using CsvViewer.BL.DocumentAcquisition.Data;
using CsvViewer.BL.DocumentAcquisition;
using CsvViewer.BL.HostContracts;
using CsvViewer.BL.Interaction;
using CsvViewer.BL.PagePresentation.Data;
using CsvViewer.BL.PagePresentation;

namespace CsvViewer.BL.Interaction;

public sealed class InteractiveViewer
{
    public const string Menu =
        "F)irst page, P)revious page, N)ext page, L)ast page, E)xit";

    private readonly IConsole? _console;
    private readonly ITableRenderer? _tableRenderer;

    public InteractiveViewer(IConsole? console, ITableRenderer? tableRenderer)
    {
        _console = console;
        _tableRenderer = tableRenderer;
    }

    public Result Run(PagedDocument? document)
    {
        if (_console is null || _tableRenderer is null)
        {
            return new Result(false, "Die Viewer-Abhängigkeiten fehlen.");
        }

        if (document is null || document.Pages.PageCount == 0)
        {
            return new Result(false, "Der Viewer benötigt mindestens eine Seite.");
        }

        Result<PageNavigator> navigatorResult = PageNavigator.Create(document.Pages.PageCount);
        if (!navigatorResult.IsSuccess)
        {
            return new Result(false, navigatorResult.Message);
        }

        PageNavigator navigator = navigatorResult.Value!;

        while (true)
        {
            Result drawResult = DrawPage(
                document.Header,
                document.Pages[navigator.CurrentPageIndex],
                _console,
                _tableRenderer);
            if (!drawResult.IsSuccess)
            {
                return drawResult;
            }

            Result<char> keyResult = _console.ReadKey();
            if (!keyResult.IsSuccess)
            {
                return new Result(false, keyResult.Message);
            }

            Result<NavigationCommand> commandResult =
                NavigationCommandMapper.Map(keyResult.Value);
            if (!commandResult.IsSuccess)
            {
                return new Result(false, commandResult.Message);
            }

            if (commandResult.Value == NavigationCommand.Exit)
            {
                return new Result(true, string.Empty);
            }

            Result<int> navigationResult = navigator.Apply(commandResult.Value);
            if (!navigationResult.IsSuccess)
            {
                return new Result(false, navigationResult.Message);
            }
        }
    }

    private static Result DrawPage(
        CsvHeader header,
        CsvRowCollection rows,
        IConsole console,
        ITableRenderer tableRenderer)
    {
        Result clearResult = console.Clear();
        if (!clearResult.IsSuccess)
        {
            return clearResult;
        }

        Result<string> tableResult = tableRenderer.Render(header, rows);
        if (!tableResult.IsSuccess)
        {
            return new Result(false, tableResult.Message);
        }

        return console.Write(
            $"{tableResult.Value}{Environment.NewLine}{Menu}{Environment.NewLine}");
    }
}
