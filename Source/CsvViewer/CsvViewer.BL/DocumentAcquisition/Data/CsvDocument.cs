namespace CsvViewer.BL.DocumentAcquisition.Data;

public sealed record CsvDocument(
    CsvHeader Header,
    CsvRowCollection Rows);
