---
id: R00002
title: "Seite als Tabelle rendern (Spaltenbreiten, Rahmen)"
type: Feature
status: Neu
created: 2026-07-01
---

# R00002: Seite als Tabelle rendern (Spaltenbreiten, Rahmen)

## Beschreibung

Ein Baustein rendert eine Seite — eine Kopfzeile plus eine Teilmenge von Datensätzen — als textuelle Tabelle. Für jede Spalte wird die Breite als längster Wert (inklusive Spaltenname) über genau die übergebenen Zeilen bestimmt. Werte werden linksbündig auf Spaltenbreite mit Leerzeichen aufgefüllt und je Spalte mit `|` abgeschlossen; zwischen Kopf- und Datenzeilen steht eine Trennlinie aus `-` je Spaltenbreite mit `+` an den Spaltenenden. Der Baustein ist eine pure Operation: Er liefert die fertige Tabelle als Zeichenkette zurück und gibt selbst nichts auf der Konsole aus. Horizontales Scrollen findet nicht statt — breite Tabellen laufen über die Konsolenbreite hinaus.

Dies ist die **zweite von drei Anforderungen** (vertikale Slices) für einen CSV-Viewer-CLI nach CCD-Kursaufgabe „CSV Viewer I". Übergeordnete Zusatzvorgabe für alle drei: saubere Umsetzung nach **SRP, DRY und IODA-Architektur** (Integration/Operation/Data-Trennung nach Westphal).

### Zielformat (zeichengenau nach Kursbeispiel)

```
Name |Age|City    |
-----+---+--------+
Peter|42 |New York|
Paul |57 |London  |
Mary |35 |Munich  |
```

## User Stories

Given/When/Then-Szenarien: siehe [user-stories/R00002.md](user-stories/R00002.md).

- **US1 — Lesbare Tabellenansicht**: Als Nutzer will ich eine Seite als ausgerichtete Tabelle mit Kopfzeile und Zellrändern sehen, damit ich die Datensätze schnell erfassen kann.
- **US2 — Spaltenbreite passt sich der Seite an**: Als Nutzer will ich, dass jede Spalte genau so breit ist wie ihr längster Wert auf der aktuellen Seite (inkl. Spaltenname), damit die Ausgabe kompakt und bündig ist.
- **US3 — Generisch für beliebige Spalten**: Als Entwickler will ich beliebige CSV-Strukturen rendern können, ohne den Renderer an ein Fachmodell zu binden.

## Akzeptanzkriterien

### Tabellenformat
- [ ] Kopfzeile wird als erste Ausgabezeile gerendert (Spaltennamen)
- [ ] Genau eine Trennlinie zwischen Kopf- und Datenzeilen: `-` je Spaltenbreite, `+` an jedem Spaltenende
- [ ] Jeder Datensatz wird als eine Zeile gerendert
- [ ] Jede Zelle endet mit `|` (kein führendes/äußeres `|`, keine obere/untere Rahmenlinie)
- [ ] Werte sind linksbündig, mit Leerzeichen rechts auf Spaltenbreite aufgefüllt, ohne zusätzliche Padding-Leerzeichen um den Inhalt

### Spaltenbreiten
- [ ] Spaltenbreite = längster Wert der Spalte inklusive Spaltenname
- [ ] Breite wird über genau die übergebenen (Seiten-)Zeilen berechnet, nicht über das Gesamtdokument
- [ ] Alle `|`-Trenner stehen spaltenweise bündig untereinander

### Generik & Sonderfälle
- [ ] Rendering arbeitet rein positionsbasiert über Header und Feldwerte (kein Fachmodell)
- [ ] Leere Seite (0 Datensätze) → Ausgabe besteht nur aus Kopfzeile + Trennlinie
- [ ] Leere Zellwerte werden als reine Auffüll-Leerzeichen dargestellt (Spaltenbreite bleibt erhalten)

### Qualität & Architektur
- [ ] Baustein ist eine pure Operation: liefert die Tabelle als String, gibt selbst nichts auf der Konsole aus
- [ ] Keine Konsolen-/UI-Abhängigkeit (reine BL)
- [ ] Unit-Tests decken Format, Breitenberechnung und Sonderfälle (leere Seite, leere Zellen, unterschiedliche Spaltenzahlen) ab

## Status

- [ ] Neu

## Technische Details

### Neue Dateien

| Datei | Rolle (IODA/SRP) | Beschreibung |
|-------|------------------|--------------|
| `Source/CsvViewer/CsvViewer.BL/Rendering/ITableRenderer.cs` | Operation-Vertrag | `string Render(CsvDocument page)` — rendert eine Seite als Tabellen-String. |
| `Source/CsvViewer/CsvViewer.BL/Rendering/TableRenderer.cs` | **Operation (pure)** | Berechnet Spaltenbreiten (längster Wert inkl. Header über die Seite), baut Kopfzeile, Trennlinie und Datenzeilen im Kursformat. Kein I/O, keine Konsole. |

### Datenmodell

| Typ | Herkunft | Zweck |
|-----|----------|-------|
| `CsvDocument` | R00001 (wiederverwendet) | Repräsentiert eine Seite (Header + Zeilen-Teilmenge). Kein neuer Data-Typ (DRY); R00003 erzeugt die Seiten-`CsvDocument`s per Slicing. |

### IODA/SRP-Zuordnung

| Schicht | Baustein | Eigenschaft |
|---------|----------|-------------|
| Data | `CsvDocument` (aus R00001) | nur Daten, wiederverwendet |
| Operation | `TableRenderer` | pure, seiteneffektfrei, gibt String zurück |
| Integration | — | Konsolenausgabe des Strings liegt in R00003 |

Spaltenbreiten-Berechnung als klar abgegrenzte private, pure Methode innerhalb `TableRenderer` (bei Bedarf später als eigene Operation extrahierbar; hier bewusst nicht überfragmentiert).

### Tests

| Testdatei | Prüft |
|-----------|-------|
| `Source/CsvViewer/CsvViewer.BL.Tests/Rendering/TableRendererTests.cs` | Exaktes Kursformat (zeichengenau), Breitenberechnung (längster Wert inkl. Header), bündige `|`, Trennlinie mit `+`, Sonderfälle: leere Seite (nur Kopf + Trennlinie), leere Zellwerte, unterschiedliche Spaltenzahlen |

## Abhängigkeiten

- Abhängig von: R00001 (nutzt `CsvDocument`)
- Blockiert: R00003 (Interaktiver Viewer)

## Out-of-Scope

- CSV-Einlesen, Parsing, Validierung → R00001
- Paging/Seiten-Slicing, Navigationsmenü (`F)irst …`), Tastatureingabe, Konsolenausgabe des Strings, CLI-Argument-Parsing, Composition Root → R00003
- Horizontales Scrollen / Umbruch breiter Tabellen — laut Kursvorgabe nicht gefordert
- Rechtsbündige/zahlenspezifische Ausrichtung — Kursformat ist durchgängig linksbündig

## Notizen

- Tabellenstil ist zeichengenau durch das Kursbeispiel festgelegt: kein Außenrahmen, Zelle = `Wert.PadRight(Breite) + "|"`, Trennlinie = `-`×Breite je Spalte + `+` am Spaltenende.
- Spaltenbreite bezieht die Kopfzeile mit ein und wird **pro Seite** neu berechnet (dieselbe Spalte kann auf verschiedenen Seiten unterschiedlich breit sein).
- `CsvDocument` wird als Seiten-Container wiederverwendet (DRY) — R00003 liefert pro Seite ein `CsvDocument` mit der Zeilen-Teilmenge.
- Renderer bleibt pure Operation → in Unit-Tests zeichengenau prüfbar ohne Konsole.

### Verworfene Alternativen

- **Vollrahmen mit `+`-Ecken bzw. oberer/unterer Rahmenlinie** — verworfen, entspricht nicht dem Kursbeispiel (kein Außenrahmen).
- **Nur Header-Trennlinie ohne `+`** — verworfen, Kursbeispiel nutzt `+` an den Spaltenenden.
- **Rendering direkt auf die Konsole** — verworfen zugunsten String-Rückgabe (Testbarkeit, IODA: I/O bleibt in R00003).
- **Eigener Seiten-Typ statt `CsvDocument`** — verworfen zugunsten Wiederverwendung (DRY).
- **Eigener `ColumnWidthCalculator`-Baustein** — vorerst verworfen (private pure Methode reicht), später extrahierbar, falls anderweitig gebraucht.
