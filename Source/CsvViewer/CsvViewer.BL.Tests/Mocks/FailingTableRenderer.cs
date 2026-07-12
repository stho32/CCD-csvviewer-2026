using CsvViewer.BL.Common;
using CsvViewer.BL.Csv;
using CsvViewer.BL.Rendering;

namespace CsvViewer.BL.Tests.Mocks;

internal sealed class FailingTableRenderer : ITableRenderer
{
    public int RenderCount { get; private set; }

    public Result<string> Render(CsvDocument? page)
    {
        RenderCount++;
        return new Result<string>(null, false, "Testfehler beim Rendern.");
    }
}
