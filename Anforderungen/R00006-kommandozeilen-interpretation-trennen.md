---
id: R00006
title: "Kommandozeilen-Interpretation von der Dokument-Beschaffung trennen"
type: Refactoring
status: Erledigt
created: 2026-07-26
---

# R00006: Kommandozeilen-Interpretation von der Dokument-Beschaffung trennen

## Beschreibung

R00005 hat `CommandLineArguments/` und `Csv/` zu einem Ordner `DocumentAcquisition/` zusammengelegt. Das war ein Fehlschnitt: Die beiden Teile haben keine Berührung miteinander. `ArgumentsParser` kennt kein CSV, `CsvParser` kennt keine Argumente.

Der inhaltliche Grund liegt tiefer als die Ordnerfrage. Die Kommandozeile liefert **zwei** Werte, die in verschiedene Richtungen fliessen: Der Dateipfad speist die Dokument-Beschaffung, die Seitengrösse die Seiten-Aufteilung. Die Auswertung des Aufrufs ist damit kein Teil der Dokument-Beschaffung, sondern ein eigener Schritt davor, der beide Zweige versorgt.

Das Architektur-Modell hatte den Widerspruch bereits sichtbar gemacht: Der Baustein stand als Kind der Dokument-Beschaffung, verwies aber ausschliesslich auf das Seiten-Modell (`D00002`) und auf die Seiten-Aufteilung (`A00010`) — kein einziger Bezug zum CSV-Dokument.

Diese Anforderung zieht die Kommandozeilen-Auswertung auf die oberste Komponenten-Ebene und benennt sie parallel zur `CsvInterpretation`: Beide interpretieren eine Eingabe, nur aus verschiedenen Quellen.

Reines Refactoring: Kein Verhalten ändert sich, keine Signatur, keine Ausgabe. Nur Ablageort, Namespace und die Einordnung im Modell.

## Zielstruktur

```
CsvViewer.BL/
  Common/                      Result
  HostContracts/               IConsole, IFileReader, ILogger
  CommandLineInterpretation/   ArgumentsParser, ViewerArguments      <- neu
  DocumentAcquisition/         CsvParser, CsvDocument, CsvHeader,
                               CsvRow, CsvRowCollection
  PagePresentation/            Paginator, PagedDocument, CsvPageCollection,
                               ITableRenderer, TableRenderer
  Interaction/                 NavigationCommand, NavigationCommandMapper,
                               PageNavigator, InteractiveViewer
```

## User Stories

Given/When/Then-Szenarien: siehe [user-stories/R00006.md](user-stories/R00006.md).

- **US1 — Getrennte Belange getrennt ablegen**: Als Entwickler will ich die Auswertung der Kommandozeile getrennt vom CSV-Parsing finden, damit ich beim Ändern des einen nicht über das andere stolpere.

## Akzeptanzkriterien

### Struktur
- [x] `ArgumentsParser` und `ViewerArguments` liegen in `CommandLineInterpretation/`
- [x] `DocumentAcquisition/` enthält ausschliesslich CSV-bezogene Typen
- [x] Namespace `CsvViewer.BL.CommandLineInterpretation` entspricht dem Ordner

### Modell
- [x] Der Baustein A00007 heisst `Kommandozeilen-Interpretation` und hat `eltern: A00002`
- [x] A00003 Dokument-Beschaffung hat nur noch die Kinder A00008 und A00009
- [x] Die Ordner-Zuordnungstabelle in A00002 ist aktualisiert

### Verhalten
- [x] Keine Signatur, kein Verhalten und keine Ausgabe ändert sich
- [x] Alle 92 Tests laufen unverändert durch
- [x] Build ohne Warnungen

## Status

- [x] Erledigt

## Abhängigkeiten

- Abhängig von: R00005 (Ordnerstruktur nach Architektur-Gruppen)
- Blockiert: —

## Out-of-Scope

- Jede weitere Ordner-Änderung; die übrigen vier Ordner bleiben unangetastet
- Inhaltliche Änderungen an Klassen, Methoden oder Tests

## Notizen

- Auslöser war die Beobachtung, dass `DocumentAcquisition/` zwei klar getrennte Sub-Topics enthält.
- Der Name ist bewusst parallel zu `CsvInterpretation` gewählt: Beide deuten eine Eingabe, die eine aus der Kommandozeile, die andere aus Textzeilen.

### Verworfene Alternativen

- **Zwei Unterordner in `DocumentAcquisition/`** — verworfen; die zweistufige Schachtelung wurde schon in R00005 als zu teuer bewertet, und sie würde die falsche Zusammengehörigkeit zementieren.
- **`CommandLine/` oder `Invocation/` als Ordnername** — verworfen zugunsten der Parallele zur `CsvInterpretation`.
