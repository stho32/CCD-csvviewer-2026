---
id: R00007
title: "Datenobjekte in Data-Unterordner, damit der Einstiegspunkt sichtbar bleibt"
type: Refactoring
status: Neu
created: 2026-07-26
---

# R00007: Datenobjekte in Data-Unterordner, damit der Einstiegspunkt sichtbar bleibt

## Beschreibung

In `DocumentAcquisition/` liegen fünf Dateien nebeneinander, von denen vier das Ergebnis-Modell sind und nur eine der Einstiegspunkt:

```
CsvDocument.cs
CsvHeader.cs
CsvParser.cs        <- der Einstieg, optisch gleichrangig
CsvRow.cs
CsvRowCollection.cs
```

Wer den Ordner öffnet, sieht fünf gleich aussehende Dateien und muss erst herausfinden, welche die Arbeit tut. Das Verhältnis von 1:4 zugunsten des Beiwerks erschwert die Orientierung, und alphabetisch steht der Parser mitten zwischen seinen eigenen Rückgabetypen.

Die Information, was der Einstieg ist, existiert bereits im Architektur-Modell — A00009 nennt unter „Schnittstellen" ausdrücklich `CsvParser.Parse(...)`. Sie steht nur nirgends dort, wo jemand den Code liest.

Diese Anforderung zieht die Datenobjekte in einen Unterordner `Data/`, sodass im Topic-Ordner nur noch die Operationen stehen. Der Name folgt der IODA-Sprache, die in R00001 bis R00003 durchgängig verwendet wird: Data ist die Schicht ohne Logik.

Reines Refactoring: Kein Verhalten ändert sich, keine Signatur, keine Ausgabe. Nur Ablageort und Namespace.

## Regel

`Data/` wird angelegt, wenn ein Topic **mehr als ein** Datenobjekt enthält. Bei genau einem bleibt es oben liegen — dort geht der Einstieg nicht unter, und ein Ordner für eine Datei kostet mehr als er bringt.

| Topic | Operationen | Datenobjekte | `Data/` |
|---|---|---|---|
| `DocumentAcquisition/` | `CsvParser` | 4 | ja |
| `PagePresentation/` | `Paginator`, `TableRenderer`, `ITableRenderer` | 2 | ja |
| `Interaction/` | `NavigationCommandMapper`, `PageNavigator`, `InteractiveViewer` | 1 | nein |
| `CommandLineInterpretation/` | `ArgumentsParser` | 1 | nein |

Diese Verschachtelung gilt ausschliesslich **innerhalb** eines Topic-Ordners. Eine Zwischenebene zwischen `CsvViewer.BL` und den Topics bleibt ausgeschlossen (R00005).

## Zielstruktur

```
CsvViewer.BL/
  Common/                      Result
  HostContracts/               IConsole, IFileReader, ILogger
  CommandLineInterpretation/   ArgumentsParser, ViewerArguments
  DocumentAcquisition/         CsvParser
    Data/                      CsvDocument, CsvHeader, CsvRow, CsvRowCollection
  PagePresentation/            ITableRenderer, Paginator, TableRenderer
    Data/                      PagedDocument, CsvPageCollection
  Interaction/                 NavigationCommand, NavigationCommandMapper,
                               PageNavigator, InteractiveViewer
```

## User Stories

Given/When/Then-Szenarien: siehe [user-stories/R00007.md](user-stories/R00007.md).

- **US1 — Einstiegspunkt auf einen Blick**: Als Entwickler will ich beim Öffnen eines Topic-Ordners sofort sehen, welcher Typ die Arbeit tut, damit ich nicht erst fünf Dateien öffnen muss.

## Akzeptanzkriterien

### Struktur
- [ ] `DocumentAcquisition/` enthält oben nur noch `CsvParser.cs`
- [ ] `DocumentAcquisition/Data/` enthält `CsvDocument`, `CsvHeader`, `CsvRow`, `CsvRowCollection`
- [ ] `PagePresentation/Data/` enthält `PagedDocument` und `CsvPageCollection`
- [ ] `Interaction/` und `CommandLineInterpretation/` bleiben unverändert — je nur ein Datenobjekt
- [ ] Kein `Data/`-Ordner enthält einen Typ mit Logik

### Namespaces
- [ ] Namespace folgt weiterhin dem Ordner: `CsvViewer.BL.DocumentAcquisition.Data`, `CsvViewer.BL.PagePresentation.Data`
- [ ] Alle `using`-Direktiven in BL, Entry Point und beiden Testprojekten sind angepasst
- [ ] Eine Klasse pro Datei bleibt gewahrt

### Verhalten
- [ ] Keine Signatur, kein Verhalten und keine Ausgabe ändert sich
- [ ] Alle 92 Tests laufen unverändert durch
- [ ] Build ohne Warnungen

## Status

- [ ] Neu

## Abhängigkeiten

- Abhängig von: R00005, R00006 (Topic-Ordner)
- Blockiert: —

## Out-of-Scope

- Ordnerstruktur der Testprojekte
- `Common/` und `HostContracts/` — dort gibt es nichts zu trennen
- Inhaltliche Änderungen an Klassen, Methoden oder Tests

## Notizen

- Auslöser: Der Parser sieht vom Namen her aus wie alles andere und geht zwischen den Datenobjekten unter.
- `Data/` statt `Model/`, weil IODA die durchgängige Architektur-Sprache des Projekts ist.

### Verworfene Alternativen

- **Datenobjekte in eine Datei zusammenfassen** — verworfen, „eine Klasse pro Datei" ist gesetzte Regel.
- **`README.md` je Topic-Ordner** — verworfen, in Visual Studio nicht sichtbar.
- **Einstieg trägt den Ordnernamen** (`DocumentAcquisition.cs`) — verworfen, `CsvParser` ist der präzisere Name.
- **Sichtbarkeit statt Ablage** (Einstieg `public`, Rest `internal`) — verworfen, die Datenobjekte sind Rückgabetypen und müssen öffentlich sein; die Trennung wäre nicht trennscharf.
