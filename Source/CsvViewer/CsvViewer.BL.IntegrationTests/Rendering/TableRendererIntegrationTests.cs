using CsvViewer.BL.Common;
using CsvViewer.BL.Csv;
using CsvViewer.BL.Rendering;

namespace CsvViewer.BL.IntegrationTests.Rendering;

public class TableRendererIntegrationTests
{
    [Test]
    public void Wenn_GeparsteCsvLeereZellenEnthaelt_dann_RendererErhaeltPositionenUndAbstaende()
    {
        // Arrange
        var lines = new[]
        {
            "Name;Stadt;Land",
            "Ada;;UK",
            "Grace;New York;USA",
        };
        var renderer = new TableRenderer();

        // Act
        Result<CsvDocument> parseResult = CsvParser.Parse(lines);
        string renderedTable = renderer.Render(parseResult.Value!);

        // Assert
        Assert.That(parseResult.IsSuccess, Is.True);
        string expected = JoinLines(
            "Name |Stadt   |Land|",
            "-----+--------+----+",
            "Ada  |        |UK  |",
            "Grace|New York|USA |");
        Assert.That(renderedTable, Is.EqualTo(expected));
    }

    [Test]
    public void Wenn_GeparsteCsvNurHeaderEnthaelt_dann_RendererErzeugtLeereSeite()
    {
        // Arrange
        var lines = new[] { "Vorname;Nachname" };
        var renderer = new TableRenderer();

        // Act
        Result<CsvDocument> parseResult = CsvParser.Parse(lines);
        string renderedTable = renderer.Render(parseResult.Value!);

        // Assert
        Assert.That(parseResult.IsSuccess, Is.True);
        Assert.That(
            renderedTable,
            Is.EqualTo(JoinLines(
                "Vorname|Nachname|",
                "-------+--------+")));
    }

    private static string JoinLines(params string[] lines)
    {
        return string.Join(Environment.NewLine, lines);
    }
}
