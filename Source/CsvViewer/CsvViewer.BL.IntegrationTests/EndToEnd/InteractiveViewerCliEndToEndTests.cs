using System.Diagnostics;
using System.Text;

namespace CsvViewer.BL.IntegrationTests.EndToEnd;

public class InteractiveViewerCliEndToEndTests
{
    private static readonly string BuildConfiguration =
        Directory.GetParent(TestContext.CurrentContext.TestDirectory)!.Name;

    private static readonly string ProjectPath = Path.GetFullPath(
        Path.Combine(
            TestContext.CurrentContext.TestDirectory,
            "..",
            "..",
            "..",
            "..",
            "CsvViewer",
            "CsvViewer.csproj"));

    [Test]
    public async Task Wenn_ViewerMitDefaultGestartetWird_dann_ErsteZehnZeilenUndMenueErscheinen()
    {
        // Arrange
        string path = WriteTemporaryCsv(
            ["Nummer", .. Enumerable.Range(1, 11).Select(number => $"Zeile-{number:D2}")]);
        string contentsBefore = File.ReadAllText(path, Encoding.UTF8);

        try
        {
            // Act
            CliResult result = await RunInteractiveCliAsync([path], "e");

            // Assert
            Assert.That(result.ExitCode, Is.Zero, result.CombinedOutput);
            Assert.That(result.CombinedOutput, Does.Contain("Zeile-01"));
            Assert.That(result.CombinedOutput, Does.Contain("Zeile-10"));
            Assert.That(result.CombinedOutput, Does.Not.Contain("Zeile-11"));
            Assert.That(
                result.CombinedOutput,
                Does.Contain("F)irst page, P)revious page, N)ext page, L)ast page, E)xit"));
            Assert.That(File.ReadAllText(path, Encoding.UTF8), Is.EqualTo(contentsBefore));
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Test]
    public async Task Wenn_AlleNavigationstastenGedruecktWerden_dann_SeitenfolgeWirdAusgegeben()
    {
        // Arrange
        string path = WriteTemporaryCsv(["Wert", "Seite-Eins", "Seite-Zwei", "Seite-Drei"]);

        try
        {
            // Act
            CliResult result = await RunInteractiveCliAsync([path, "1"], "nnplfe");

            // Assert
            Assert.That(result.ExitCode, Is.Zero, result.CombinedOutput);
            Assert.That(CountOccurrences(result.CombinedOutput, "Seite-Eins"), Is.EqualTo(2));
            Assert.That(CountOccurrences(result.CombinedOutput, "Seite-Zwei"), Is.EqualTo(2));
            Assert.That(CountOccurrences(result.CombinedOutput, "Seite-Drei"), Is.EqualTo(2));
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Test]
    public async Task Wenn_UngueltigeTasteGedruecktWird_dann_SeiteWirdUnveraendertNeuGezeichnet()
    {
        // Arrange
        string path = WriteTemporaryCsv(["Wert", "Einzigartige-Zeile"]);

        try
        {
            // Act
            CliResult result = await RunInteractiveCliAsync([path], "?e");

            // Assert
            Assert.That(result.ExitCode, Is.Zero, result.CombinedOutput);
            Assert.That(
                CountOccurrences(result.CombinedOutput, "Einzigartige-Zeile"),
                Is.EqualTo(2));
            Assert.That(result.CombinedOutput, Does.Not.Contain("[ERROR]"));
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Test]
    public async Task Wenn_EigeneSeitengroesseVerwendetWird_dann_RestseiteIstErreichbar()
    {
        // Arrange
        string path = WriteTemporaryCsv(["Wert", "Eins", "Zwei", "Drei"]);

        try
        {
            // Act
            CliResult result = await RunInteractiveCliAsync([path, "2"], "ne");

            // Assert
            Assert.That(result.ExitCode, Is.Zero, result.CombinedOutput);
            Assert.That(result.CombinedOutput, Does.Contain("Eins"));
            Assert.That(result.CombinedOutput, Does.Contain("Zwei"));
            Assert.That(result.CombinedOutput, Does.Contain("Drei"));
            Assert.That(CountOccurrences(result.CombinedOutput, "Drei"), Is.EqualTo(1));
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Test]
    public async Task Wenn_DateiNurHeaderEnthaelt_dann_HeaderUndTrennlinieWerdenAngezeigt()
    {
        // Arrange
        string path = WriteTemporaryCsv(["Vorname;Nachname"]);

        try
        {
            // Act
            CliResult result = await RunInteractiveCliAsync([path], "E");

            // Assert
            Assert.That(result.ExitCode, Is.Zero, result.CombinedOutput);
            Assert.That(result.CombinedOutput, Does.Contain("Vorname|Nachname|"));
            Assert.That(result.CombinedOutput, Does.Contain("-------+--------+"));
        }
        finally
        {
            File.Delete(path);
        }
    }

    [TestCaseSource(nameof(InvalidArguments))]
    public async Task Wenn_ArgumenteUngueltigSind_dann_ViewerStartetNicht(
        string[] args,
        string expectedMessage)
    {
        // Arrange & Act
        CliResult result = await RunNonInteractiveCliAsync(args);

        // Assert
        Assert.That(result.ExitCode, Is.Not.Zero);
        Assert.That(result.CombinedOutput, Does.Contain(expectedMessage));
        Assert.That(result.CombinedOutput, Does.Not.Contain("F)irst page"));
    }

    [Test]
    public async Task Wenn_DateiFehlt_dann_UrsacheUndFehlercodeWerdenAusgegeben()
    {
        // Arrange
        string path = Path.Combine(
            Path.GetTempPath(),
            $"CsvViewer_R00003_missing_{Guid.NewGuid():N}.csv");
        Assert.That(File.Exists(path), Is.False);

        // Act
        CliResult result = await RunNonInteractiveCliAsync([path]);

        // Assert
        Assert.That(result.ExitCode, Is.Not.Zero);
        Assert.That(result.CombinedOutput, Does.Contain("nicht gefunden"));
        Assert.That(result.CombinedOutput, Does.Not.Contain("F)irst page"));
        Assert.That(File.Exists(path), Is.False);
    }

    [Test]
    public async Task Wenn_DateiLeerIst_dann_ViewerStartetNicht()
    {
        // Arrange
        string path = WriteTemporaryCsv([]);

        try
        {
            // Act
            CliResult result = await RunNonInteractiveCliAsync([path]);

            // Assert
            Assert.That(result.ExitCode, Is.Not.Zero);
            Assert.That(result.CombinedOutput, Does.Contain("CSV-Eingabe ist leer"));
            Assert.That(result.CombinedOutput, Does.Not.Contain("F)irst page"));
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Test]
    public async Task Wenn_CsvZeileKaputtIst_dann_ViewerStartetNicht()
    {
        // Arrange
        string path = WriteTemporaryCsv(["A;B", "nur-ein-feld"]);
        string contentsBefore = File.ReadAllText(path, Encoding.UTF8);

        try
        {
            // Act
            CliResult result = await RunNonInteractiveCliAsync([path]);

            // Assert
            Assert.That(result.ExitCode, Is.Not.Zero);
            Assert.That(result.CombinedOutput, Does.Contain("Zeile 2"));
            Assert.That(result.CombinedOutput, Does.Not.Contain("F)irst page"));
            Assert.That(File.ReadAllText(path, Encoding.UTF8), Is.EqualTo(contentsBefore));
        }
        finally
        {
            File.Delete(path);
        }
    }

    private static IEnumerable<TestCaseData> InvalidArguments()
    {
        yield return new TestCaseData(
            Array.Empty<string>(),
            "Usage: csvviewer <datei.csv> [seitengröße]");
        yield return new TestCaseData(
            new[] { "a.csv", "10", "zuviel" },
            "Usage: csvviewer <datei.csv> [seitengröße]");
        yield return new TestCaseData(
            new[] { "a.csv", "abc" },
            "positive Ganzzahl");
        yield return new TestCaseData(
            new[] { "a.csv", "0" },
            "positive Ganzzahl");
        yield return new TestCaseData(
            new[] { "a.csv", "-1" },
            "positive Ganzzahl");
    }

    private static async Task<CliResult> RunInteractiveCliAsync(
        string[] args,
        string keys)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "/usr/bin/script",
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            WorkingDirectory = Path.GetDirectoryName(ProjectPath)!,
        };
        startInfo.ArgumentList.Add("-qefc");
        startInfo.ArgumentList.Add(BuildShellCommand(args));
        startInfo.ArgumentList.Add("/dev/null");

        return await RunProcessAsync(startInfo, keys);
    }

    private static async Task<CliResult> RunNonInteractiveCliAsync(string[] args)
    {
        var startInfo = CreateDotnetStartInfo(args);
        return await RunProcessAsync(startInfo, null);
    }

    private static ProcessStartInfo CreateDotnetStartInfo(string[] args)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            WorkingDirectory = Path.GetDirectoryName(ProjectPath)!,
        };

        foreach (string argument in BuildDotnetArguments(args))
        {
            startInfo.ArgumentList.Add(argument);
        }

        return startInfo;
    }

    private static string BuildShellCommand(string[] args)
    {
        return string.Join(
            ' ',
            BuildDotnetArguments(args).Prepend("dotnet").Select(ShellQuote));
    }

    private static IEnumerable<string> BuildDotnetArguments(string[] args)
    {
        yield return "run";
        yield return "--no-build";
        yield return "--no-restore";
        yield return "--configuration";
        yield return BuildConfiguration;
        yield return "--project";
        yield return ProjectPath;
        yield return "--";

        foreach (string argument in args)
        {
            yield return argument;
        }
    }

    private static async Task<CliResult> RunProcessAsync(
        ProcessStartInfo startInfo,
        string? standardInput)
    {
        using var process = new Process { StartInfo = startInfo };
        Assert.That(process.Start(), Is.True);

        Task<string> standardOutput = process.StandardOutput.ReadToEndAsync();
        Task<string> standardError = process.StandardError.ReadToEndAsync();

        if (standardInput is not null)
        {
            await process.StandardInput.WriteAsync(standardInput);
            await process.StandardInput.FlushAsync();
        }

        process.StandardInput.Close();

        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(30));
        try
        {
            await process.WaitForExitAsync(timeout.Token);
        }
        catch (OperationCanceledException)
        {
            process.Kill(entireProcessTree: true);
            Assert.Fail("Der CLI-Prozess wurde nicht innerhalb von 30 Sekunden beendet.");
        }

        return new CliResult(
            process.ExitCode,
            await standardOutput,
            await standardError);
    }

    private static string WriteTemporaryCsv(string[] lines)
    {
        string path = Path.Combine(
            Path.GetTempPath(),
            $"CsvViewer_R00003_e2e_{Guid.NewGuid():N}.csv");
        File.WriteAllLines(path, lines, Encoding.UTF8);
        return path;
    }

    private static int CountOccurrences(string text, string value)
    {
        int count = 0;
        int offset = 0;

        while ((offset = text.IndexOf(value, offset, StringComparison.Ordinal)) >= 0)
        {
            count++;
            offset += value.Length;
        }

        return count;
    }

    private static string ShellQuote(string value)
    {
        return $"'{value.Replace("'", "'\"'\"'", StringComparison.Ordinal)}'";
    }

    private sealed record CliResult(int ExitCode, string StandardOutput, string StandardError)
    {
        public string CombinedOutput => StandardOutput + StandardError;
    }
}
