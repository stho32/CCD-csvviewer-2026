using System.Text;
using CsvViewer.BL.Common;
using CsvViewer.BL.IO;

namespace CsvViewer.BL.IntegrationTests.IO;

public class FileReaderIntegrationTests
{
    [Test]
    public void Wenn_VorhandeneDatei_dann_ZeilenInReihenfolge()
    {
        // Arrange
        string tempDirectory = CreateTempDirectory();
        try
        {
            string path = Path.Combine(tempDirectory, "daten.csv");
            var content = new[] { "Vorname;Nachname", "Anna;Meier", "Ben;Schmidt" };
            File.WriteAllLines(path, content, Encoding.UTF8);
            var fileReader = new FileReader();

            // Act
            Result<IReadOnlyList<string>> result = fileReader.ReadLines(path);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.EqualTo(content));
        }
        finally
        {
            Directory.Delete(tempDirectory, recursive: true);
        }
    }

    [Test]
    public void Wenn_Utf8DateiMitUmlauten_dann_ZeichenKorrektGelesen()
    {
        // Arrange
        string tempDirectory = CreateTempDirectory();
        try
        {
            string path = Path.Combine(tempDirectory, "umlaute.csv");
            var content = new[] { "Straße;Grüße", "Fuß;Öl" };
            File.WriteAllLines(path, content, Encoding.UTF8);
            var fileReader = new FileReader();

            // Act
            Result<IReadOnlyList<string>> result = fileReader.ReadLines(path);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value![0], Is.EqualTo("Straße;Grüße"));
            Assert.That(result.Value[1], Is.EqualTo("Fuß;Öl"));
        }
        finally
        {
            Directory.Delete(tempDirectory, recursive: true);
        }
    }

    [Test]
    public void Wenn_FehlendeDatei_dann_FehlerResult()
    {
        // Arrange
        string tempDirectory = CreateTempDirectory();
        try
        {
            string path = Path.Combine(tempDirectory, "gibtsnicht.csv");
            var fileReader = new FileReader();

            // Act
            Result<IReadOnlyList<string>> result = fileReader.ReadLines(path);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Value, Is.Null);
            Assert.That(result.Message, Does.Contain("nicht gefunden"));
        }
        finally
        {
            Directory.Delete(tempDirectory, recursive: true);
        }
    }

    [Test]
    public void Wenn_LeereDatei_dann_ErfolgMitLeererZeilenliste()
    {
        // Arrange
        string tempDirectory = CreateTempDirectory();
        try
        {
            string path = Path.Combine(tempDirectory, "leer.csv");
            File.WriteAllText(path, string.Empty, Encoding.UTF8);
            var fileReader = new FileReader();

            // Act
            Result<IReadOnlyList<string>> result = fileReader.ReadLines(path);

            // Assert — FileReader ist CSV-agnostisch: Leerheit ist Sache des Parsers
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.Empty);
        }
        finally
        {
            Directory.Delete(tempDirectory, recursive: true);
        }
    }

    [Test]
    public void Wenn_NichtLesbareDatei_dann_FehlerResult()
    {
        if (OperatingSystem.IsWindows())
        {
            Assert.Ignore("Unix-Dateirechte werden unter Windows nicht getestet.");
            return;
        }

        // Arrange — Datei existiert, ist aber ohne Leserechte (erzwingt I/O-Exception)
        string tempDirectory = CreateTempDirectory();
        string path = Path.Combine(tempDirectory, "gesperrt.csv");
        try
        {
            File.WriteAllText(path, "A;B\n1;2", Encoding.UTF8);
            File.SetUnixFileMode(path, UnixFileMode.None);
            var fileReader = new FileReader();

            // Act
            Result<IReadOnlyList<string>> result = fileReader.ReadLines(path);

            // Assert — I/O-Exception wird abgefangen und in Fehler-Result übersetzt
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Value, Is.Null);
            Assert.That(result.Message, Does.Contain("konnte nicht gelesen werden"));
        }
        finally
        {
            // Rechte zurücksetzen, damit das Aufräumen funktioniert
            File.SetUnixFileMode(path, UnixFileMode.UserRead | UnixFileMode.UserWrite);
            Directory.Delete(tempDirectory, recursive: true);
        }
    }

    [Test]
    public void Wenn_LeererPfad_dann_FehlerResult()
    {
        // Arrange
        var fileReader = new FileReader();

        // Act
        Result<IReadOnlyList<string>> result = fileReader.ReadLines("   ");

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Value, Is.Null);
        Assert.That(result.Message, Does.Contain("leer"));
    }

    private static string CreateTempDirectory()
    {
        string path = Path.Combine(
            Path.GetTempPath(), "CsvViewerFileReaderTests_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(path);
        return path;
    }
}
