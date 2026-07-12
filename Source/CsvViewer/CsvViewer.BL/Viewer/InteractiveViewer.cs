using CsvViewer.BL.Common;
using CsvViewer.BL.Csv;
using CsvViewer.BL.IO;
using CsvViewer.BL.Navigation;
using CsvViewer.BL.Rendering;

namespace CsvViewer.BL.Viewer;

public sealed class InteractiveViewer
{
    public const string Menu =
        "F)irst page, P)revious page, N)ext page, L)ast page, E)xit";

    private readonly IConsole _console;
    private readonly ITableRenderer _tableRenderer;

    public InteractiveViewer(IConsole console, ITableRenderer tableRenderer)
    {
        _console = console;
        _tableRenderer = tableRenderer;
    }

    public Result Run(IReadOnlyList<CsvDocument>? pages)
    {
        if (pages is null || pages.Count == 0)
        {
            return new Result(false, "Der Viewer benötigt mindestens eine Seite.");
        }

        Result<PageNavigator> navigatorResult = PageNavigator.Create(pages.Count);
        if (!navigatorResult.IsSuccess)
        {
            return new Result(false, navigatorResult.Message);
        }

        PageNavigator navigator = navigatorResult.Value!;

        while (true)
        {
            Result drawResult = DrawPage(pages[navigator.CurrentPageIndex]);
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

    private Result DrawPage(CsvDocument page)
    {
        Result clearResult = _console.Clear();
        if (!clearResult.IsSuccess)
        {
            return clearResult;
        }

        Result<string> tableResult = _tableRenderer.Render(page);
        if (!tableResult.IsSuccess)
        {
            return new Result(false, tableResult.Message);
        }

        return _console.Write(
            $"{tableResult.Value}{Environment.NewLine}{Menu}{Environment.NewLine}");
    }
}
