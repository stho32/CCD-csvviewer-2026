namespace CsvViewer.BL.Csv;

/// <summary>
/// Generische, fachlich neutrale Repräsentation einer eingelesenen CSV-Datei.
/// Data-Baustein nach IODA: hält ausschließlich Daten, keine Logik.
/// </summary>
/// <param name="Header">Kopfzeile mit den Spaltennamen in Original-Reihenfolge.</param>
/// <param name="Rows">
/// Geordnete Datensätze. Jeder Datensatz enthält seine Feldwerte positionsbasiert
/// in Kopfzeilen-Reihenfolge. Keine Bindung an ein konkretes Fachmodell.
/// </param>
public sealed record CsvDocument(
    CsvHeader Header,
    CsvRowCollection Rows);
