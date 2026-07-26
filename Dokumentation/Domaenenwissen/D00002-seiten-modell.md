---
id: D00002
name: Seiten-Modell
status: Aktiv
sprache: de
verwandt: [D00001, D00003, D00004]
---

# Seiten-Modell

## Definition

Die Aufteilung der Datensätze eines [CSV-Dokuments](D00001-csv-dokument.md) in Seiten fester Größe für die blätterbare Anzeige. Eine Seite ist eine zusammenhängende Teilmenge der Datensätze in Leserichtung; die Kopfzeile existiert genau einmal für das gesamte Dokument und wird auf jeder Seite mitgerendert.

Die Seiten werden einmalig vorberechnet, nicht bei jedem Blättern neu geschnitten.

## Bestandteile

**Seitengröße** — positive Ganzzahl, Default 10.

> **Zählweise:** Die Seitengröße zählt ausschließlich **Datenzeilen**. Kopfzeile, Trennlinie und Menüzeile kommen obendrauf. Eine Seite bei Größe 10 belegt also 12 Konsolenzeilen.

Diese Zählweise ist die am leichtesten zu übersehende Regel des Modells und der Hauptgrund für diesen Eintrag — sie stand bisher nur als Randnotiz in R00001.

**Seiten-Aufteilung:**

- Seitenzahl = ⌈Datensätze / Seitengröße⌉, aber **mindestens 1**.
- Die letzte Seite ist gegebenenfalls kürzer als die Seitengröße.
- Bei 0 Datensätzen (Datei enthält nur die Kopfzeile) entsteht genau **eine** Seite; angezeigt werden Kopfzeile und Trennlinie.

**Ungültige Seitengröße** — nicht numerisch oder ≤ 0 führt zum Abbruch mit Meldung und Exit-Code ≠ 0. Es wird ausdrücklich **nicht** stillschweigend auf den Default 10 zurückgefallen.

**Herkunft der Seitengröße** — zweites, optionales Kommandozeilenargument: `csvviewer datei.csv [seitengröße]`. Fehlt es, gilt der Default 10.

## Beziehungen

- [CSV-Dokument](D00001-csv-dokument.md) — liefert die Datensätze, die aufgeteilt werden.
- [Tabellen-Ausgabeformat](D00003-tabellen-ausgabeformat.md) — berechnet die Spaltenbreiten **pro Seite** neu; dieselbe Spalte kann auf verschiedenen Seiten unterschiedlich breit sein.
- [Navigationsmenü](D00004-navigationsmenue.md) — bewegt den aktuellen Seitenindex innerhalb der berechneten Seitenzahl und klemmt an deren Rändern.

## Vorkommen in Anforderungen

- [R00003](../../Anforderungen/R00003-interaktiver-viewer.md) — führend: Seitengröße, Aufteilung, Grenzfälle, Argument-Validierung.
- [R00001](../../Anforderungen/R00001-csv-datei-einlesen.md) — nennt die Zählweise (Default 10 Datenzeilen, Menü und Rahmen extra) als kursweite Kontextvorgabe.
- [R00002](../../Anforderungen/R00002-seite-als-tabelle-rendern.md) — Spaltenbreiten werden über genau die Zeilen einer Seite berechnet, nicht über das Gesamtdokument.

## Notizen / Quellen

- Default 10 ist Vorgabe der CCD-Kursaufgabe „CSV Viewer I".
- Die Seiten sind Teilmengen der Datensatzliste, kein jeweils eigenes CSV-Dokument — die Kopfzeile wird nicht pro Seite dupliziert.
