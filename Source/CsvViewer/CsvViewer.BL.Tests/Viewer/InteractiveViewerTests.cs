using CsvViewer.BL.Common;
using CsvViewer.BL.Csv;
using CsvViewer.BL.Rendering;
using CsvViewer.BL.Tests.Mocks;
using CsvViewer.BL.Viewer;

namespace CsvViewer.BL.Tests.Viewer;

public class InteractiveViewerTests
{
    [Test]
    public void Wenn_NextNextPreviousGedruecktWird_dann_ErwarteteSeitenWerdenNeuGezeichnet()
    {
        // Arrange
        IReadOnlyList<CsvDocument> pages = CreatePages("eins", "zwei", "drei");
        var console = new TestConsole('N', 'N', 'P', 'E');
        var viewer = new InteractiveViewer(console, new TableRenderer());

        // Act
        Result result = viewer.Run(pages);

        // Assert
        Assert.That(result.IsSuccess, Is.True, result.Message);
        Assert.That(console.ClearCount, Is.EqualTo(4));
        Assert.That(console.ReadCount, Is.EqualTo(4));
        Assert.That(console.WrittenTexts, Has.Count.EqualTo(4));
        AssertPageSequence(console.WrittenTexts, "eins", "zwei", "drei", "zwei");
        Assert.That(
            console.WrittenTexts.All(text => text.Contains(InteractiveViewer.Menu)),
            Is.True);
    }

    [Test]
    public void Wenn_UngueltigeTasteGedruecktWird_dann_DieselbeSeiteWirdNeuGezeichnet()
    {
        // Arrange
        IReadOnlyList<CsvDocument> pages = CreatePages("eins", "zwei");
        var console = new TestConsole('?', 'E');
        var viewer = new InteractiveViewer(console, new TableRenderer());

        // Act
        Result result = viewer.Run(pages);

        // Assert
        Assert.That(result.IsSuccess, Is.True, result.Message);
        Assert.That(console.WrittenTexts, Has.Count.EqualTo(2));
        Assert.That(console.WrittenTexts[0], Is.EqualTo(console.WrittenTexts[1]));
        Assert.That(console.WrittenTexts.All(text => !text.Contains("Fehler")), Is.True);
    }

    [Test]
    public void Wenn_KleinGeschriebenesLastFirstExitGedruecktWird_dann_BefehleWerdenAusgefuehrt()
    {
        // Arrange
        IReadOnlyList<CsvDocument> pages = CreatePages("eins", "zwei", "drei");
        var console = new TestConsole('l', 'f', 'e');
        var viewer = new InteractiveViewer(console, new TableRenderer());

        // Act
        Result result = viewer.Run(pages);

        // Assert
        Assert.That(result.IsSuccess, Is.True, result.Message);
        AssertPageSequence(console.WrittenTexts, "eins", "drei", "eins");
        Assert.That(console.ClearCount, Is.EqualTo(3));
    }

    [Test]
    public void Wenn_ExitSofortGedruecktWird_dann_NurErsteSeiteWirdGezeichnet()
    {
        // Arrange
        IReadOnlyList<CsvDocument> pages = CreatePages("eins", "zwei");
        var console = new TestConsole('E');
        var viewer = new InteractiveViewer(console, new TableRenderer());

        // Act
        Result result = viewer.Run(pages);

        // Assert
        Assert.That(result.IsSuccess, Is.True, result.Message);
        Assert.That(console.ClearCount, Is.EqualTo(1));
        Assert.That(console.ReadCount, Is.EqualTo(1));
        Assert.That(console.WrittenTexts, Has.Count.EqualTo(1));
        Assert.That(console.WrittenTexts[0], Does.Contain("eins"));
        Assert.That(console.WrittenTexts[0], Does.Not.Contain("zwei"));
    }

    [Test]
    public void Wenn_KeineSeitenUebergebenWerden_dann_KeineKonsolenaktionFindetStatt()
    {
        // Arrange
        var console = new TestConsole('E');
        var viewer = new InteractiveViewer(console, new TableRenderer());

        // Act
        Result result = viewer.Run(Array.Empty<CsvDocument>());

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(console.ClearCount, Is.Zero);
        Assert.That(console.ReadCount, Is.Zero);
        Assert.That(console.WrittenTexts, Is.Empty);
    }

    [Test]
    public void Wenn_SeitenNullSind_dann_KeineKonsolenaktionFindetStatt()
    {
        // Arrange
        var console = new TestConsole('E');
        var viewer = new InteractiveViewer(console, new TableRenderer());

        // Act
        Result result = viewer.Run(null);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(console.ClearCount, Is.Zero);
        Assert.That(console.ReadCount, Is.Zero);
        Assert.That(console.WrittenTexts, Is.Empty);
    }

    [Test]
    public void Wenn_ViewerAbhaengigkeitFehlt_dann_ViewerStartetNicht()
    {
        // Arrange
        var console = new TestConsole('E');
        var viewer = new InteractiveViewer(console, null);

        // Act
        Result result = viewer.Run(CreatePages("eins"));

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Message, Does.Contain("Abhängigkeiten"));
        Assert.That(console.ClearCount, Is.Zero);
        Assert.That(console.ReadCount, Is.Zero);
        Assert.That(console.WrittenTexts, Is.Empty);
    }

    [Test]
    public void Wenn_KeineTasteVerfuegbarIst_dann_NachErsterAusgabeWirdAbgebrochen()
    {
        // Arrange
        var console = new TestConsole();
        var viewer = new InteractiveViewer(console, new TableRenderer());

        // Act
        Result result = viewer.Run(CreatePages("eins"));

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Message, Does.Contain("Keine Testtaste"));
        Assert.That(console.ClearCount, Is.EqualTo(1));
        Assert.That(console.WrittenTexts, Has.Count.EqualTo(1));
        Assert.That(console.ReadCount, Is.EqualTo(1));
    }

    [Test]
    public void Wenn_LeerenFehlschlaegt_dann_WederAusgabeNochEingabeFindetStatt()
    {
        // Arrange
        var console = new TestConsole('E') { FailClear = true };
        var viewer = new InteractiveViewer(console, new TableRenderer());

        // Act
        Result result = viewer.Run(CreatePages("eins"));

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Message, Does.Contain("Leeren"));
        Assert.That(console.ClearCount, Is.EqualTo(1));
        Assert.That(console.WrittenTexts, Is.Empty);
        Assert.That(console.ReadCount, Is.Zero);
    }

    [Test]
    public void Wenn_RendernFehlschlaegt_dann_WederAusgabeNochEingabeFindetStatt()
    {
        // Arrange
        var console = new TestConsole('E');
        var renderer = new FailingTableRenderer();
        var viewer = new InteractiveViewer(console, renderer);

        // Act
        Result result = viewer.Run(CreatePages("eins"));

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Message, Does.Contain("Rendern"));
        Assert.That(console.ClearCount, Is.EqualTo(1));
        Assert.That(renderer.RenderCount, Is.EqualTo(1));
        Assert.That(console.WrittenTexts, Is.Empty);
        Assert.That(console.ReadCount, Is.Zero);
    }

    [Test]
    public void Wenn_SchreibenFehlschlaegt_dann_KeineEingabeFindetStatt()
    {
        // Arrange
        var console = new TestConsole('E') { FailWrite = true };
        var viewer = new InteractiveViewer(console, new TableRenderer());

        // Act
        Result result = viewer.Run(CreatePages("eins"));

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Message, Does.Contain("Schreiben"));
        Assert.That(console.ClearCount, Is.EqualTo(1));
        Assert.That(console.WrittenTexts, Is.Empty);
        Assert.That(console.ReadCount, Is.Zero);
    }

    private static IReadOnlyList<CsvDocument> CreatePages(params string[] values)
    {
        return values
            .Select(
                value => new CsvDocument(
                    new CsvHeader(["Seite"]),
                    new CsvRowCollection([new CsvRow([value])])))
            .ToArray();
    }

    private static void AssertPageSequence(
        IReadOnlyList<string> writtenTexts,
        params string[] expectedValues)
    {
        Assert.That(writtenTexts, Has.Count.EqualTo(expectedValues.Length));

        for (int index = 0; index < expectedValues.Length; index++)
        {
            Assert.That(writtenTexts[index], Does.Contain(expectedValues[index]));

            foreach (string otherValue in expectedValues.Distinct())
            {
                if (otherValue != expectedValues[index])
                {
                    Assert.That(writtenTexts[index], Does.Not.Contain(otherValue));
                }
            }
        }
    }
}
