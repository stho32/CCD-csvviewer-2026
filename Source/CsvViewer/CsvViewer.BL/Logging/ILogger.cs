using CsvViewer.BL.Common;

namespace CsvViewer.BL.Logging;

public interface ILogger
{
    Result Error(string message);
}
