namespace CsvViewer.BL.Csv;

/// <summary>Kopfzeile einer CSV-Datei: die Spaltennamen in Original-Reihenfolge.</summary>
public class CsvHeader
{
    private readonly string[] _columnNames;

    public CsvHeader(IEnumerable<string> columnNames)
    {
        _columnNames = columnNames.ToArray();
    }

    public string this[int index] => _columnNames[index];
    public int ColumnCount => _columnNames.Length;
    public IEnumerator<string> GetEnumerator() => ((IEnumerable<string>)_columnNames).GetEnumerator();
}
