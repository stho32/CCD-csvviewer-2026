# CsvViewer

CLI-Werkzeug zum Anzeigen von CSV-Dateien.

## Tech-Stack

| Technologie | Version | Zweck |
|---|---|---|
| .NET | 10.0 | Runtime und SDK |
| NUnit | 4.3.2 | Unit- und Integrationstests |
| NUnit3TestAdapter | 5.0.0 | Test-Discovery |
| NUnit.Analyzers | 4.7.0 | Statische Analyse der Tests |
| Microsoft.NET.Test.Sdk | 17.14.0 | Test-Host |
| coverlet.collector | 6.0.4 | Code-Coverage |

Die CLI-Argumente werden positional und ohne Library geparst (`CsvViewer.BL/CommandLineArguments/ArgumentsParser.cs`).

## Struktur

```
Source/CsvViewer/
  CsvViewer.sln
  CsvViewer/                       # Entry Point (Composition Root)
    Program.cs
    HostSpecific/                  # Adapter an Konsole und Dateisystem
  CsvViewer.BL/                    # Business Logic
    CommandLineArguments/          # positionales Argument-Parsing
    Csv/                           # Datenmodell und Parser
    IO/                            # Vertraege fuer Datei- und Konsolenzugriff
    Paging/                        # Aufteilen in Seiten
    Navigation/                    # Tastenzuordnung und Seitenindex
    Rendering/                     # Tabellen-Rendering
    Viewer/                        # interaktiver Loop
  CsvViewer.BL.Tests/              # Unit-Tests
  CsvViewer.BL.IntegrationTests/   # Integrations- und E2E-Tests
Anforderungen/                     # Anforderungs-Dokumente (RNNNNN)
beispiel.csv                       # Beispieldaten fuer manuelle Erprobung
testlauf.sh                        # Baut und startet den Viewer mit beispiel.csv
```

## Voraussetzungen

- .NET SDK 10.0+ (`dotnet --version` >= 10.0.x)

## Getting Started

```bash
# Build
dotnet build Source/CsvViewer/CsvViewer.sln

# Tests
dotnet test Source/CsvViewer/CsvViewer.sln

# Ausfuehren — erstes Argument = Datei, zweites (optional) = Seitengroesse (Default 10)
dotnet run --project Source/CsvViewer/CsvViewer/CsvViewer.csproj -- daten.csv
dotnet run --project Source/CsvViewer/CsvViewer/CsvViewer.csproj -- daten.csv 25

# Manueller Testlauf mit den Beispieldaten
./testlauf.sh 5
```

## Bedienung

Im Viewer navigiert ein einzelner Tastendruck (ohne Enter, Gross-/Kleinschreibung egal):
`F)irst page, P)revious page, N)ext page, L)ast page, E)xit`. An den Raendern klemmt die
Navigation, ungueltige Tasten werden ignoriert.

Ist die Eingabe umgeleitet, liest der Viewer die Tastenfolge aus dem Strom — ein Zeichen
je Tastendruck. Damit laesst er sich ohne Terminal steuern, was die E2E-Tests nutzen:

```bash
printf 'nne' | dotnet run --project Source/CsvViewer/CsvViewer/CsvViewer.csproj -- daten.csv 5
```

Endet die Eingabe ohne `E`, meldet der Viewer den Abbruch und beendet mit Exit-Code != 0.

## Anforderungen

Siehe [Anforderungen/](./Anforderungen/). Neue Anforderung via `/erstelle-anforderung`.

## Architektur

Layered CLI-Tool-Architektur — Details unter `.claude/app-architectures/dotnet-cli-tool/`.
