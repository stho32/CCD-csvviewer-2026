using CsvViewer.BL.Csv;
using CsvViewer.BL.Common;

namespace CsvViewer.BL.Tests.Csv;

/// <summary>
/// Unit-Tests für <see cref="CsvParser"/> — rein in-memory (Zeilen → Dokument).
/// Deckt alle Format- und Validierungsfälle aus R00001 ab.
/// </summary>
public class CsvParserTests
{
    [Test]
    public void Parse_HeaderUndDatenzeilen_LiefertDokumentMitZeilenInReihenfolge()
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
    public void Parse_GueltigesDokument_FeldwerteSindPositionsbasiertZugreifbar()
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
    public void Parse_LeereEingabe_LiefertFehlerResult()
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
    public void Parse_NullEingabe_LiefertFehlerResult()
    {
        // Act
        Result<CsvDocument> result = CsvParser.Parse(null!);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Value, Is.Null);
    }

    [Test]
    public void Parse_NurKopfzeile_LiefertGueltigesDokumentMitNullDatensaetzen()
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
    public void Parse_DatenzeileMitZuWenigFeldern_LiefertFehlerResult()
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
    public void Parse_DatenzeileMitZuVielenFeldern_LiefertFehlerResult()
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
    public void Parse_Zellinhalte_WerdenUnveraendertUebernommen()
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
    public void Parse_LeereFeldwerte_BleibenAlsLeereStringsErhalten()
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
