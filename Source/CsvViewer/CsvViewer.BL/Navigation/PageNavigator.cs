using CsvViewer.BL.Common;

namespace CsvViewer.BL.Navigation;

public sealed class PageNavigator
{
    private readonly int _lastPageIndex;

    private PageNavigator(int pageCount)
    {
        _lastPageIndex = pageCount - 1;
    }

    public int CurrentPageIndex { get; private set; }

    public static Result<PageNavigator> Create(int pageCount)
    {
        if (pageCount <= 0)
        {
            return new Result<PageNavigator>(
                null,
                false,
                "Die Navigation benötigt mindestens eine Seite.");
        }

        return new Result<PageNavigator>(
            new PageNavigator(pageCount),
            true,
            string.Empty);
    }

    public Result<int> Apply(NavigationCommand command)
    {
        if (!Enum.IsDefined(command))
        {
            return new Result<int>(
                CurrentPageIndex,
                false,
                "Der Navigationsbefehl ist unbekannt.");
        }

        CurrentPageIndex = command switch
        {
            NavigationCommand.First => 0,
            NavigationCommand.Previous => Math.Max(0, CurrentPageIndex - 1),
            NavigationCommand.Next => Math.Min(_lastPageIndex, CurrentPageIndex + 1),
            NavigationCommand.Last => _lastPageIndex,
            NavigationCommand.Exit or NavigationCommand.None => CurrentPageIndex,
            _ => CurrentPageIndex,
        };

        return new Result<int>(CurrentPageIndex, true, string.Empty);
    }
}
