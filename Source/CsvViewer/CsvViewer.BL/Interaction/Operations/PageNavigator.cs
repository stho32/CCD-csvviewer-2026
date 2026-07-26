using CsvViewer.BL.Common;
using CsvViewer.BL.Interaction.Data;

namespace CsvViewer.BL.Interaction.Operations;

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
        switch (command)
        {
            case NavigationCommand.First:
                CurrentPageIndex = 0;
                break;
            case NavigationCommand.Previous:
                CurrentPageIndex = Math.Max(0, CurrentPageIndex - 1);
                break;
            case NavigationCommand.Next:
                CurrentPageIndex = Math.Min(_lastPageIndex, CurrentPageIndex + 1);
                break;
            case NavigationCommand.Last:
                CurrentPageIndex = _lastPageIndex;
                break;
            case NavigationCommand.Exit:
            case NavigationCommand.None:
                break;
            default:
                return new Result<int>(
                    CurrentPageIndex,
                    false,
                    "Der Navigationsbefehl ist unbekannt.");
        }

        return new Result<int>(CurrentPageIndex, true, string.Empty);
    }
}
