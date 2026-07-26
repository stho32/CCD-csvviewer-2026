---
id: A00009
name: CSV-Interpretation
code: CsvParser
ebene: Komponente
eltern: A00003
repo:
status: Aktiv
herkunft: code
quellen:
  - Source/CsvViewer/CsvViewer.BL/DocumentAcquisition/CsvParser.cs
  - Source/CsvViewer/CsvViewer.BL/DocumentAcquisition/CsvDocument.cs
  - Source/CsvViewer/CsvViewer.BL/DocumentAcquisition/CsvHeader.cs
  - Source/CsvViewer/CsvViewer.BL/DocumentAcquisition/CsvRow.cs
  - Source/CsvViewer/CsvViewer.BL/DocumentAcquisition/CsvRowCollection.cs
verwandt: [A00008]
domaene: [D00001]
---

# CSV-Interpretation

## Verantwortung

Interpretiert Textzeilen als Kopfzeile mit zugehörigen Datensätzen.

## Schnittstellen

**Eingehend** — `CsvParser.Parse(IReadOnlyList<string> lines)`.

**Ausgehend** — `Result<CsvDocument>`. Leere Eingabe und Zeilen mit abweichender Feldanzahl werden zu Fehlermeldungen; die Meldung nennt die betroffene Zeilennummer. Eine Datei mit nur einer Kopfzeile ist gültig und ergibt 0 Datensätze.

Datentypen dieses Bausteins: `CsvDocument`, `CsvHeader`, `CsvRow`, `CsvRowCollection`.

## Abhaengigkeiten

Keine. Pure Operation ohne I/O.

## Entscheidungen

Generisches Datenmodell ohne Bindung an ein Fachmodell — Feldwerte sind ausschliesslich positionsbasiert zugreifbar, nicht über Spaltennamen. Ein typgebundenes Zeilenmodell wurde in [R00001](../../Anforderungen/R00001-csv-datei-einlesen.md) verworfen.

## Offene Fragen

Keine.

## Notizen / Quellen

Format-Vorgaben (UTF-8, Trenner `;`, erste Zeile ist Kopfzeile, keine Quoting-Regeln) sind Kursvorgabe und im [CSV-Dokument](../Domaenenwissen/D00001-csv-dokument.md) festgehalten.
