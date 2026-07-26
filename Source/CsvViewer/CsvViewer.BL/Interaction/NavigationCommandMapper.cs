using CsvViewer.BL.Common;

namespace CsvViewer.BL.Interaction;

public static class NavigationCommandMapper
{
    public static Result<NavigationCommand> Map(char key)
    {
        NavigationCommand command = char.ToUpperInvariant(key) switch
        {
            'F' => NavigationCommand.First,
            'P' => NavigationCommand.Previous,
            'N' => NavigationCommand.Next,
            'L' => NavigationCommand.Last,
            'E' => NavigationCommand.Exit,
            _ => NavigationCommand.None,
        };

        return new Result<NavigationCommand>(command, true, string.Empty);
    }
}
