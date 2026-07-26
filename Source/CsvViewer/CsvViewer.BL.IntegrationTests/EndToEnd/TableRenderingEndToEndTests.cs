using CsvViewer.BL.Common;
using CsvViewer.BL.Csv;
using CsvViewer.BL.IntegrationTests.TestFiles;
using CsvViewer.BL.IO;
using CsvViewer.BL.Rendering;

namespace CsvViewer.BL.IntegrationTests.EndToEnd;

public class TableRenderingEndToEndTests
{
    [Test]
    public void Wenn_GueltigeCsvDateiGerendertWird_dann_NutzerErhaeltLesbareTabelle()
    {
        // Arrange
        string path = TemporaryCsv.Write(
            "Name;Age;City",
            "Peter;42;New York",
            "Paul;57;London",
            "Mary;35;Munich");

        try
        {
            // Act
            string renderedTable = ReadParseAndRender(path);

            // Assert
            string expected = JoinLines(
                "Name |Age|City    |",
                "-----+---+--------+",
                "Peter|42 |New York|",
                "Paul |57 |London  |",
                "Mary |35 |Munich  |");
            Assert.That(renderedTable, Is.EqualTo(expected));
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Test]
    public void Wenn_DateienUnterschiedlichLangeSeitenwerteHaben_dann_BreitenPassenSichJeDateiAn()
    {
        // Arrange
        string shortPagePath = TemporaryCsv.Write("Name", "Ada");
        string widePagePath = TemporaryCsv.Write("Name", "Alexandria");

        try
        {
            // Act
            string shortTable = ReadParseAndRender(shortPagePath);
            string wideTable = ReadParseAndRender(widePagePath);

            // Assert
            Assert.That(
                shortTable,
                Is.EqualTo(JoinLines("Name|", "----+", "Ada |")));
            Assert.That(
                wideTable,
                Is.EqualTo(JoinLines("Name      |", "----------+", "Alexandria|")));
        }
        finally
        {
            File.Delete(shortPagePath);
            File.Delete(widePagePath);
        }
    }

    [Test]
    public void Wenn_CsvDateienVerschiedeneSpaltenzahlenHaben_dann_BeideWerdenGenerischGerendert()
    {
        // Arrange
        string oneColumnPath = TemporaryCsv.Write("Wert", "eins");
        string fourColumnPath = TemporaryCsv.Write(
            "A;BB;C;DD",
            "1;2;333;4");

        try
        {
            // Act
            string oneColumnTable = ReadParseAndRender(oneColumnPath);
            string fourColumnTable = ReadParseAndRender(fourColumnPath);

            // Assert
            Assert.That(
                oneColumnTable,
                Is.EqualTo(JoinLines("Wert|", "----+", "eins|")));
            Assert.That(
                fourColumnTable,
                Is.EqualTo(JoinLines(
                    "A|BB|C  |DD|",
                    "-+--+---+--+",
                    "1|2 |333|4 |")));
        }
        finally
        {
            File.Delete(oneColumnPath);
            File.Delete(fourColumnPath);
        }
    }

    private static string ReadParseAndRender(string path)
    {
        Result<IReadOnlyList<string>> readResult = new FileReader().ReadLines(path);
        Assert.That(readResult.IsSuccess, Is.True, readResult.Message);

        Result<CsvDocument> parseResult = CsvParser.Parse(readResult.Value!);
        Assert.That(parseResult.IsSuccess, Is.True, parseResult.Message);

        Result<string> renderResult = new TableRenderer()
            .Render(parseResult.Value!.Header, parseResult.Value.Rows);
        Assert.That(renderResult.IsSuccess, Is.True, renderResult.Message);

        return renderResult.Value!;
    }

    private static string JoinLines(params string[] lines)
    {
        return string.Join(Environment.NewLine, lines);
    }
}
