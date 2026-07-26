---
id: A00003
name: Dokument-Beschaffung
ebene: Komponente
eltern: A00002
repo:
status: Aktiv
herkunft: code
quellen:
  - Source/CsvViewer/CsvViewer.BL/DocumentAcquisition/
  - Source/CsvViewer/CsvViewer.BL/HostContracts/IFileReader.cs
verwandt: [A00004]
domaene: [D00001]
---

# Dokument-Beschaffung

## Verantwortung

Beschafft aus den Programmargumenten ein geprüftes CSV-Dokument.

## Schnittstellen

**Eingehend** — `string[] args` vom Composition Root.

**Ausgehend** — `Result<ViewerArguments>` und `Result<CsvDocument>`; im Fehlerfall eine Meldung statt eines Dokuments.

## Abhaengigkeiten

Innere Struktur — drei Schritte in fester Reihenfolge:

1. [Argument-Eingang](A00007-argument-eingang.md) — Pfad und Seitengröße prüfen
2. [Datei-Zugriff](A00008-datei-zugriff.md) — Zeilen von der Platte holen
3. [CSV-Interpretation](A00009-csv-interpretation.md) — Zeilen als Kopfzeile plus Datensätze deuten

Extern: Dateisystem (lesend). Weiter gereicht wird das Ergebnis an [Seiten-Darstellung](A00004-seiten-darstellung.md).

## Entscheidungen

Datei-Zugriff und CSV-Interpretation sind bewusst getrennt: Der Datei-Zugriff kennt kein CSV, die Interpretation kein Dateisystem. Begründet in [R00001](../../Anforderungen/R00001-csv-datei-einlesen.md).

## Offene Fragen

Keine.

## Notizen / Quellen

Fachlicher Begriff: [CSV-Dokument](../Domaenenwissen/D00001-csv-dokument.md) — dort stehen Format-Vorgaben und Validierungsregeln.
