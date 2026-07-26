---
id: A00005
name: Bedienung
code: Interaction/
ebene: Komponente
eltern: A00002
repo:
status: Aktiv
herkunft: code
quellen:
  - Source/CsvViewer/CsvViewer.BL/Interaction/
  - Source/CsvViewer/CsvViewer.BL/HostContracts/IConsole.cs
  - Source/CsvViewer/CsvViewer/HostSpecific/
verwandt: [A00011]
domaene: [D00004]
---

# Bedienung

## Verantwortung

Lässt den Anwender durch die Seiten blättern.

## Schnittstellen

**Eingehend** — `PagedDocument` vom Composition Root; Tastendrücke vom Terminal.

**Ausgehend** — gezeichnete Seiten plus Menüzeile auf die Konsole; `Result` als Gesamtergebnis des Ablaufs.

## Abhaengigkeiten

Innere Struktur — drei Rollen:

1. [Navigations-Steuerung](A00012-navigations-steuerung.md) — entscheidet, welche Seite als nächstes dran ist
2. [Viewer-Ablauf](A00013-viewer-ablauf.md) — treibt den Zyklus
3. [Konsolen-Anbindung](A00014-konsolen-anbindung.md) — der Draht zum echten Terminal

Ruft [Seiten-Aufteilung](A00010-seiten-aufteilung.md) zum Zeichnen auf.

## Entscheidungen

Der Ablauf liegt in der Geschäftslogik, nicht im Entry Point — nur so ist er mit einer Test-Konsole ohne echtes Terminal prüfbar ([R00003](../../Anforderungen/R00003-interaktiver-viewer.md)).

## Offene Fragen

Keine.

## Notizen / Quellen

Fachlicher Begriff: [Navigationsmenü](../Domaenenwissen/D00004-navigationsmenue.md) — Wortlaut, Tastenbelegung, Klemmverhalten.
