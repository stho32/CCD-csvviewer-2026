using CsvViewer.BL.Common;
using CsvViewer.BL.Csv;

namespace CsvViewer.BL.Paging;

public static class Paginator
{
    public static Result<IReadOnlyList<CsvDocument>> Paginate(
        CsvDocument? document,
        int pageSize)
    {
        if (document is null)
        {
            return new Result<IReadOnlyList<CsvDocument>>(
                null,
                false,
                "Das CSV-Dokument fehlt.");
        }

        if (pageSize <= 0)
        {
            return new Result<IReadOnlyList<CsvDocument>>(
                null,
                false,
                "Die Seitengröße muss größer als 0 sein.");
        }

        var pages = new List<CsvDocument>();

        for (int offset = 0; offset < document.Rows.RowCount; offset += pageSize)
        {
            int rowsOnPage = Math.Min(pageSize, document.Rows.RowCount - offset);
            var rows = new CsvRow[rowsOnPage];

            for (int rowIndex = 0; rowIndex < rowsOnPage; rowIndex++)
            {
                rows[rowIndex] = document.Rows[offset + rowIndex];
            }

            pages.Add(new CsvDocument(document.Header, new CsvRowCollection(rows)));
        }

        if (pages.Count == 0)
        {
            pages.Add(
                new CsvDocument(
                    document.Header,
                    new CsvRowCollection(Array.Empty<CsvRow>())));
        }

        return new Result<IReadOnlyList<CsvDocument>>(pages, true, string.Empty);
    }
}
