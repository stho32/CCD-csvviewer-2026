using System.Text;
using CsvViewer.BL.Common;
using CsvViewer.BL.IO;

namespace CsvViewer.BL.IntegrationTests.IO;

/// <summary>
/// Integrationstests für <see cref="FileReader"/> gegen echte temporäre Dateien.
/// Prüft das Zusammenspiel mit dem Dateisystem (UTF-8, Reihenfolge, Fehlerfälle).
/// </summary>
public class FileReaderIntegrationTests
{
    private string _tempDirectory = string.Empty;
    private FileReader _fileReader = null!;

    [SetUp]
    public void SetUp()
    {
        _tempDirectory = Path.Combine(
            Path.GetTempPath(), "CsvViewerFileReaderTests_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_tempDirectory);
        _fileReader = new FileReader();
    }

    [TearDown]
    public void TearDown()
    {
        if (Directory.Exists(_tempDirectory))
        {
            Directory.Delete(_tempDirectory, recursive: true);
        }
    }

    [Test]
    public void ReadLines_VorhandeneDatei_LiefertZeilenInReihenfolge()
    {
        // Arrange
        string path = Path.Combine(_tempDirectory, "daten.csv");
        var content = new[] { "Vorname;Nachname", "Anna;Meier", "Ben;Schmidt" };
        File.WriteAllLines(path, content, Encoding.UTF8);

        // Act
        Result<IReadOnlyList<string>> result = _fileReader.ReadLines(path);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.EqualTo(content));
    }

    [Test]
    public void ReadLines_Utf8DateiMitUmlauten_LiestZeichenKorrekt()
    {
        // Arrange
        string path = Path.Combine(_tempDirectory, "umlaute.csv");
        var content = new[] { "Straße;Grüße", "Fuß;Öl" };
        File.WriteAllLines(path, content, Encoding.UTF8);

        // Act
        Result<IReadOnlyList<string>> result = _fileReader.ReadLines(path);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value![0], Is.EqualTo("Straße;Grüße"));
        Assert.That(result.Value[1], Is.EqualTo("Fuß;Öl"));
    }

    [Test]
    public void ReadLines_FehlendeDatei_LiefertFehlerResult()
    {
        // Arrange
        string path = Path.Combine(_tempDirectory, "gibtsnicht.csv");

        // Act
        Result<IReadOnlyList<string>> result = _fileReader.ReadLines(path);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Value, Is.Null);
        Assert.That(result.Message, Does.Contain("nicht gefunden"));
    }

    [Test]
    public void ReadLines_LeereDatei_LiefertErfolgMitLeererZeilenliste()
    {
        // Arrange
        string path = Path.Combine(_tempDirectory, "leer.csv");
        File.WriteAllText(path, string.Empty, Encoding.UTF8);

        // Act
        Result<IReadOnlyList<string>> result = _fileReader.ReadLines(path);

        // Assert — FileReader ist CSV-agnostisch: Leerheit ist Sache des Parsers
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.Empty);
    }

    [Test]
    public void ReadLines_NichtLesbareDatei_LiefertFehlerResult()
    {
        // Arrange — Datei existiert, ist aber ohne Leserechte (erzwingt I/O-Exception)
        if (OperatingSystem.IsWindows())
        {
            Assert.Ignore("Unix-Dateirechte werden unter Windows nicht getestet.");
            return;
        }

        string path = Path.Combine(_tempDirectory, "gesperrt.csv");
        File.WriteAllText(path, "A;B\n1;2", Encoding.UTF8);
        File.SetUnixFileMode(path, UnixFileMode.None);

        try
        {
            // Act
            Result<IReadOnlyList<string>> result = _fileReader.ReadLines(path);

            // Assert — I/O-Exception wird abgefangen und in Fehler-Result übersetzt
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Value, Is.Null);
            Assert.That(result.Message, Does.Contain("konnte nicht gelesen werden"));
        }
        finally
        {
            // Rechte zurücksetzen, damit das TearDown aufräumen kann
            File.SetUnixFileMode(path, UnixFileMode.UserRead | UnixFileMode.UserWrite);
        }
    }

    [Test]
    public void ReadLines_LeererPfad_LiefertFehlerResult()
    {
        // Act
        Result<IReadOnlyList<string>> result = _fileReader.ReadLines("   ");

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Value, Is.Null);
        Assert.That(result.Message, Does.Contain("leer"));
    }
}
