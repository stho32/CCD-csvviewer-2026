using CsvViewer.BL.Common;

namespace CsvViewer.BL.Csv;

/// <summary>
/// Pure Operation nach IODA: wandelt rohe Textzeilen in ein <see cref="CsvDocument"/> um.
/// Kein I/O, keine Seiteneffekte. Trennt Felder am Semikolon, interpretiert die erste
/// Zeile als Kopfzeile und validiert die Struktur. Zellinhalte bleiben unverändert
/// (kein Quoting/Escaping).
/// </summary>
public static class CsvParser
{
    private const char Delimiter = ';';

    public static Result<CsvDocument> Parse(IReadOnlyList<string> lines)
    {
        if (lines is null || lines.Count == 0)
        {
            return new Result<CsvDocument>(
                null, false, "Die CSV-Eingabe ist leer.");
        }

        string[] header = lines[0].Split(Delimiter);

        var rows = new List<IReadOnlyList<string>>(lines.Count - 1);
        for (int i = 1; i < lines.Count; i++)
        {
            string[] fields = lines[i].Split(Delimiter);
            if (fields.Length != header.Length)
            {
                return new Result<CsvDocument>(
                    null,
                    false,
                    $"Zeile {i + 1} hat {fields.Length} Feld(er), erwartet wurden {header.Length} " +
                    "gemäß Kopfzeile.");
            }

            rows.Add(fields);
        }

        var document = new CsvDocument(header, rows);
        return new Result<CsvDocument>(document, true, string.Empty);
    }
}
