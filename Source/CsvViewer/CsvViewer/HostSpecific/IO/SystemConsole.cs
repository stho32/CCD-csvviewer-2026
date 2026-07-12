using CsvViewer.BL.Common;
using CsvViewer.BL.IO;

namespace CsvViewer.HostSpecific.IO;

internal sealed class SystemConsole : IConsole
{
    public Result Clear()
    {
        try
        {
            Console.Clear();
            return new Result(true, string.Empty);
        }
        catch (Exception ex) when (
            ex is IOException
            or PlatformNotSupportedException)
        {
            return new Result(false, $"Die Konsole konnte nicht geleert werden: {ex.Message}");
        }
    }

    public Result Write(string text)
    {
        try
        {
            Console.Write(text);
            return new Result(true, string.Empty);
        }
        catch (IOException ex)
        {
            return new Result(false, $"Die Konsolenausgabe ist fehlgeschlagen: {ex.Message}");
        }
    }

    public Result<char> ReadKey()
    {
        try
        {
            char key = Console.ReadKey(intercept: true).KeyChar;
            return new Result<char>(key, true, string.Empty);
        }
        catch (Exception ex) when (
            ex is InvalidOperationException
            or IOException)
        {
            return new Result<char>(
                default,
                false,
                $"Die Tastatureingabe ist fehlgeschlagen: {ex.Message}");
        }
    }
}
