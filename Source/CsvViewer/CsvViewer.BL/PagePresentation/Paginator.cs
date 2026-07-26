using CsvViewer.BL.Common;
using CsvViewer.BL.DocumentAcquisition.Data;
using CsvViewer.BL.DocumentAcquisition;
using CsvViewer.BL.PagePresentation.Data;

namespace CsvViewer.BL.PagePresentation;

public static class Paginator
{
    public static Result<PagedDocument> Paginate(
        CsvDocument? document,
        int pageSize)
    {
        if (document is null)
        {
            return new Result<PagedDocument>(
                null,
                false,
                "Das CSV-Dokument fehlt.");
        }

        if (pageSize <= 0)
        {
            return new Result<PagedDocument>(
                null,
                false,
                "Die Seitengröße muss größer als 0 sein.");
        }

        var pages = new List<CsvRowCollection>();

        for (int offset = 0; offset < document.Rows.RowCount; offset += pageSize)
        {
            int rowsOnPage = Math.Min(pageSize, document.Rows.RowCount - offset);
            var rows = new CsvRow[rowsOnPage];

            for (int rowIndex = 0; rowIndex < rowsOnPage; rowIndex++)
            {
                rows[rowIndex] = document.Rows[offset + rowIndex];
            }

            pages.Add(new CsvRowCollection(rows));
        }

        if (pages.Count == 0)
        {
            pages.Add(new CsvRowCollection(Array.Empty<CsvRow>()));
        }

        return new Result<PagedDocument>(
            new PagedDocument(document.Header, new CsvPageCollection(pages)),
            true,
            string.Empty);
    }
}
