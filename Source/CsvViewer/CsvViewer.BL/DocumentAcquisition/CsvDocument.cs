namespace CsvViewer.BL.DocumentAcquisition;

public sealed record CsvDocument(
    CsvHeader Header,
    CsvRowCollection Rows);
