using CsvViewer.BL.Csv;

namespace CsvViewer.BL.Paging;

/// <summary>Ein CSV-Dokument in paginierter Form: der Header existiert genau einmal, jede Seite ist eine Zeilen-Teilmenge in Leserichtung.</summary>
public sealed record PagedDocument(
    CsvHeader Header,
    CsvPageCollection Pages);
