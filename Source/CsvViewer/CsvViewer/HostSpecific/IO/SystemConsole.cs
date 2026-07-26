using CsvViewer.BL.Common;
using CsvViewer.BL.HostContracts;

namespace CsvViewer.HostSpecific.IO;

/// <summary>
/// Adapter an die echte Konsole. Ist ein Kanal umgeleitet (Pipe statt Terminal), wird
/// der jeweilige Terminal-Aufruf durch sein Strom-Äquivalent ersetzt: So bleibt der
/// Viewer per <c>printf 'nne' | csvviewer datei.csv</c> steuerbar statt abzubrechen.
/// </summary>
internal sealed class SystemConsole : IConsole
{
    public Result Clear()
    {
        // Bei umgeleiteter Ausgabe gibt es keinen Bildschirm zu leeren; Console.Clear()
        // wuerde dort je nach Plattform mit einer IOException scheitern.
        if (Console.IsOutputRedirected)
        {
            return new Result(true, string.Empty);
        }

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
        // Console.ReadKey() braucht ein Terminal und wirft bei umgeleiteter Eingabe eine
        // InvalidOperationException. Dann wird zeichenweise aus dem Strom gelesen — ein
        // Zeichen entspricht einem Tastendruck, ein Enter ist weiterhin nicht noetig.
        if (Console.IsInputRedirected)
        {
            return ReadKeyFromRedirectedInput();
        }

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

    private static Result<char> ReadKeyFromRedirectedInput()
    {
        try
        {
            int value = Console.In.Read();
            if (value < 0)
            {
                return new Result<char>(
                    default,
                    false,
                    "Die Eingabe endete, ohne dass E) zum Beenden gewählt wurde.");
            }

            return new Result<char>((char)value, true, string.Empty);
        }
        catch (IOException ex)
        {
            return new Result<char>(
                default,
                false,
                $"Die Eingabe konnte nicht gelesen werden: {ex.Message}");
        }
    }
}
