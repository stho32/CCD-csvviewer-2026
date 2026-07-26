namespace CsvViewer.BL.DocumentAcquisition;

public class CsvRowCollection
{
    private readonly CsvRow[] _rows;

    public CsvRowCollection(IEnumerable<CsvRow> rows)
    {
        _rows = rows.ToArray();
    }

    public CsvRow this[int index] => _rows[index];
    public int RowCount => _rows.Length;
    public IEnumerator<CsvRow> GetEnumerator() => ((IEnumerable<CsvRow>)_rows).GetEnumerator();
}
