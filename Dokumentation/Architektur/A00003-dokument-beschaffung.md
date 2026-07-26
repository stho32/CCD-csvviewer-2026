---
id: A00003
name: Dokument-Beschaffung
code: DocumentAcquisition/
ebene: Komponente
eltern: A00002
repo:
status: Aktiv
herkunft: code
quellen:
  - Source/CsvViewer/CsvViewer.BL/DocumentAcquisition/
  - Source/CsvViewer/CsvViewer.BL/HostContracts/IFileReader.cs
verwandt: [A00010]
domaene: [D00001]
---

# Dokument-Beschaffung

## Verantwortung

Macht aus einem Dateipfad ein geprüftes CSV-Dokument.

## Schnittstellen

**Eingehend** — der Dateipfad aus der [Kommandozeilen-Interpretation](A00007-kommandozeilen-interpretation.md).

**Ausgehend** — `Result<CsvDocument>`; im Fehlerfall eine Meldung statt eines Dokuments.

## Abhaengigkeiten

Innere Struktur — zwei Schritte in fester Reihenfolge:

1. [Datei-Zugriff](A00008-datei-zugriff.md) — Zeilen von der Platte holen
2. [CSV-Interpretation](A00009-csv-interpretation.md) — Zeilen als Kopfzeile plus Datensätze deuten

Extern: Dateisystem (lesend). Weiter gereicht wird das Ergebnis an [Seiten-Aufteilung](A00010-seiten-aufteilung.md).

## Entscheidungen

Datei-Zugriff und CSV-Interpretation sind bewusst getrennt: Der Datei-Zugriff kennt kein CSV, die Interpretation kein Dateisystem. Begründet in [R00001](../../Anforderungen/R00001-csv-datei-einlesen.md).

## Offene Fragen

Keine.

## Notizen / Quellen

Fachlicher Begriff: [CSV-Dokument](../Domaenenwissen/D00001-csv-dokument.md) — dort stehen Format-Vorgaben und Validierungsregeln.
