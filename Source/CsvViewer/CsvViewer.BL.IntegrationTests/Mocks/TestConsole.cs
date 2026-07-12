using CsvViewer.BL.Common;
using CsvViewer.BL.IO;

namespace CsvViewer.BL.IntegrationTests.Mocks;

internal sealed class TestConsole : IConsole
{
    private readonly Queue<char> _keys;

    public TestConsole(params char[] keys)
    {
        _keys = new Queue<char>(keys);
    }

    public int ClearCount { get; private set; }
    public List<string> WrittenTexts { get; } = [];

    public Result Clear()
    {
        ClearCount++;
        return new Result(true, string.Empty);
    }

    public Result Write(string text)
    {
        WrittenTexts.Add(text);
        return new Result(true, string.Empty);
    }

    public Result<char> ReadKey()
    {
        if (_keys.Count == 0)
        {
            return new Result<char>(default, false, "Keine Testtaste verfügbar.");
        }

        return new Result<char>(_keys.Dequeue(), true, string.Empty);
    }
}
