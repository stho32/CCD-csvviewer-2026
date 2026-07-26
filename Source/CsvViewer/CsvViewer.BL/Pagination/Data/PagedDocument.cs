using CsvViewer.BL.DocumentAcquisition.Data;
using CsvViewer.BL.DocumentAcquisition;

namespace CsvViewer.BL.Pagination.Data;

/// <summary>Ein CSV-Dokument in paginierter Form: der Header existiert genau einmal, jede Seite ist eine Zeilen-Teilmenge in Leserichtung.</summary>
public sealed record PagedDocument(
    CsvHeader Header,
    CsvPageCollection Pages);
