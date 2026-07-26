---
id: R00004
title: "FileReader nach HostSpecific verschieben"
type: Refactoring
status: Erledigt
created: 2026-07-26
---

# R00004: FileReader nach HostSpecific verschieben

## Beschreibung

`FileReader` ist ein Adapter an das Dateisystem der Console-App und damit host-spezifisch — er enthält keine Geschäftslogik, sondern übersetzt `System.IO`-Aufrufe in `Result`-Objekte. Er liegt bislang trotzdem in `CsvViewer.BL/IO/`, weil er in R00001 entstand, bevor es den Ordner `HostSpecific/` im Entry Point gab. Diese Anforderung zieht ihn nach `CsvViewer/HostSpecific/IO/` — dorthin, wo `SystemConsole` und `ConsoleLogger` bereits sitzen. Der Vertrag `IFileReader` bleibt in der BL.

Damit gilt für alle drei Adapter dasselbe Muster: BL definiert das Interface, das Entry-Point-Projekt implementiert es, `Program` verdrahtet. Die BL ist danach frei von direkter `System.IO`-Nutzung.

## User Stories

Given/When/Then-Szenarien: siehe [user-stories/R00004.md](user-stories/R00004.md).

- **US1 — Adapter an einem Ort**: Als Entwickler will ich alle host-spezifischen Adapter unter `HostSpecific/` finden, damit ich beim Wechsel des Hosts genau ein Verzeichnis anfassen muss.
- **US2 — BL ohne Infrastruktur-Zugriff**: Als Entwickler will ich, dass die BL kein `System.IO` und keine `System.Console` direkt aufruft, damit sie ohne Dateisystem und Terminal testbar bleibt.
- **US3 — Verhalten unverändert**: Als Nutzer will ich, dass der Viewer sich nach dem Umbau exakt wie vorher verhält, damit ein reines Refactoring für mich unsichtbar bleibt.

## Akzeptanzkriterien

### Verschiebung
- [x] `FileReader` liegt unter `Source/CsvViewer/CsvViewer/HostSpecific/IO/FileReader.cs`
- [x] Namespace ist `CsvViewer.HostSpecific.IO` (analog `SystemConsole`)
- [x] Klasse ist `internal sealed` (analog `SystemConsole` und `ConsoleLogger`)
- [x] `IFileReader` verbleibt unverändert in `CsvViewer.BL/IO/`
- [x] `CsvViewer.BL/IO/FileReader.cs` existiert nicht mehr
- [x] Das Verhalten des Readers (UTF-8, Fehler-`Result` statt Exception, Meldungstexte) ist unverändert

### Verdrahtung
- [x] `Program` deklariert den Reader über den Vertrag (`IFileReader fileReader = new FileReader();`) statt über den konkreten Typ
- [x] `Program` bleibt reine Verdrahtung ohne Geschäftslogik
- [x] Reihenfolge und Exit-Codes der Fehlerbehandlung in `Program` sind unverändert

### Testbarkeit
- [x] `CsvViewer.csproj` gibt seine internen Typen per `InternalsVisibleTo` an `CsvViewer.BL.IntegrationTests` frei
- [x] Der `ProjectReference` von `CsvViewer.BL.IntegrationTests` auf `CsvViewer.csproj` bindet die Assembly ein (`ReferenceOutputAssembly="false"` entfällt)
- [x] Die sechs bestehenden `FileReaderIntegrationTests` laufen unverändert im Grün — kein Testfall entfällt
- [x] Die CLI-E2E-Tests starten den Viewer weiterhin als eigenen Prozess und laufen grün

### Qualität
- [x] Solution baut ohne Warnungen (`TreatWarningsAsErrors` bleibt aktiv)
- [x] Alle Tests aller Ebenen laufen grün
- [x] Kein `using System.IO`-Bedarf und kein `File.`-Aufruf mehr in `CsvViewer.BL`

## Status

- [x] Erledigt

## Technische Details

### Verschobene Dateien

| Von | Nach | Änderung |
|-----|------|----------|
| `Source/CsvViewer/CsvViewer.BL/IO/FileReader.cs` | `Source/CsvViewer/CsvViewer/HostSpecific/IO/FileReader.cs` | Namespace `CsvViewer.BL.IO` → `CsvViewer.HostSpecific.IO`, Sichtbarkeit `public` → `internal`, `using CsvViewer.BL.IO;` für `IFileReader` ergänzt |

### Zu ändernde Dateien

| Datei | Änderung |
|-------|----------|
| `Source/CsvViewer/CsvViewer/Program.cs` | `using CsvViewer.HostSpecific.IO;` ist bereits vorhanden (`SystemConsole`); `var fileReader = new FileReader();` → `IFileReader fileReader = new FileReader();` |
| `Source/CsvViewer/CsvViewer/CsvViewer.csproj` | `<InternalsVisibleTo Include="CsvViewer.BL.IntegrationTests" />` ergänzen |
| `Source/CsvViewer/CsvViewer.BL.IntegrationTests/CsvViewer.BL.IntegrationTests.csproj` | `ReferenceOutputAssembly="false"` am `ProjectReference` auf `CsvViewer.csproj` entfernen |
| `Source/CsvViewer/CsvViewer.BL.IntegrationTests/IO/FileReaderIntegrationTests.cs` | `using CsvViewer.BL.IO;` → `using CsvViewer.HostSpecific.IO;` (`Result` kommt weiter aus `CsvViewer.BL.Common`) |

### IODA/SRP-Zuordnung nach dem Umbau

| Schicht | Bausteine | Projekt |
|---------|-----------|---------|
| Data | `CsvDocument`, `ViewerArguments`, `NavigationCommand`, `PagedDocument` | BL |
| Operation (pure) | `CsvParser`, `TableRenderer`, `ArgumentsParser`, `Paginator`, `NavigationCommandMapper`, `PageNavigator` | BL |
| I/O-Vertrag | `IFileReader`, `IConsole`, `ILogger` | BL |
| I/O-Rand (host-spezifisch) | `FileReader`, `SystemConsole`, `ConsoleLogger` | Entry Point |
| Integration | `InteractiveViewer` (Loop, testbar), `Program` (Composition Root) | BL / Entry Point |

### Tests

| Testdatei | Prüft |
|-----------|-------|
| `Source/CsvViewer/CsvViewer.BL.IntegrationTests/IO/FileReaderIntegrationTests.cs` | Unverändert: vorhandene Datei, UTF-8-Umlaute, fehlende Datei, leere Datei, nicht lesbare Datei (Unix), leerer Pfad — jetzt gegen den Typ im Entry-Point-Projekt |
| `Source/CsvViewer/CsvViewer.BL.IntegrationTests/EndToEnd/InteractiveViewerCliEndToEndTests.cs` | Unverändert: der Gesamtfluss über den echten Prozess belegt, dass die Verdrahtung nach dem Umzug trägt |

## Abhängigkeiten

- Abhängig von: R00001 (`FileReader`), R00003 (`HostSpecific/`, Composition Root)
- Blockiert: —

## Out-of-Scope

- Verhaltensänderungen am Lesen (andere Encodings, Streaming, große Dateien)
- Einführung eines DI-Containers — `Program` bleibt manuelle Verdrahtung
- Umbau von `SystemConsole` oder `ConsoleLogger`; dass beide durch `InternalsVisibleTo` erstmals testbar werden, ist Nebeneffekt, keine Aufgabe dieser Anforderung
- Ein eigenes Infrastruktur-Projekt zwischen BL und Entry Point

## Notizen

- Auslöser: `IFileReader` wurde von keinem Konsumenten verwendet — `Program` rief den konkreten `FileReader` direkt auf. Ein Vertrag ohne Gegenüber ist das Symptom dafür, dass Interface und Implementierung in derselben Assembly lagen.
- Die Coverage-Zahl des BL-Projekts verschiebt sich leicht, weil ein Adapter mit I/O-Zweigen die Assembly verlässt. Das Coverage-Ziel bezieht sich weiterhin auf die Solution.
- Risiko beim `ProjectReference`: `CsvViewer.csproj` setzt `SelfContained` und `PublishSingleFile` und baut nach `bin/<Config>/net10.0/win-x64/`. Wird die Assembly eingebunden, kann der RuntimeIdentifier einen Konflikt (NETSDK-Fehler) im Testprojekt auslösen. Fällt das an, wird der RID im Testprojekt nachgezogen statt die Referenz zurückzunehmen.

### Verworfene Alternativen

- **Status quo lassen, `IFileReader` löschen** — verworfen: beseitigt das Symptom, lässt aber einen Infrastruktur-Adapter in der Geschäftslogik.
- **`FileReader` im Entry Point `public` lassen** — verworfen zugunsten `internal`, konsistent zu `SystemConsole` und `ConsoleLogger`.
- **`FileReaderIntegrationTests` ersatzlos streichen und nur über CLI-E2E abdecken** — verworfen: Sonderfälle wie leerer Pfad, UTF-8-Umlaute und fehlende Leserechte verlieren ihre gezielte Prüfung.
- **Eigenes Projekt `CsvViewer.Infrastructure`** — verworfen: drei Adapterklassen rechtfertigen keine vierte Assembly.
