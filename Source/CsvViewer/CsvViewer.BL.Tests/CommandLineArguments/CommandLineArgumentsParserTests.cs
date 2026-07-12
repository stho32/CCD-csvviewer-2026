using CsvViewer.BL.CommandLineArguments;
using CsvViewer.BL.Common;

namespace CsvViewer.BL.Tests.CommandLineArguments;

public class CommandLineArgumentsParserTests
{
    [Test]
    public void Wenn_DateioptionGueltig_dann_ViewerArgumentsMitDateipfad()
    {
        // Arrange
        string[] args = ["--file", "daten.csv"];

        // Act
        Result<ViewerArguments> result = CommandLineArgumentsParser.Parse(args);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.EqualTo(new ViewerArguments("daten.csv")));
    }

    [Test]
    public void Wenn_DateioptionFehlt_dann_FehlerOhneViewerArguments()
    {
        // Arrange
        string[] args = [];

        // Act
        Result<ViewerArguments> result = CommandLineArgumentsParser.Parse(args);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Value, Is.Null);
        Assert.That(result.Message, Is.Not.Empty);
    }
}
