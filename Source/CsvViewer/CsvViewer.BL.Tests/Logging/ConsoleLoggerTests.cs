using System.Text;
using CsvViewer.BL.Common;
using CsvViewer.BL.Logging;

namespace CsvViewer.BL.Tests.Logging;

[NonParallelizable]
public class ConsoleLoggerTests
{
    [Test]
    public void Wenn_InfoGeschriebenWird_dann_MeldungErscheintAufStandardausgabe()
    {
        // Arrange
        TextWriter originalOut = Console.Out;
        var output = new StringWriter();
        Assert.That(output.ToString(), Is.Empty);

        try
        {
            Console.SetOut(output);

            // Act
            Result result = new ConsoleLogger().Info("gestartet");

            // Assert
            Assert.That(result.IsSuccess, Is.True, result.Message);
            Assert.That(
                output.ToString(),
                Is.EqualTo($"[INFO] gestartet{Environment.NewLine}"));
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    [Test]
    public void Wenn_FehlerGeschriebenWird_dann_MeldungErscheintAufFehlerausgabe()
    {
        // Arrange
        TextWriter originalError = Console.Error;
        var output = new StringWriter();
        Assert.That(output.ToString(), Is.Empty);

        try
        {
            Console.SetError(output);

            // Act
            Result result = new ConsoleLogger().Error("kaputt");

            // Assert
            Assert.That(result.IsSuccess, Is.True, result.Message);
            Assert.That(
                output.ToString(),
                Is.EqualTo($"[ERROR] kaputt{Environment.NewLine}"));
        }
        finally
        {
            Console.SetError(originalError);
        }
    }

    [Test]
    public void Wenn_AusgabeIOExceptionWirft_dann_FehlerResultStattException()
    {
        // Arrange
        TextWriter originalError = Console.Error;
        var output = new ThrowingTextWriter();
        Assert.That(output.WriteAttempts, Is.Zero);

        try
        {
            Console.SetError(output);

            // Act
            Result result = new ConsoleLogger().Error("kaputt");

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Does.Contain("fehlgeschlagen"));
            Assert.That(output.WriteAttempts, Is.EqualTo(1));
        }
        finally
        {
            Console.SetError(originalError);
        }
    }

    private sealed class ThrowingTextWriter : TextWriter
    {
        public override Encoding Encoding => Encoding.UTF8;
        public int WriteAttempts { get; private set; }

        public override void WriteLine(string? value)
        {
            WriteAttempts++;
            throw new IOException("Erwarteter Testfehler.");
        }
    }
}
