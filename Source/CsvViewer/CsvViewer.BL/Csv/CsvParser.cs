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

        var header = new CsvHeader(lines[0].Split(Delimiter));

        var rows = new List<CsvRow>(lines.Count - 1);
        for (int i = 1; i < lines.Count; i++)
        {
            string[] fields = lines[i].Split(Delimiter);
            if (fields.Length != header.ColumnCount)
            {
                return new Result<CsvDocument>(
                    null,
                    false,
                    $"Zeile {i + 1} hat {fields.Length} Feld(er), erwartet wurden {header.ColumnCount} " +
                    "gemäß Kopfzeile.");
            }

            rows.Add(new CsvRow(fields));
        }

        var document = new CsvDocument(header, new CsvRowCollection(rows));
        return new Result<CsvDocument>(document, true, string.Empty);
    }
}
