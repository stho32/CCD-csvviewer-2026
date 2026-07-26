using CsvViewer.BL.Common;
using CsvViewer.BL.DocumentAcquisition.Data;
using CsvViewer.BL.DocumentAcquisition;

namespace CsvViewer.BL.Tests.Csv;

public class CsvParserTests
{
    [Test]
    public void Wenn_HeaderUndDatenzeilen_dann_DokumentMitZeilenInReihenfolge()
    {
        // Arrange
        var lines = new List<string>
        {
            "Vorname;Nachname;Alter",
            "Anna;Meier;30",
            "Ben;Schmidt;25",
            "Clara;Weber;41",
        };

        // Act
        Result<CsvDocument> result = CsvParser.Parse(lines);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.Not.Null);
        AssertHeader(result.Value!.Header, "Vorname", "Nachname", "Alter");
        Assert.That(result.Value.Rows.RowCount, Is.EqualTo(3));
        AssertRow(result.Value.Rows[0], "Anna", "Meier", "30");
        AssertRow(result.Value.Rows[1], "Ben", "Schmidt", "25");
        AssertRow(result.Value.Rows[2], "Clara", "Weber", "41");
    }

    [Test]
    public void Wenn_GueltigesDokument_dann_FeldwertePositionsbasiertZugreifbar()
    {
        // Arrange
        var lines = new List<string>
        {
            "Spalte0;Spalte1;Spalte2",
            "wert0;wert1;wert2",
        };

        // Act
        Result<CsvDocument> result = CsvParser.Parse(lines);

        // Assert — Zugriff rein über Position (Kopfzeilen-Reihenfolge), ohne Fachmodell
        Assert.That(result.IsSuccess, Is.True);
        CsvRow row = result.Value!.Rows[0];
        Assert.That(row[0], Is.EqualTo("wert0"));
        Assert.That(row[1], Is.EqualTo("wert1"));
        Assert.That(row[2], Is.EqualTo("wert2"));
    }

    [Test]
    public void Wenn_LeereEingabe_dann_FehlerResult()
    {
        // Arrange
        var lines = new List<string>();

        // Act
        Result<CsvDocument> result = CsvParser.Parse(lines);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Value, Is.Null);
        Assert.That(result.Message, Does.Contain("leer"));
    }

    [Test]
    public void Wenn_NullEingabe_dann_FehlerResult()
    {
        // Act
        Result<CsvDocument> result = CsvParser.Parse(null!);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Value, Is.Null);
    }

    [Test]
    public void Wenn_NurKopfzeile_dann_GueltigesDokumentMitNullDatensaetzen()
    {
        // Arrange
        var lines = new List<string> { "Vorname;Nachname;Alter" };

        // Act
        Result<CsvDocument> result = CsvParser.Parse(lines);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.Not.Null);
        AssertHeader(result.Value!.Header, "Vorname", "Nachname", "Alter");
        Assert.That(result.Value.Rows.RowCount, Is.EqualTo(0));
    }

    [Test]
    public void Wenn_DatenzeileMitZuWenigFeldern_dann_FehlerResult()
    {
        // Arrange
        var lines = new List<string>
        {
            "Vorname;Nachname;Alter",
            "Anna;Meier",
        };

        // Act
        Result<CsvDocument> result = CsvParser.Parse(lines);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Value, Is.Null);
        Assert.That(result.Message, Does.Contain("Zeile 2"));
    }

    [Test]
    public void Wenn_DatenzeileMitZuVielenFeldern_dann_FehlerResult()
    {
        // Arrange
        var lines = new List<string>
        {
            "Vorname;Nachname",
            "Anna;Meier;30",
        };

        // Act
        Result<CsvDocument> result = CsvParser.Parse(lines);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Value, Is.Null);
        Assert.That(result.Message, Does.Contain("Zeile 2"));
    }

    [Test]
    public void Wenn_ZellinhalteMitSonderzeichen_dann_UnveraendertUebernommen()
    {
        // Arrange — Anführungszeichen, Leerraum und Backslashes bleiben ohne Quoting/Escaping erhalten
        var lines = new List<string>
        {
            "A;B;C",
            "\"zitiert\";  Leerraum  ;pfad\\zu\\datei",
        };

        // Act
        Result<CsvDocument> result = CsvParser.Parse(lines);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        AssertRow(result.Value!.Rows[0], "\"zitiert\"", "  Leerraum  ", "pfad\\zu\\datei");
    }

    [Test]
    public void Wenn_LeereFeldwerte_dann_AlsLeereStringsErhalten()
    {
        // Arrange — aufeinanderfolgende Semikola ergeben leere Felder, die zählen mit
        var lines = new List<string>
        {
            "A;B;C",
            "x;;z",
        };

        // Act
        Result<CsvDocument> result = CsvParser.Parse(lines);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        AssertRow(result.Value!.Rows[0], "x", "", "z");
    }

    private static void AssertHeader(CsvHeader header, params string[] erwarteteSpaltennamen)
    {
        Assert.That(header.ColumnCount, Is.EqualTo(erwarteteSpaltennamen.Length));
        for (int i = 0; i < erwarteteSpaltennamen.Length; i++)
        {
            Assert.That(header[i], Is.EqualTo(erwarteteSpaltennamen[i]),
                $"Spaltenname an Position {i}");
        }
    }

    private static void AssertRow(CsvRow row, params string[] erwarteteFelder)
    {
        Assert.That(row.FieldCount, Is.EqualTo(erwarteteFelder.Length));
        for (int i = 0; i < erwarteteFelder.Length; i++)
        {
            Assert.That(row[i], Is.EqualTo(erwarteteFelder[i]),
                $"Feldwert an Position {i}");
        }
    }
}
