using CsvViewer.BL.Common;

namespace CsvViewer.BL.IO;

/// <summary>
/// CSV-agnostischer Vertrag zum Einlesen einer Datei als Textzeilen.
/// Kennt kein CSV-Format — liefert nur die rohen UTF-8-Zeilen.
/// </summary>
public interface IFileReader
{
    /// <summary>
    /// Liest die Datei am angegebenen Pfad als UTF-8 und liefert ihre Zeilen.
    /// Fehler (fehlende/nicht lesbare Datei) werden als Fehler-<see cref="Result{T}"/>
    /// signalisiert, nicht als Exception über die Bausteingrenze.
    /// </summary>
    Result<IReadOnlyList<string>> ReadLines(string path);
}
