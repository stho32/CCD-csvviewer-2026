namespace CsvViewer.BL.Csv;

public sealed record CsvDocument(
    CsvHeader Header,
    CsvRowCollection Rows);
