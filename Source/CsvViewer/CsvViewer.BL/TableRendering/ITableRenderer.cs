using CsvViewer.BL.Common;
using CsvViewer.BL.DocumentAcquisition.Data;
using CsvViewer.BL.DocumentAcquisition;

namespace CsvViewer.BL.TableRendering;

public interface ITableRenderer
{
    Result<string> Render(CsvHeader? header, CsvRowCollection? rows);
}
