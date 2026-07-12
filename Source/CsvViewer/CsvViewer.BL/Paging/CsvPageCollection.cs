using CsvViewer.BL.Csv;

namespace CsvViewer.BL.Paging;

public class CsvPageCollection
{
    private readonly CsvDocument[] _pages;

    public CsvPageCollection(IEnumerable<CsvDocument> pages)
    {
        _pages = pages.ToArray();
    }

    public CsvDocument this[int index] => _pages[index];
    public int PageCount => _pages.Length;
    public IEnumerator<CsvDocument> GetEnumerator() => ((IEnumerable<CsvDocument>)_pages).GetEnumerator();
}
