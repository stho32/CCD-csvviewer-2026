using System.Text;

namespace CsvViewer.BL.IntegrationTests.TestFiles;

internal static class TemporaryCsv
{
    public static string Write(params string[] lines)
    {
        string path = Path.Combine(
            Path.GetTempPath(),
            $"CsvViewer_test_{Guid.NewGuid():N}.csv");
        File.WriteAllLines(path, lines, Encoding.UTF8);
        return path;
    }
}
