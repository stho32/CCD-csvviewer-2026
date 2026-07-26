---
id: A00010
name: Seiten-Aufteilung
code: Paginator
ebene: Komponente
eltern: A00004
repo:
status: Aktiv
herkunft: code
quellen:
  - Source/CsvViewer/CsvViewer.BL/PagePresentation/Paginator.cs
  - Source/CsvViewer/CsvViewer.BL/PagePresentation/PagedDocument.cs
  - Source/CsvViewer/CsvViewer.BL/PagePresentation/CsvPageCollection.cs
verwandt: [A00007, A00012]
domaene: [D00002]
---

# Seiten-Aufteilung

## Verantwortung

Zerlegt die Datensätze eines CSV-Dokuments in Seiten fester Größe.

## Schnittstellen

**Eingehend** — `Paginator.Paginate(CsvDocument document, int pageSize)`.

**Ausgehend** — `Result<PagedDocument>`: die Kopfzeile genau einmal, dazu die Seiten als Zeilen-Teilmengen in Leserichtung.

Regeln: Seitenzahl = ⌈Datensätze / Seitengröße⌉, mindestens 1. Letzte Seite gegebenenfalls kürzer. Bei 0 Datensätzen entsteht genau eine leere Seite.

Datentypen dieses Bausteins: `PagedDocument`, `CsvPageCollection`.

## Abhaengigkeiten

Keine. Pure Operation, läuft einmalig beim Start.

## Entscheidungen

Seiten sind Zeilen-Teilmengen innerhalb eines `PagedDocument`, kein jeweils eigenes `CsvDocument` — die Kopfzeile wird nicht je Seite dupliziert.

## Offene Fragen

Keine.

## Notizen / Quellen

Zählweise und Grenzfälle im [Seiten-Modell](../Domaenenwissen/D00002-seiten-modell.md).
