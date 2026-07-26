---
id: R00003
title: "Interaktiver Viewer (Paging, Navigation, Composition Root)"
type: Feature
status: Erledigt
created: 2026-07-01
---

# R00003: Interaktiver Viewer (Paging, Navigation, Composition Root)

## Beschreibung

Die Anwendung ist der interaktive Einstiegspunkt des CSV-Viewers. Sie liest die positionalen Kommandozeilenargumente (Dateipfad, optionale Seitengröße; Default 10 Datensätze), verdrahtet als Composition Root das Einlesen (`FileReader` + `CsvParser`), das seitenweise Aufteilen der Datensätze und das Rendern (`TableRenderer`) und steuert einen Navigations-Loop. Nach jedem Schritt wird die Konsole gelöscht und die aktuelle Seite samt Menü (`F)irst page, P)revious page, N)ext page, L)ast page, E)xit`) neu gezeichnet. Ein einzelner Tastendruck (ohne Enter, groß-/kleinschreibungs-unabhängig) wählt die Aktion; die Navigation klemmt an den Rändern, ungültige Tasten werden ignoriert. Fehler (Datei, leere Datei, kaputte Zeile, ungültige/zu viele Argumente) führen zu einer klaren Meldung und einem Exit-Code ≠ 0.

Dies ist die **dritte von drei Anforderungen** (vertikale Slices) für einen CSV-Viewer-CLI nach CCD-Kursaufgabe „CSV Viewer I". Übergeordnete Zusatzvorgabe für alle drei: saubere Umsetzung nach **SRP, DRY und IODA-Architektur** (Integration/Operation/Data-Trennung nach Westphal). R00003 stellt die IODA-Integrationsschicht: Sie verdrahtet die reinen Operationen aus R00001/R00002 mit den I/O-Randbausteinen.

## User Stories

Given/When/Then-Szenarien: siehe [user-stories/R00003.md](user-stories/R00003.md).

- **US1 — Viewer starten und erste Seite sehen**: Als Nutzer will ich eine CSV-Datei per `csvviewer datei.csv` öffnen, damit ich sofort die erste Seite als Tabelle sehe.
- **US2 — Durch Seiten navigieren**: Als Nutzer will ich per Tastendruck zwischen Seiten wechseln, damit ich alle Datensätze durchsehen kann.
- **US3 — Seitengröße vorgeben**: Als Nutzer will ich die Datensätze-pro-Seite optional als zweites Argument setzen, damit ich die Ansicht an mein Terminal anpassen kann.
- **US4 — Klare Rückmeldung bei Fehlbedienung**: Als Nutzer will ich bei falschem Aufruf oder fehlerhafter Datei eine verständliche Meldung statt eines Absturzes.

## Akzeptanzkriterien

### CLI-Argumente
- [x] Erstes Argument = Dateipfad (erforderlich)
- [x] Zweites Argument = Seitengröße (optional, positive Ganzzahl)
- [x] Ohne zweites Argument gilt Default = 10 Datensätze pro Seite
- [x] Fehlendes Dateiargument oder mehr als 2 Argumente → Abbruch mit knapper Usage-Meldung, Exit ≠ 0
- [x] Nicht-numerische oder ≤ 0 Seitengröße → Abbruch mit Meldung, Exit ≠ 0

### Paging
- [x] Datensätze werden in Seiten à Seitengröße aufgeteilt (letzte Seite ggf. kürzer)
- [x] Seitenzahl = ⌈Datensätze / Seitengröße⌉, mindestens 1 Seite
- [x] Bei 0 Datensätzen (nur Kopfzeile) wird genau eine Seite mit Kopf + Trennlinie angezeigt
- [x] Jede Seite berechnet ihre Spaltenbreiten unabhängig (über den Renderer aus R00002)

### Navigation
- [x] Menüzeile `F)irst page, P)revious page, N)ext page, L)ast page, E)xit` wird unter der Tabelle angezeigt
- [x] Auswahl per einzelnem Tastendruck ohne Enter, groß-/kleinschreibungs-unabhängig
- [x] F → erste, L → letzte, N → nächste, P → vorherige Seite
- [x] N auf letzter Seite / P auf erster Seite → bleibt stehen (kein Umlauf)
- [x] Ungültige Taste → Eingabe wird ignoriert, Seite unverändert neu gezeichnet
- [x] E → Programm endet mit Exit-Code 0
- [x] Vor jedem Zeichnen wird die Konsole gelöscht und die aktuelle Seite neu aufgebaut

### Architektur & Qualität
- [x] `Program` ist Composition Root: verdrahtet `FileReader`, `CsvParser`, Paging und `TableRenderer`
- [x] Paging- und Navigations-Logik sind als testbare Operation von der Konsole entkoppelt (`IConsole`-Abstraktion für Ein-/Ausgabe/Clear)
- [x] Entry-Point-Projekt enthält keine Geschäftslogik über das Verdrahten hinaus
- [x] Unit-Tests decken Paging-Grenzfälle und Navigation (Klemmen, Ignorieren, Exit) mit einer Test-Konsole ab
- [x] Integrationstest deckt den Gesamtfluss Datei → Anzeige der ersten Seite ab

## Status

- [x] Erledigt

## Technische Details

### Neue Dateien in `CsvViewer.BL/`

| Datei | Rolle (IODA/SRP) | Beschreibung |
|-------|------------------|--------------|
| `CommandLineArguments/ViewerArguments.cs` | **Data** | `record`: `FilePath`, `PageSize`. |
| `CommandLineArguments/ArgumentsParser.cs` | **Operation (pure)** | `string[] → Result<ViewerArguments>`: positional, validiert Argumentanzahl + Seitengröße (numerisch, > 0), Default 10. |
| `Paging/Paginator.cs` | **Operation (pure)** | `(CsvDocument, int pageSize) → IReadOnlyList<CsvDocument>`: schneidet Datensätze in Seiten (letzte ggf. kürzer), immer ≥ 1 Seite. |
| `Navigation/NavigationCommand.cs` | **Data** | `enum { First, Previous, Next, Last, Exit, None }`. |
| `Navigation/NavigationCommandMapper.cs` | **Operation (pure)** | `char → NavigationCommand`, case-insensitive; Unbekanntes → `None`. |
| `Navigation/PageNavigator.cs` | **Operation** über Index-State | aktueller Seitenindex + `First/Previous/Next/Last` mit Klemmen an den Rändern. |
| `IO/IConsole.cs` | **I/O-Vertrag** | `Clear()`, `Write(string)`, `ReadKey() → char`. |
| `Viewer/InteractiveViewer.cs` | **Integration (in BL, testbar)** | Loop: `Clear` → Seite rendern (`TableRenderer`) + Menü schreiben → Taste lesen → auf Command mappen → `PageNavigator` anwenden, bis `Exit`. Hält die Menü-Konstante. |

### Neue Dateien im Entry-Point `CsvViewer/`

| Datei | Rolle | Beschreibung |
|-------|-------|--------------|
| `SystemConsole.cs` | **I/O-Randbaustein** | `IConsole` über `System.Console` (`Clear`, `Write`, `ReadKey(intercept: true)`). |
| `Program.cs` (ändern) | **Composition Root** | Args parsen → `FileReader` → `CsvParser` → `Paginator` → `InteractiveViewer` mit `SystemConsole` + `TableRenderer` starten; `Result`/Fehler auf Meldung (stderr) + Exit-Code abbilden. |

### Wiederverwendet

`Common/Result.cs`, `Csv/CsvDocument.cs`, `IO/FileReader.cs`, `Csv/CsvParser.cs` (R00001), `Rendering/TableRenderer.cs` (R00002), `Logging/ConsoleLogger.cs` (Fehlerausgabe).

### Aufräumen (aus Scaffolding)

- `CommandLineArguments/CommandLineOptions.cs` und `CommandLineArgumentsParser.cs` (Flag-basiert) werden durch `ViewerArguments` + `ArgumentsParser` ersetzt.
- `PackageReference CommandLineParser` aus `CsvViewer.BL.csproj` entfernen — positionale Argumente brauchen keine Library.

### IODA/SRP-Zuordnung

| Schicht | Bausteine |
|---------|-----------|
| Data | `ViewerArguments`, `NavigationCommand`, `CsvDocument` |
| Operation (pure) | `ArgumentsParser`, `Paginator`, `NavigationCommandMapper`, `PageNavigator` |
| I/O-Rand | `SystemConsole` (real), `FileReader` |
| Integration | `InteractiveViewer` (Loop), `Program` (Composition Root) |

### Tests

| Testdatei | Prüft |
|-----------|-------|
| `Source/CsvViewer/CsvViewer.BL.Tests/CommandLineArguments/ArgumentsParserTests.cs` | gültig, fehlende Datei, zu viele Args, nicht-numerisch, ≤ 0, Default 10 |
| `Source/CsvViewer/CsvViewer.BL.Tests/Paging/PaginatorTests.cs` | Aufteilung, kürzere letzte Seite, 0 Datensätze → 1 Seite |
| `Source/CsvViewer/CsvViewer.BL.Tests/Navigation/PageNavigatorTests.cs` | Klemmen an Rändern, Sprünge First/Last |
| `Source/CsvViewer/CsvViewer.BL.Tests/Navigation/NavigationCommandMapperTests.cs` | Key→Command, case-insensitive, Unbekanntes → None |
| `Source/CsvViewer/CsvViewer.BL.Tests/Viewer/InteractiveViewerTests.cs` | mit Fake-`IConsole`: Tastenfolge → erwartete gezeichnete Seiten, Ignorieren, Exit |
| `Source/CsvViewer/CsvViewer.BL.IntegrationTests/Viewer/ViewerFlowIntegrationTests.cs` | Temp-Datei → Einlesen+Parsen+Paging+Render der ersten Seite (Fake-Konsole) |

## Abhängigkeiten

- Abhängig von: R00001 (Einlesen), R00002 (Rendering)
- Blockiert: —

## Out-of-Scope

- Interne Details von Einlesen/Parsen/Validierung → R00001
- Tabellen-Rendering-Format, Spaltenbreiten → R00002
- Horizontales Scrollen, Suchen/Filtern/Sortieren, Bearbeiten von Zellen — nicht Teil von CSV Viewer I
- Andere Delimiter als `;`, Quoting/Escaping

## Notizen

- CLI ist positional exakt nach Kursvorgabe: `csvviewer datei.csv [seitengröße]`, Default 10 Datensätze (nur Datenzeilen; Menü/Rahmen/Kopf kommen obendrauf).
- `InteractiveViewer` liegt bewusst in der BL (nicht im Entry-Point), damit der Loop mit einer Fake-`IConsole` ohne echte Konsole testbar ist; `Program` bleibt reine Verdrahtung.
- Die konkrete Konsole (`SystemConsole`) sitzt am Rand im Entry-Point-Projekt, damit die BL frei von direkter `System.Console`-Nutzung im Loop bleibt.
- Seiten werden einmalig vorberechnet (`Paginator` → Liste von Seiten-`CsvDocument`s); der `TableRenderer` bestimmt die Spaltenbreiten je Seite beim Zeichnen (R00002).

### Verworfene Alternativen

- **Zeilen-Eingabe mit Enter** — verworfen zugunsten einzelnem Tastendruck (`ReadKey`).
- **Anhängen statt Löschen** der Ausgabe — verworfen zugunsten `Clear` + Neuzeichnen (Pager-Effekt).
- **Tolerant auf Default 10 bei ungültiger Seitengröße** — verworfen zugunsten Abbruch mit Meldung.
- **Hinweiszeile bei ungültiger Taste** — verworfen zugunsten stillem Ignorieren + Neuzeichnen.
- **Loop im Entry-Point (`Program`)** — verworfen zugunsten testbarem `InteractiveViewer` in der BL.
- **`CommandLineParser`-Library / Flag-CLI** — verworfen zugunsten positionaler Argumente exakt nach Kursvorgabe.
