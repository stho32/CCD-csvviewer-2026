using CsvViewer.BL.DocumentAcquisition;
using CsvViewer.BL.PagePresentation;

namespace CsvViewer.BL.Tests.Rendering;

public class TableRendererTests
{
    [Test]
    public void Wenn_KursbeispielGerendertWird_dann_AusgabeIstZeichengenau()
    {
        // Arrange
        CsvHeader header = CreateHeader("Name", "Age", "City");
        CsvRowCollection rows = CreateRows(
            ["Peter", "42", "New York"],
            ["Paul", "57", "London"],
            ["Mary", "35", "Munich"]);
        var renderer = new TableRenderer();

        // Act
        string result = renderer.Render(header, rows).Value!;

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
        CsvHeader header = CreateHeader("LangerHeader", "B");
        CsvRowCollection rows = CreateRows(["kurz", "x"]);
        var renderer = new TableRenderer();

        // Act
        string result = renderer.Render(header, rows).Value!;

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
        CsvHeader header = CreateHeader("A", "B");
        CsvRowCollection rows = CreateRows(["laengster", "x"]);
        var renderer = new TableRenderer();

        // Act
        string result = renderer.Render(header, rows).Value!;

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
        CsvHeader header = CreateHeader("Name");
        CsvRowCollection shortRows = CreateRows(["Ada"]);
        CsvRowCollection wideRows = CreateRows(["Alexandria"]);
        var renderer = new TableRenderer();

        // Act
        string shortResult = renderer.Render(header, shortRows).Value!;
        string wideResult = renderer.Render(header, wideRows).Value!;

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
        CsvHeader header = CreateHeader("Name", "Alter");
        CsvRowCollection rows = CreateRows();
        var renderer = new TableRenderer();

        // Act
        string result = renderer.Render(header, rows).Value!;

        // Assert
        Assert.That(
            result,
            Is.EqualTo(JoinLines("Name|Alter|", "----+-----+")));
    }

    [Test]
    public void Wenn_ZellwertLeerIst_dann_ZelleBestehtAusAuffuellLeerzeichen()
    {
        // Arrange
        CsvHeader header = CreateHeader("A", "Laenge");
        CsvRowCollection rows = CreateRows(
            ["x", ""],
            ["", "Wert"]);
        var renderer = new TableRenderer();

        // Act
        string result = renderer.Render(header, rows).Value!;

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
        CsvHeader header = CreateHeader("Wert");
        CsvRowCollection rows = CreateRows(["eins"], ["zwei"]);
        var renderer = new TableRenderer();

        // Act
        string result = renderer.Render(header, rows).Value!;

        // Assert
        Assert.That(
            result,
            Is.EqualTo(JoinLines("Wert|", "----+", "eins|", "zwei|")));
    }

    [Test]
    public void Wenn_SeiteVierSpaltenHat_dann_AlleSpaltenWerdenPositionsbasiertGerendert()
    {
        // Arrange
        CsvHeader header = CreateHeader("A", "BB", "C", "DD");
        CsvRowCollection rows = CreateRows(["1", "2", "333", "4"]);
        var renderer = new TableRenderer();

        // Act
        string result = renderer.Render(header, rows).Value!;

        // Assert
        string expected = JoinLines(
            "A|BB|C  |DD|",
            "-+--+---+--+",
            "1|2 |333|4 |");
        Assert.That(result, Is.EqualTo(expected));
    }

    private static CsvHeader CreateHeader(params string[] columnNames)
    {
        return new CsvHeader(columnNames);
    }

    private static CsvRowCollection CreateRows(params string[][] rows)
    {
        return new CsvRowCollection(rows.Select(fields => new CsvRow(fields)));
    }

    private static string JoinLines(params string[] lines)
    {
        return string.Join(Environment.NewLine, lines);
    }
}
