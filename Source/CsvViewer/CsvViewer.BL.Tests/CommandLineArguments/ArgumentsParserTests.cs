using CsvViewer.BL.CommandLineInterpretation;
using CsvViewer.BL.Common;
using CsvViewer.BL.DocumentAcquisition;

namespace CsvViewer.BL.Tests.CommandLineArguments;

public class ArgumentsParserTests
{
    [Test]
    public void Wenn_NurDateipfadAngegeben_dann_DefaultSeitengroesseWirdErzeugt()
    {
        // Arrange
        string[] args = ["daten.csv"];

        // Act
        Result<ViewerArguments> result = ArgumentsParser.Parse(args);

        // Assert
        Assert.That(result.IsSuccess, Is.True, result.Message);
        Assert.That(
            result.Value,
            Is.EqualTo(new ViewerArguments("daten.csv", ArgumentsParser.DefaultPageSize)));
        Assert.That(args, Is.EqualTo(new[] { "daten.csv" }));
    }

    [Test]
    public void Wenn_PositiveSeitengroesseAngegeben_dann_DieseWirdErzeugt()
    {
        // Arrange
        string[] args = ["daten.csv", "40"];

        // Act
        Result<ViewerArguments> result = ArgumentsParser.Parse(args);

        // Assert
        Assert.That(result.IsSuccess, Is.True, result.Message);
        Assert.That(result.Value, Is.EqualTo(new ViewerArguments("daten.csv", 40)));
        Assert.That(args, Is.EqualTo(new[] { "daten.csv", "40" }));
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase(" ")]
    [TestCase("abc")]
    [TestCase("0")]
    [TestCase("-1")]
    public void Wenn_SeitengroesseUngueltig_dann_KeineArgumenteWerdenErzeugt(string? pageSize)
    {
        // Arrange
        string[] args = ["daten.csv", pageSize!];

        // Act
        Result<ViewerArguments> result = ArgumentsParser.Parse(args);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Value, Is.Null);
        Assert.That(result.Message, Does.Contain("positive Ganzzahl"));
        Assert.That(args[0], Is.EqualTo("daten.csv"));
    }

    [Test]
    public void Wenn_ArgumenteNullSind_dann_UsageOhneViewerArguments()
    {
        // Arrange
        string[]? args = null;

        // Act
        Result<ViewerArguments> result = ArgumentsParser.Parse(args);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Value, Is.Null);
        Assert.That(result.Message, Is.EqualTo(ArgumentsParser.Usage));
    }

    [TestCaseSource(nameof(InvalidArgumentCounts))]
    public void Wenn_ArgumentanzahlUngueltig_dann_UsageOhneViewerArguments(string[] args)
    {
        // Arrange
        string[] originalArgs = [.. args];

        // Act
        Result<ViewerArguments> result = ArgumentsParser.Parse(args);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Value, Is.Null);
        Assert.That(result.Message, Is.EqualTo(ArgumentsParser.Usage));
        Assert.That(args, Is.EqualTo(originalArgs));
    }

    private static IEnumerable<string[]> InvalidArgumentCounts()
    {
        yield return [];
        yield return [""];
        yield return [" "];
        yield return ["a.csv", "10", "zuviel"];
    }
}
