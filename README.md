# CsvViewer

CLI-Werkzeug zum Anzeigen von CSV-Dateien.

## Tech-Stack

| Technologie | Version | Zweck |
|---|---|---|
| .NET | 10.0 | Runtime und SDK |
| CommandLineParser | 2.9.1 | CLI-Argument-Parsing |
| NUnit | 4.3.2 | Unit- und Integrationstests |
| coverlet | 6.0.4 | Code-Coverage |

## Struktur

```
Source/CsvViewer/
  CsvViewer.sln
  CsvViewer/                       # Entry Point (Console App)
  CsvViewer.BL/                    # Business Logic
  CsvViewer.BL.Tests/              # Unit-Tests
  CsvViewer.BL.IntegrationTests/   # Integrationstests
Anforderungen/                     # Anforderungs-Dokumente (RNNNNN)
```

## Voraussetzungen

- .NET SDK 10.0+ (`dotnet --version` >= 10.0.x)

## Getting Started

```bash
# Build
dotnet build Source/CsvViewer/CsvViewer.sln

# Tests
dotnet test Source/CsvViewer/CsvViewer.sln

# Ausfuehren
dotnet run --project Source/CsvViewer/CsvViewer/CsvViewer.csproj -- --file daten.csv
```

## Anforderungen

Siehe [Anforderungen/](./Anforderungen/). Neue Anforderung via `/erstelle-anforderung`.

## Architektur

Layered CLI-Tool-Architektur — Details unter `.claude/app-architectures/dotnet-cli-tool/`.
