---
id: A00004
name: Seiten-Darstellung
code:
ebene: Komponente
eltern: A00002
repo:
status: Veraltet
herkunft: code
quellen: []
verwandt: [A00010, A00011]
domaene: [D00002, D00003]
---

# Seiten-Darstellung

> **Veraltet seit [R00008](../../Anforderungen/R00008-ein-topic-ein-einstiegspunkt.md).**
> Aufgegangen in [Seiten-Aufteilung](A00010-seiten-aufteilung.md) und
> [Tabellen-Rendering](A00011-tabellen-rendering.md), die beide direkt unter
> [CsvViewer CLI](A00002-csvviewer-cli.md) hängen.

## Verantwortung

Machte aus einem CSV-Dokument die sichtbare Seitenansicht.

## Schnittstellen

Entfällt — die Schnittstellen liegen bei den Nachfolgern.

## Abhaengigkeiten

Entfällt.

## Entscheidungen

Keine eigenen mehr. Die Begründung, warum das Rendering einen String zurückgibt statt selbst auf die Konsole zu schreiben, steht jetzt bei [Tabellen-Rendering](A00011-tabellen-rendering.md).

## Offene Fragen

Keine.

## Notizen / Quellen

**Warum dieser Baustein aufgelöst wurde:** Er fasste zwei Dinge zusammen, die einander nicht kennen. `Paginator` und `TableRenderer` referenzieren sich gegenseitig nicht, haben verschiedene Aufrufer und laufen in verschiedenen Phasen — die Aufteilung einmalig beim Start, das Rendern bei jedem Zeichenvorgang. Ausschlaggebend war, dass `ITableRenderer.Render` gar kein `PagedDocument` entgegennimmt, sondern `CsvHeader` und `CsvRowCollection`: Der Renderer hat mit dem Ergebnis der Paginierung nichts zu tun.

Der Fehlschnitt fiel beim Zeichnen des Datenfluss-Diagramms auf, weil dieser Baustein als einziger in zwei Phasen auftauchte.

Der Eintrag bleibt als Spur der Modellhistorie erhalten und wird nicht gelöscht.
