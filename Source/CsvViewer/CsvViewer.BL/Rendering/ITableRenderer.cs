using CsvViewer.BL.Csv;

namespace CsvViewer.BL.Rendering;

public interface ITableRenderer
{
    string Render(CsvDocument page);
}
