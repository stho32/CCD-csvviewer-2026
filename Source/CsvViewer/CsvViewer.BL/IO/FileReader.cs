using System.Text;
using CsvViewer.BL.Common;

namespace CsvViewer.BL.IO;

public sealed class FileReader : IFileReader
{
    public Result<IReadOnlyList<string>> ReadLines(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return new Result<IReadOnlyList<string>>(
                null, false, "Der Dateipfad ist leer.");
        }

        if (!File.Exists(path))
        {
            return new Result<IReadOnlyList<string>>(
                null, false, $"Die Datei '{path}' wurde nicht gefunden.");
        }

        try
        {
            IReadOnlyList<string> lines = File.ReadAllLines(path, Encoding.UTF8);
            return new Result<IReadOnlyList<string>>(lines, true, string.Empty);
        }
        catch (Exception ex) when (
            ex is IOException
            or UnauthorizedAccessException
            or System.Security.SecurityException
            or NotSupportedException)
        {
            return new Result<IReadOnlyList<string>>(
                null, false, $"Die Datei '{path}' konnte nicht gelesen werden: {ex.Message}");
        }
    }
}
