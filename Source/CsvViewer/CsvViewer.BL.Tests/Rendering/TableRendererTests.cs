using CsvViewer.BL.Csv;
using CsvViewer.BL.Rendering;

namespace CsvViewer.BL.Tests.Rendering;

public class TableRendererTests
{
    [Test]
    public void Wenn_KursbeispielGerendertWird_dann_AusgabeIstZeichengenau()
    {
        // Arrange
        CsvDocument page = CreatePage(
            ["Name", "Age", "City"],
            ["Peter", "42", "New York"],
            ["Paul", "57", "London"],
            ["Mary", "35", "Munich"]);
        var renderer = new TableRenderer();

        // Act
        string result = renderer.Render(page);

        // Assert
        string expected = JoinLines(
            "Name |Age|City    |",
            "-----+---+--------+",
            "Peter|42 |New York|",
            "Paul |57 |London  |",
            "Mary |35 |Munich  |");
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void Wenn_HeaderLaengerAlsAlleDatenwerteIst_dann_HeaderBestimmtSpaltenbreite()
    {
        // Arrange
        CsvDocument page = CreatePage(
            ["LangerHeader", "B"],
            ["kurz", "x"]);
        var renderer = new TableRenderer();

        // Act
        string result = renderer.Render(page);

        // Assert
        string expected = JoinLines(
            "LangerHeader|B|",
            "------------+-+",
            "kurz        |x|");
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void Wenn_DatenwertLaengerAlsHeaderIst_dann_DatenwertBestimmtSpaltenbreite()
    {
        // Arrange
        CsvDocument page = CreatePage(
            ["A", "B"],
            ["laengster", "x"]);
        var renderer = new TableRenderer();

        // Act
        string result = renderer.Render(page);

        // Assert
        string expected = JoinLines(
            "A        |B|",
            "---------+-+",
            "laengster|x|");
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void Wenn_ZweiSeitenUnterschiedlichLangeWerteHaben_dann_BreitenWerdenJeSeiteBerechnet()
    {
        // Arrange
        CsvDocument shortPage = CreatePage(["Name"], ["Ada"]);
        CsvDocument widePage = CreatePage(["Name"], ["Alexandria"]);
        var renderer = new TableRenderer();

        // Act
        string shortResult = renderer.Render(shortPage);
        string wideResult = renderer.Render(widePage);

        // Assert
        Assert.That(
            shortResult,
            Is.EqualTo(JoinLines("Name|", "----+", "Ada |")));
        Assert.That(
            wideResult,
            Is.EqualTo(JoinLines("Name      |", "----------+", "Alexandria|")));
    }

    [Test]
    public void Wenn_SeiteKeineDatenzeilenHat_dann_NurHeaderUndTrennlinieWerdenGerendert()
    {
        // Arrange
        CsvDocument page = CreatePage(["Name", "Alter"]);
        var renderer = new TableRenderer();

        // Act
        string result = renderer.Render(page);

        // Assert
        Assert.That(
            result,
            Is.EqualTo(JoinLines("Name|Alter|", "----+-----+")));
    }

    [Test]
    public void Wenn_ZellwertLeerIst_dann_ZelleBestehtAusAuffuellLeerzeichen()
    {
        // Arrange
        CsvDocument page = CreatePage(
            ["A", "Laenge"],
            ["x", ""],
            ["", "Wert"]);
        var renderer = new TableRenderer();

        // Act
        string result = renderer.Render(page);

        // Assert
        string expected = JoinLines(
            "A|Laenge|",
            "-+------+",
            "x|      |",
            " |Wert  |");
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void Wenn_SeiteEineSpalteHat_dann_EineSpalteWirdPositionsbasiertGerendert()
    {
        // Arrange
        CsvDocument page = CreatePage(["Wert"], ["eins"], ["zwei"]);
        var renderer = new TableRenderer();

        // Act
        string result = renderer.Render(page);

        // Assert
        Assert.That(
            result,
            Is.EqualTo(JoinLines("Wert|", "----+", "eins|", "zwei|")));
    }

    [Test]
    public void Wenn_SeiteVierSpaltenHat_dann_AlleSpaltenWerdenPositionsbasiertGerendert()
    {
        // Arrange
        CsvDocument page = CreatePage(
            ["A", "BB", "C", "DD"],
            ["1", "2", "333", "4"]);
        var renderer = new TableRenderer();

        // Act
        string result = renderer.Render(page);

        // Assert
        string expected = JoinLines(
            "A|BB|C  |DD|",
            "-+--+---+--+",
            "1|2 |333|4 |");
        Assert.That(result, Is.EqualTo(expected));
    }

    private static CsvDocument CreatePage(
        string[] header,
        params string[][] rows)
    {
        return new CsvDocument(
            new CsvHeader(header),
            new CsvRowCollection(rows.Select(fields => new CsvRow(fields))));
    }

    private static string JoinLines(params string[] lines)
    {
        return string.Join(Environment.NewLine, lines);
    }
}
