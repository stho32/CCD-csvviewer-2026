using CsvViewer.BL.DocumentAcquisition;

namespace CsvViewer.BL.PagePresentation;

public class CsvPageCollection
{
    private readonly CsvRowCollection[] _pages;

    public CsvPageCollection(IEnumerable<CsvRowCollection> pages)
    {
        _pages = pages.ToArray();
    }

    public CsvRowCollection this[int index] => _pages[index];
    public int PageCount => _pages.Length;
    public IEnumerator<CsvRowCollection> GetEnumerator() => ((IEnumerable<CsvRowCollection>)_pages).GetEnumerator();
}
