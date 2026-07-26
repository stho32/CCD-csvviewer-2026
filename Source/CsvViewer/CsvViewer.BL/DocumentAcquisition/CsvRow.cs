namespace CsvViewer.BL.DocumentAcquisition;

/// <summary>Ein Datensatz einer CSV-Datei: Feldwerte positionsbasiert in Kopfzeilen-Reihenfolge.</summary>
public class CsvRow
{
    private readonly string[] _fields;

    public CsvRow(IEnumerable<string> fields)
    {
        _fields = fields.ToArray();
    }

    public string this[int index] => _fields[index];
    public int FieldCount => _fields.Length;
    public IEnumerator<string> GetEnumerator() => ((IEnumerable<string>)_fields).GetEnumerator();
}
