using CsvViewer.BL.Common;
using CsvViewer.BL.DocumentAcquisition.Data;
using CsvViewer.BL.DocumentAcquisition;
using CsvViewer.BL.TableRendering;

namespace CsvViewer.BL.Tests.Mocks;

internal sealed class FailingTableRenderer : ITableRenderer
{
    public int RenderCount { get; private set; }

    public Result<string> Render(CsvHeader? header, CsvRowCollection? rows)
    {
        RenderCount++;
        return new Result<string>(null, false, "Testfehler beim Rendern.");
    }
}
