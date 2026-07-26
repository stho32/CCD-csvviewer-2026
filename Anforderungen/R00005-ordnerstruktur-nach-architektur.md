---
id: R00005
title: "Ordnerstruktur der BL nach Architektur-Gruppen schneiden"
type: Refactoring
status: Erledigt
created: 2026-07-26
---

# R00005: Ordnerstruktur der BL nach Architektur-Gruppen schneiden

## Beschreibung

`CsvViewer.BL` enthält neun gleichrangige Ordner (`CommandLineArguments`, `Common`, `Csv`, `IO`, `Logging`, `Navigation`, `Paging`, `Rendering`, `Viewer`). Jeder einzelne ist fachlich sauber benannt, aber es fehlt die Klammer darüber: Wer das Projekt öffnet, sieht neun Dinge nebeneinander und muss sich die Zusammenhänge selbst erschliessen.

Das Architektur-Modell (`Dokumentation/Architektur/`) hat diese Klammer bereits herausgearbeitet — drei fachliche Gruppen plus die Verdrahtung. Diese Anforderung bringt die Ordnerstruktur mit dem Modell zur Deckung, sodass die Architektur im Explorer sichtbar wird statt nur im Dokument.

Zusätzlich werden die drei Verträge, deren Umsetzung im Entry Point liegt (`IConsole`, `IFileReader`, `ILogger`), in einem gemeinsamen Ordner `HostContracts` gebündelt. Er ist das Gegenstück zu `HostSpecific/` im Entry-Point-Projekt: Vertrag hier, Umsetzung dort. Bisher lagen sie über `IO/` und `Logging/` verstreut.

Es handelt sich um ein reines Refactoring: Kein Verhalten ändert sich, keine Signatur, keine Ausgabe. Nur Ablageort und Namespace.

## Zielstruktur

```
CsvViewer.BL/
  Common/                Result
  HostContracts/         IConsole, IFileReader, ILogger
  DocumentAcquisition/   ArgumentsParser, ViewerArguments, CsvParser,
                         CsvDocument, CsvHeader, CsvRow, CsvRowCollection
  PagePresentation/      Paginator, PagedDocument, CsvPageCollection,
                         ITableRenderer, TableRenderer
  Interaction/           NavigationCommand, NavigationCommandMapper,
                         PageNavigator, InteractiveViewer
```

Die Ordnernamen sind die englische Entsprechung der Bausteinnamen im Architektur-Modell:
`DocumentAcquisition` = Dokument-Beschaffung, `PagePresentation` = Seiten-Darstellung,
`Interaction` = Bedienung. `HostContracts` entspricht bewusst keinem Baustein — es ist
eine Ablage nach Wirkungsart, während das Modell dem Datenfluss folgt.

Englisch wie alle übrigen Bezeichner im Code; die deutschen Bausteinnamen bleiben der Dokumentation vorbehalten.

## User Stories

Given/When/Then-Szenarien: siehe [user-stories/R00005.md](user-stories/R00005.md).

- **US1 — Architektur im Explorer erkennen**: Als Entwickler will ich beim Öffnen von `CsvViewer.BL` auf einen Blick die fachlichen Gruppen sehen, damit ich neuen Code ohne Nachdenken am richtigen Ort ablege.
- **US2 — Verträge an einem Ort**: Als Entwickler will ich alle Verträge, die der Host implementiert, in einem Ordner finden, damit die Grenze zwischen Geschäftslogik und Host-Anbindung sichtbar ist.

## Akzeptanzkriterien

### Struktur
- [x] `CsvViewer.BL` enthält genau die fünf Ordner `Common`, `HostContracts`, `DocumentAcquisition`, `PagePresentation`, `Interaction`
- [x] Die Ordner `CommandLineArguments`, `Csv`, `IO`, `Logging`, `Navigation`, `Paging`, `Rendering`, `Viewer` existieren nicht mehr
- [x] `IConsole`, `IFileReader` und `ILogger` liegen gemeinsam in `HostContracts`
- [x] `ITableRenderer` bleibt bei seiner Implementierung in `PagePresentation` — er ist ein BL-interner Vertrag, kein Host-Vertrag

### Namespaces
- [x] Jeder Namespace entspricht seinem Ordner (`CsvViewer.BL.DocumentAcquisition`, `CsvViewer.BL.PagePresentation`, `CsvViewer.BL.Interaction`, `CsvViewer.BL.HostContracts`, `CsvViewer.BL.Common`)
- [x] Alle `using`-Direktiven in BL, Entry Point und beiden Testprojekten sind angepasst
- [x] File-scoped Namespaces bleiben erhalten

### Verhalten
- [x] Keine Signatur, kein Verhalten und keine Ausgabe ändert sich
- [x] Alle Tests laufen unverändert durch — kein Test wird angepasst ausser seinen `using`-Direktiven
- [x] Build ohne Warnungen

## Status

- [x] Erledigt

## Abhängigkeiten

- Abhängig von: R00004 (Adapter-Schnitt zwischen BL und Host)
- Blockiert: —

## Out-of-Scope

- Ordnerstruktur des Entry-Point-Projekts — `Program.cs` plus `HostSpecific/` ist bereits stimmig
- Ordnerstruktur der Testprojekte
- Jede inhaltliche Änderung an Klassen, Methoden oder Tests

## Notizen

- Auslöser ist die Verfeinerung des Architektur-Modells auf Komponenten-Ebene: Sechs von acht Bausteinen entsprachen schon 1:1 einem Ordner, nur die Gruppenebene darüber fehlte im Code.
- Bewusst einstufig: Eine zweistufige Schachtelung (`DocumentAcquisition/Csv/`) würde den Modellbaum exakt abbilden, kostet bei 18 Typen aber mehr Navigationsaufwand, als die Symmetrie einbringt.

### Verworfene Alternativen

- **Alles belassen** — verworfen; die neun flachen Ordner sind der einzige Ort, an dem die Architektur nicht sichtbar ist.
- **Zweistufige Schachtelung nach Modellbaum** — verworfen zugunsten flacher Gruppen, siehe Notizen.
- **Deutsche Ordnernamen analog zum Architektur-Modell** — verworfen; alle Bezeichner im Code sind englisch.
- **`ILogger` in den Host verschieben** — nicht Teil dieser Anforderung, obwohl ihn aktuell kein BL-Typ nutzt; das wäre eine inhaltliche Entscheidung über den Vertrag, nicht über seine Ablage.
