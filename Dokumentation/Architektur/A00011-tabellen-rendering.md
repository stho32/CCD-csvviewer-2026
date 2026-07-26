---
id: A00011
name: Tabellen-Rendering
code: TableRendering/
ebene: Komponente
eltern: A00002
repo:
status: Aktiv
herkunft: code
quellen:
  - Source/CsvViewer/CsvViewer.BL/TableRendering/
verwandt: [A00013]
domaene: [D00003]
---

# Tabellen-Rendering

## Verantwortung

Formt eine Seite in eine ausgerichtete Texttabelle.

## Schnittstellen

**Eingehend** — `ITableRenderer.Render(CsvHeader header, CsvRowCollection rows)`.

**Ausgehend** — `Result<string>` mit der fertigen Tabelle. Nichts wird selbst ausgegeben.

Format: Zelle = Wert auf Spaltenbreite aufgefüllt plus `|`, kein Aussenrahmen, eine Trennlinie aus `-` und `+` zwischen Kopf und Daten. Spaltenbreite = längster Wert **inklusive** Spaltenname, berechnet über genau die Zeilen der aktuellen Seite.

## Abhaengigkeiten

Keine. Pure Operation, läuft bei jedem Zeichenvorgang erneut.

Wird über die Abstraktion `ITableRenderer` benutzt, nicht über den konkreten Typ — dadurch kann [Viewer-Ablauf](A00013-viewer-ablauf.md) im Test mit einem fehlschlagenden Renderer geprüft werden.

## Entscheidungen

String-Rückgabe statt direkter Konsolenausgabe, damit das Format zeichengenau testbar bleibt ([R00002](../../Anforderungen/R00002-seite-als-tabelle-rendern.md)).

## Offene Fragen

Keine.

## Notizen / Quellen

Das zeichengenaue Zielformat steht im [Tabellen-Ausgabeformat](../Domaenenwissen/D00003-tabellen-ausgabeformat.md).

Anders als `IConsole` liegt `ITableRenderer` direkt neben seiner Implementierung. Beide Muster sind im Repo vertreten — hier Vertrag und Umsetzung beieinander, dort getrennt über die Projektgrenze.
