using CsvViewer.BL.Common;

namespace CsvViewer.BL.HostContracts;

public interface ILogger
{
    Result Error(string message);
}
