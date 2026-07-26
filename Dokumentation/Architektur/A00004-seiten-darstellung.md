---
id: A00004
name: Seiten-Darstellung
ebene: Komponente
eltern: A00002
repo:
status: Aktiv
herkunft: code
quellen:
  - Source/CsvViewer/CsvViewer.BL/PagePresentation/
verwandt: [A00003, A00005]
domaene: [D00002, D00003]
---

# Seiten-Darstellung

## Verantwortung

Macht aus einem CSV-Dokument die sichtbare Seitenansicht.

## Schnittstellen

**Eingehend** — `CsvDocument` plus Seitengröße; später je Zeichenvorgang eine einzelne Seite.

**Ausgehend** — `Result<PagedDocument>` beim Aufteilen, `Result<string>` beim Rendern einer Seite.

## Abhaengigkeiten

Innere Struktur — zwei Schritte zu unterschiedlichen Zeitpunkten:

1. [Seiten-Aufteilung](A00010-seiten-aufteilung.md) — einmalig beim Start
2. [Tabellen-Rendering](A00011-tabellen-rendering.md) — bei jedem Zeichnen erneut

Bekommt das Dokument von [Dokument-Beschaffung](A00003-dokument-beschaffung.md), wird aufgerufen von [Bedienung](A00005-bedienung.md).

## Entscheidungen

Das Rendering gibt einen String zurück, statt selbst auf die Konsole zu schreiben — dadurch bleibt es eine pure Operation und zeichengenau testbar ([R00002](../../Anforderungen/R00002-seite-als-tabelle-rendern.md)).

## Offene Fragen

Keine.

## Notizen / Quellen

Fachliche Begriffe: [Seiten-Modell](../Domaenenwissen/D00002-seiten-modell.md), [Tabellen-Ausgabeformat](../Domaenenwissen/D00003-tabellen-ausgabeformat.md).

Nur zwei Kinder — hier ist bewusst Schluss, die Substanz reicht nicht für eine weitere Ebene.
