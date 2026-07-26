using CsvViewer.BL.Common;

namespace CsvViewer.BL.HostContracts;

public interface IConsole
{
    Result Clear();
    Result Write(string text);
    Result<char> ReadKey();
}
