using CsvViewer.BL.Common;

namespace CsvViewer.BL.Logging;

public interface ILogger
{
    Result Info(string message);
    Result Error(string message);
}
