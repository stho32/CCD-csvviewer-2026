using CsvViewer.BL.Common;
using CsvViewer.BL.DocumentAcquisition.Data;

namespace CsvViewer.BL.DocumentAcquisition;

/// <summary>
/// Erste Zeile = Kopfzeile; Zellinhalte bleiben unverändert (kein Quoting/Escaping) — R00001.
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
                int lineNumber = i + 1;
                return new Result<CsvDocument>(
                    null,
                    false,
                    $"Zeile {lineNumber} hat {fields.Length} Feld(er), erwartet wurden {header.ColumnCount} " +
                    "gemäß Kopfzeile.");
            }

            rows.Add(new CsvRow(fields));
        }

        var document = new CsvDocument(header, new CsvRowCollection(rows));
        return new Result<CsvDocument>(document, true, string.Empty);
    }
}
