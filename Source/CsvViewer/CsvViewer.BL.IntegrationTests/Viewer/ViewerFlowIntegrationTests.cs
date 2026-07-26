using System.Text;
using CsvViewer.BL.Common;
using CsvViewer.BL.DocumentAcquisition;
using CsvViewer.BL.IntegrationTests.Mocks;
using CsvViewer.BL.IntegrationTests.TestFiles;
using CsvViewer.BL.Interaction;
using CsvViewer.BL.PagePresentation;
using CsvViewer.HostSpecific.IO;

namespace CsvViewer.BL.IntegrationTests.Viewer;

public class ViewerFlowIntegrationTests
{
    [Test]
    public void Wenn_ElfZeilenOhneSeitengroesseGeoeffnetWerden_dann_ErsteZehnWerdenAngezeigt()
    {
        // Arrange
        string path = TemporaryCsv.Write(
            ["Nummer", .. Enumerable.Range(1, 11).Select(number => number.ToString())]);
        string contentsBefore = File.ReadAllText(path, Encoding.UTF8);
        var console = new TestConsole('E');

        try
        {
            // Act
            Result result = RunViewer([path], console);

            // Assert
            Assert.That(result.IsSuccess, Is.True, result.Message);
            Assert.That(console.ClearCount, Is.EqualTo(1));
            Assert.That(console.WrittenTexts, Has.Count.EqualTo(1));
            Assert.That(console.WrittenTexts[0], Does.Contain("1 "));
            Assert.That(console.WrittenTexts[0], Does.Contain("10"));
            Assert.That(console.WrittenTexts[0], Does.Not.Contain("11"));
            Assert.That(console.WrittenTexts[0], Does.Contain(InteractiveViewer.Menu));
            Assert.That(File.ReadAllText(path, Encoding.UTF8), Is.EqualTo(contentsBefore));
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Test]
    public void Wenn_EigeneSeitengroesseVerwendetUndNextGedruecktWird_dann_RestseiteWirdAngezeigt()
    {
        // Arrange
        string path = TemporaryCsv.Write(["Wert", "eins", "zwei", "drei"]);
        string contentsBefore = File.ReadAllText(path, Encoding.UTF8);
        var console = new TestConsole('N', 'E');

        try
        {
            // Act
            Result result = RunViewer([path, "2"], console);

            // Assert
            Assert.That(result.IsSuccess, Is.True, result.Message);
            Assert.That(console.ClearCount, Is.EqualTo(2));
            Assert.That(console.WrittenTexts, Has.Count.EqualTo(2));
            Assert.That(console.WrittenTexts[0], Does.Contain("eins"));
            Assert.That(console.WrittenTexts[0], Does.Contain("zwei"));
            Assert.That(console.WrittenTexts[0], Does.Not.Contain("drei"));
            Assert.That(console.WrittenTexts[1], Does.Contain("drei"));
            Assert.That(console.WrittenTexts[1], Does.Not.Contain("eins"));
            Assert.That(File.ReadAllText(path, Encoding.UTF8), Is.EqualTo(contentsBefore));
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Test]
    public void Wenn_DateiNurHeaderEnthaelt_dann_EineLeereSeiteMitMenueWirdAngezeigt()
    {
        // Arrange
        string path = TemporaryCsv.Write(["Vorname;Nachname"]);
        var console = new TestConsole('E');

        try
        {
            // Act
            Result result = RunViewer([path], console);

            // Assert
            Assert.That(result.IsSuccess, Is.True, result.Message);
            Assert.That(console.WrittenTexts, Has.Count.EqualTo(1));
            string expectedTable = string.Join(
                Environment.NewLine,
                "Vorname|Nachname|",
                "-------+--------+");
            Assert.That(console.WrittenTexts[0], Does.StartWith(expectedTable));
            Assert.That(console.WrittenTexts[0], Does.EndWith(
                $"{InteractiveViewer.Menu}{Environment.NewLine}"));
        }
        finally
        {
            File.Delete(path);
        }
    }

    private static Result RunViewer(string[] args, TestConsole console)
    {
        Result<ViewerArguments> argumentsResult = ArgumentsParser.Parse(args);
        if (!argumentsResult.IsSuccess)
        {
            return new Result(false, argumentsResult.Message);
        }

        Result<IReadOnlyList<string>> readResult =
            new FileReader().ReadLines(argumentsResult.Value!.FilePath);
        if (!readResult.IsSuccess)
        {
            return new Result(false, readResult.Message);
        }

        Result<CsvDocument> parseResult = CsvParser.Parse(readResult.Value!);
        if (!parseResult.IsSuccess)
        {
            return new Result(false, parseResult.Message);
        }

        Result<PagedDocument> pagedResult =
            Paginator.Paginate(parseResult.Value, argumentsResult.Value.PageSize);
        if (!pagedResult.IsSuccess)
        {
            return new Result(false, pagedResult.Message);
        }

        return new InteractiveViewer(console, new TableRenderer()).Run(pagedResult.Value);
    }
}
