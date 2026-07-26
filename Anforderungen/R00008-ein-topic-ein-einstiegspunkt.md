---
id: R00008
title: "Ein Topic, ein Einstiegspunkt — PagePresentation auftrennen, Innenleben absenken"
type: Refactoring
status: Neu
created: 2026-07-26
---

# R00008: Ein Topic, ein Einstiegspunkt — PagePresentation auftrennen, Innenleben absenken

## Beschreibung

R00007 hat die Datenobjekte abgesenkt, damit der Einstiegspunkt eines Topics sichtbar wird. In `Interaction/` hilft das nicht: Dort steht `InteractiveViewer` weiterhin zwischen `NavigationCommandMapper` und `PageNavigator`, weil die Trennung dort nicht Daten-gegen-Operation ist, sondern Einstieg-gegen-Innenleben.

Die allgemeinere Regel lautet:

> **Ein Topic hat genau einen Einstiegspunkt** — einen Typ, dessen Methode von ausserhalb des Ordners aufgerufen wird. Datentypen zählen nicht mit, die werden nur gelesen. Hat ein Topic zwei Einstiegspunkte, sind es zwei Topics.

Angewendet auf den Bestand deckt sie zusätzlich einen Fehlschnitt auf:

| Topic | Von aussen aufgerufen | Urteil |
|---|---|---|
| `CommandLineInterpretation/` | `ArgumentsParser.Parse` | 1 |
| `DocumentAcquisition/` | `CsvParser.Parse` | 1 |
| `PagePresentation/` | `Paginator.Paginate` **und** `ITableRenderer.Render` | **2** |
| `Interaction/` | `InteractiveViewer.Run` | 1 |

`PagePresentation/` fasst zwei Dinge zusammen, die einander nicht kennen: `Paginator` und `TableRenderer` referenzieren sich gegenseitig nicht, haben verschiedene Aufrufer (`Program` beziehungsweise `InteractiveViewer`) und laufen in verschiedenen Phasen — die Aufteilung einmalig beim Start, das Rendern bei jedem Zeichenvorgang. Entscheidend: `ITableRenderer.Render` nimmt `CsvHeader` und `CsvRowCollection` entgegen, **nicht** `PagedDocument`. Der Renderer hat mit dem Ergebnis der Paginierung nichts zu tun.

Es ist derselbe Fehlschnitt wie in R00006: thematisch klang „hat mit Seiten zu tun" zusammengehörig, technisch ist es das nicht.

Reines Refactoring: Kein Verhalten ändert sich, keine Signatur, keine Ausgabe. Nur Ablageort und Namespace.

## Ablage-Regel

```
Topic/
  <Einstiegspunkt>.cs    allein oben — der eine von aussen gerufene Typ
  Data/                  Datentypen des Topics
  Operations/            Operationen, die nur der Einstiegspunkt nutzt
```

`Data/` und `Operations/` folgen der IODA-Sprache des Projekts. Ein Unterordner wird nur angelegt, wenn es etwas hineinzulegen gibt — die Sonderregel „erst ab zwei Datenobjekten" aus R00007 entfällt, weil die neue Regel den Einstieg immer allein stellt.

## Zielstruktur

```
CsvViewer.BL/
  Common/                      Result
  HostContracts/               IConsole, IFileReader, ILogger
  CommandLineInterpretation/   ArgumentsParser
    Data/                      ViewerArguments
  DocumentAcquisition/         CsvParser
    Data/                      CsvDocument, CsvHeader, CsvRow, CsvRowCollection
  Pagination/                  Paginator
    Data/                      PagedDocument, CsvPageCollection
  TableRendering/              ITableRenderer, TableRenderer
  Interaction/                 InteractiveViewer
    Data/                      NavigationCommand
    Operations/                NavigationCommandMapper, PageNavigator
```

`TableRendering/` behält Vertrag und Umsetzung nebeneinander: `ITableRenderer` ist der Einstiegspunkt, `TableRenderer` seine einzige Implementierung — sie zu trennen würde nichts sichtbar machen.

## User Stories

Given/When/Then-Szenarien: siehe [user-stories/R00008.md](user-stories/R00008.md).

- **US1 — Einstiegspunkt steht allein**: Als Entwickler will ich in jedem Topic-Ordner genau eine Datei auf oberster Ebene sehen, damit ich ohne Suchen weiss, wo ich zu lesen anfange.
- **US2 — Ein Topic tut eine Sache**: Als Entwickler will ich, dass Paginierung und Rendering getrennt liegen, weil sie einander nicht brauchen.

## Akzeptanzkriterien

### Auftrennung
- [ ] `PagePresentation/` existiert nicht mehr
- [ ] `Pagination/` enthält `Paginator.cs` und `Data/` mit `PagedDocument`, `CsvPageCollection`
- [ ] `TableRendering/` enthält `ITableRenderer.cs` und `TableRenderer.cs`

### Ablage
- [ ] `Interaction/` enthält oben nur `InteractiveViewer.cs`
- [ ] `Interaction/Operations/` enthält `NavigationCommandMapper` und `PageNavigator`
- [ ] `Interaction/Data/` enthält `NavigationCommand`
- [ ] `CommandLineInterpretation/Data/` enthält `ViewerArguments`
- [ ] Jedes Topic hat genau einen Typ auf oberster Ebene — Ausnahme `TableRendering/` mit Vertrag plus Umsetzung

### Namespaces
- [ ] Namespace folgt dem Ordner, auch bei `Operations/`
- [ ] Alle `using`-Direktiven in BL, Entry Point und beiden Testprojekten sind angepasst
- [ ] Eine Klasse pro Datei bleibt gewahrt

### Verhalten
- [ ] Keine Signatur, kein Verhalten und keine Ausgabe ändert sich
- [ ] Alle 92 Tests laufen unverändert durch
- [ ] Build ohne Warnungen

## Status

- [ ] Neu

## Abhängigkeiten

- Abhängig von: R00007
- Blockiert: —

## Out-of-Scope

- Ordnerstruktur der Testprojekte
- `Common/` und `HostContracts/` — Querschnitt beziehungsweise reine Vertragssammlung, kein Topic mit Einstiegspunkt
- Inhaltliche Änderungen an Klassen, Methoden oder Tests

## Notizen

- Die Geschäftslogik hat danach sechs Topics plus zwei Sonderordner. Das liegt über der Faustregel „drei bis fünf", die für Architektur-Bausteine gilt — hier bewusst in Kauf genommen: Sechs klar geschnittene Topics sind besser als fünf, von denen eines zwei Dinge vermischt.
- Der Fehlschnitt fiel beim Zeichnen des Datenfluss-Diagramms auf, weil `PagePresentation` als einziger Baustein in zwei Phasen auftauchte.

### Verworfene Alternativen

- **Sechs Topics nach Lebenszyklus gruppieren** (`Startup/`, `Runtime/`) — verworfen; das gruppiert nach Zeit statt nach Fachlichkeit und führt die Verschachtelung wieder ein, die R00005 abgeschafft hat.
- **`TableRendering` unter `Interaction` hängen** — verworfen; der Renderer ist eine pure Operation, die Bedienung ist Integration. Das mischt Wirkungsarten.
- **`PagePresentation` belassen** — verworfen; zwei Einstiegspunkte in einem Ordner sind der Grund, warum der Einstieg dort nicht auffindbar ist.
