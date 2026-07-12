using CsvViewer.BL.Common;

namespace CsvViewer.BL.IO;

public interface IConsole
{
    Result Clear();
    Result Write(string text);
    Result<char> ReadKey();
}
