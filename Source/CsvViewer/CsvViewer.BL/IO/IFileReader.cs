using CsvViewer.BL.Common;

namespace CsvViewer.BL.IO;

public interface IFileReader
{
    /// <summary>
    /// Liest als UTF-8; Fehler kommen als Fehler-Result zurück, nie als Exception.
    /// </summary>
    Result<IReadOnlyList<string>> ReadLines(string path);
}
