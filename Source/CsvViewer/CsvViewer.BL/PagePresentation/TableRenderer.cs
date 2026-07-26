using System.Text;
using CsvViewer.BL.Common;
using CsvViewer.BL.DocumentAcquisition;

namespace CsvViewer.BL.PagePresentation;

public sealed class TableRenderer : ITableRenderer
{
    public Result<string> Render(CsvHeader? header, CsvRowCollection? rows)
    {
        if (header is null || rows is null)
        {
            return new Result<string>(null, false, "Die zu rendernde Seite fehlt.");
        }

        int[] columnWidths = CalculateColumnWidths(header, rows);
        var lines = new string[rows.RowCount + 2];

        lines[0] = BuildValueLine(
            columnWidths,
            columnIndex => header[columnIndex]);
        lines[1] = BuildSeparatorLine(columnWidths);

        for (int rowIndex = 0; rowIndex < rows.RowCount; rowIndex++)
        {
            CsvRow row = rows[rowIndex];
            lines[rowIndex + 2] = BuildValueLine(
                columnWidths,
                columnIndex => row[columnIndex]);
        }

        return new Result<string>(
            string.Join(Environment.NewLine, lines),
            true,
            string.Empty);
    }

    private static int[] CalculateColumnWidths(CsvHeader header, CsvRowCollection rows)
    {
        var columnWidths = new int[header.ColumnCount];

        for (int columnIndex = 0; columnIndex < header.ColumnCount; columnIndex++)
        {
            columnWidths[columnIndex] = header[columnIndex].Length;
        }

        for (int rowIndex = 0; rowIndex < rows.RowCount; rowIndex++)
        {
            CsvRow row = rows[rowIndex];
            for (int columnIndex = 0; columnIndex < header.ColumnCount; columnIndex++)
            {
                columnWidths[columnIndex] = Math.Max(
                    columnWidths[columnIndex],
                    row[columnIndex].Length);
            }
        }

        return columnWidths;
    }

    private static string BuildValueLine(
        IReadOnlyList<int> columnWidths,
        Func<int, string> getValue)
    {
        var line = new StringBuilder();

        for (int columnIndex = 0; columnIndex < columnWidths.Count; columnIndex++)
        {
            line.Append(getValue(columnIndex).PadRight(columnWidths[columnIndex]));
            line.Append('|');
        }

        return line.ToString();
    }

    private static string BuildSeparatorLine(IReadOnlyList<int> columnWidths)
    {
        var line = new StringBuilder();

        foreach (int columnWidth in columnWidths)
        {
            line.Append('-', columnWidth);
            line.Append('+');
        }

        return line.ToString();
    }
}
