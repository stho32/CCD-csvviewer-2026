# CLAUDE.md

## Projekt

CsvViewer — CLI-Werkzeug zum Anzeigen von CSV-Dateien.

## Architektur

Layered CLI-Tool-Architektur: Entry Point -> BL (Business Logic).
Referenz: `.claude/app-architectures/dotnet-cli-tool/ARCHITECTURE.md`.

- **CsvViewer** — Einstiegspunkt: Composition Root (`Program.cs`) plus die host-spezifischen
  Adapter unter `HostSpecific/` (`SystemConsole`, `ConsoleLogger`)
- **CsvViewer.BL** — Geschaeftslogik: Argument-Parsing, CSV-Parsing, Paging, Navigation,
  Rendering, interaktiver Loop, Validierung, IO- und Logging-Interfaces
- **CsvViewer.BL.Tests** — Unit-Tests (NUnit), hand-written Mocks
- **CsvViewer.BL.IntegrationTests** — Integrationstests mit echten Dateien

## Befehle

- Build: `dotnet build Source/CsvViewer/CsvViewer.sln`
- Test: `dotnet test Source/CsvViewer/CsvViewer.sln`
- Test + Coverage: `dotnet test Source/CsvViewer/CsvViewer.sln --collect:"XPlat Code Coverage"`
- Run: `dotnet run --project Source/CsvViewer/CsvViewer/CsvViewer.csproj -- [args]`

## Regeln

- Entry Point enthaelt KEINE Geschaeftslogik
- Alle oeffentlichen Methoden in BL geben Result-Objekte zurueck (keine Exceptions ueber Layer-Grenzen)
- `Main()` gibt int zurueck: 0 = Erfolg, non-zero = Fehler
- `<Nullable>enable</Nullable>` und `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>` in allen Projekten
- Hand-written Mocks statt Mocking-Frameworks
- File-scoped Namespaces (`namespace X;`)
- Coverage-Ziel: >80% Line Coverage im BL-Projekt, gemessen ueber die Solution

## Anforderungen

Verzeichnis `Anforderungen/`, Format `RNNNNN-slug.md`. Neue Anforderung via `/erstelle-anforderung`.

## Domaenenwissen

Verzeichnis `Dokumentation/Domaenenwissen/`, Format `DNNNNN-slug.md`, Sprache `de`.
Pflege via `/domaenenwissen <subbefehl>`.
