using CsvViewer.BL.Common;
using CsvViewer.BL.Csv;

namespace CsvViewer.BL.Rendering;

public interface ITableRenderer
{
    Result<string> Render(CsvHeader? header, CsvRowCollection? rows);
}
