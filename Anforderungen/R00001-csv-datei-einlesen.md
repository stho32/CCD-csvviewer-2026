---
id: R00001
title: "CSV-Datei einlesen (generisches Parsing + Datenmodell)"
type: Feature
status: Neu
created: 2026-07-01
---

# R00001: CSV-Datei einlesen (generisches Parsing + Datenmodell)

## Beschreibung

Ein Baustein liest eine UTF-8-kodierte CSV-Datei mit Semikolon (`;`) als Feldtrenner ein und stellt ihren Inhalt als generische, fachlich neutrale Struktur bereit: eine Kopfzeile (Spaltennamen) plus eine geordnete Liste von Datensätzen, wobei jeder Datensatz seine Feldwerte in Kopfzeilen-Reihenfolge hält. Es gibt keine Bindung an ein konkretes Datenmodell (keine `Person`-Klasse o.ä.). Zellinhalte werden unverändert übernommen — kein Quoting, keine Escapes, keine Zeilenumbrüche innerhalb von Feldern. Beim Einlesen wird strukturell validiert; Verstöße führen zu einem klar gemeldeten Abbruch über das Result-Muster.

Dies ist die **erste von drei Anforderungen** (vertikale Slices) für einen CSV-Viewer-CLI nach CCD-Kursaufgabe „CSV Viewer I". Übergeordnete Zusatzvorgabe für alle drei: saubere Umsetzung nach **SRP, DRY und IODA-Architektur** (Integration/Operation/Data-Trennung nach Westphal).

## User Stories

Given/When/Then-Szenarien: siehe [user-stories/R00001.md](user-stories/R00001.md).

- **US1 — Generisches Einlesen**: Als Nutzer will ich eine beliebige `;`-getrennte CSV-Datei einlesen, damit ihr Inhalt unabhängig von den konkreten Spalten weiterverarbeitet werden kann.
- **US2 — Fachlich neutrale Struktur**: Als Entwickler will ich die Daten als Header + geordnete Datensätze (Felder in Kopfzeilen-Reihenfolge) erhalten, damit Rendering und Paging ohne Kenntnis eines Fachmodells arbeiten können.
- **US3 — Robustheit bei fehlerhaften Dateien**: Als Nutzer will ich bei strukturell fehlerhaften Dateien eine klare Meldung statt undefinierten Verhaltens.

## Akzeptanzkriterien

### Einlesen & Format
- [ ] Datei wird als UTF-8 gelesen
- [ ] Felder werden am Semikolon `;` getrennt
- [ ] Erste Zeile wird als Kopfzeile (Spaltennamen) interpretiert
- [ ] Jede weitere Zeile wird als ein Datensatz interpretiert
- [ ] Zellinhalte werden unverändert übernommen (kein Quoting/Escaping/Zeilenumbruch-Handling)

### Generische Struktur
- [ ] Ergebnis besteht aus Kopfzeile + geordneter Datensatzliste
- [ ] Datensätze behalten die Eingabereihenfolge
- [ ] Feldwerte sind positionsbasiert in Kopfzeilen-Reihenfolge zugreifbar
- [ ] Keine Bindung an ein konkretes Fachmodell (keine typgebundene Zeilenklasse)

### Validierung & Fehlerfälle
- [ ] Fehlende/nicht lesbare Datei → Abbruch mit Meldung, kein Weiterverarbeiten
- [ ] Komplett leere Datei → Abbruch mit Meldung
- [ ] Nur Kopfzeile (keine Datensätze) → gültiges Ergebnis mit 0 Datensätzen
- [ ] Datenzeile mit abweichender Feldanzahl zur Kopfzeile → Abbruch mit Meldung
- [ ] Fehler werden über das Result-Muster signalisiert (keine Exceptions über Bausteingrenzen)

### Qualität
- [ ] Baustein hat keine Konsolen-/UI-Abhängigkeit (reine BL)
- [ ] Unit-Tests decken alle Format- und Fehlerfälle ab

## Status

- [ ] Neu

## Technische Details

### Neue Dateien

| Datei | Rolle (IODA/SRP) | Beschreibung |
|-------|------------------|--------------|
| `Source/CsvViewer/CsvViewer.BL/Csv/CsvDocument.cs` | **Data** | Generisches, immutables `record`: `Header` + geordnete `Rows` (positionsbasierte Feldwerte). Keine Logik. |
| `Source/CsvViewer/CsvViewer.BL/IO/IFileReader.cs` | I/O-Schnittstelle | CSV-agnostischer Datei-Lesevertrag: Pfad → `Result<IReadOnlyList<string>>`. |
| `Source/CsvViewer/CsvViewer.BL/IO/FileReader.cs` | **I/O-Randbaustein** | Liest Pfad als UTF-8, liefert Zeilen. Kennt kein CSV. Failt bei fehlender/nicht lesbarer Datei. |
| `Source/CsvViewer/CsvViewer.BL/Csv/CsvParser.cs` | **Operation (pure)** | `IReadOnlyList<string> → Result<CsvDocument>`: Split am `;`, Header-Erkennung, Validierung. Kein I/O. |

### Wiederverwendet

| Datei | Zweck |
|-------|-------|
| `Source/CsvViewer/CsvViewer.BL/Common/Result.cs` | `Result<T>` für Fehlersignalisierung ohne Exceptions über Bausteingrenzen |

### IODA/SRP-Zuordnung

| Schicht | Baustein | Eigenschaft |
|---------|----------|-------------|
| Data | `CsvDocument` | nur Daten |
| Operation | `CsvParser` | pure, seiteneffektfrei |
| I/O-Rand | `FileReader` | nur Dateizugriff, kein CSV-Wissen |
| Integration | — | **bewusst nicht in R1**; Verdrahtung `FileReader` → `CsvParser` liegt in R3 (Composition Root in `Program`) |

### Fehler-Zuständigkeit

| Fehlerfall | Zuständiger Baustein |
|------------|----------------------|
| Datei fehlt / nicht lesbar | `FileReader` (Fehler-`Result`) |
| Komplett leere Datei | `CsvParser` (Fehler-`Result`) |
| Nur Kopfzeile | `CsvParser` (Erfolg, 0 Datensätze) |
| Abweichende Feldanzahl | `CsvParser` (Fehler-`Result`) |

### Tests

| Testdatei | Prüft |
|-----------|-------|
| `Source/CsvViewer/CsvViewer.BL.Tests/Csv/CsvParserTests.cs` | Alle Format- und Validierungsfälle rein in-memory (Zeilen → Dokument) |
| `Source/CsvViewer/CsvViewer.BL.IntegrationTests/IO/FileReaderIntegrationTests.cs` | Echte Temp-Dateien: vorhanden, fehlend, leer |

## Abhängigkeiten

- Abhängig von: —
- Blockiert: R00002 (Rendering), R00003 (Interaktiver Viewer)

## Out-of-Scope

- Tabellen-Rendering, Spaltenbreiten, Rahmen → R00002
- Paging, interaktive Navigation, CLI-Argument-Parsing, Composition Root → R00003
- Quoting/Escaping, eingebettete Zeilenumbrüche, andere Delimiter als `;` — laut Kursvorgabe nicht gefordert
- Das Zusammenschalten von Lesen + Parsen (Integration) — liegt in R00003

## Notizen

- Delimiter `;`, UTF-8 und „erste Zeile = Header" sind feste Kursvorgaben.
- `FileReader` bewusst CSV-agnostisch → in R00002/R00003 und Folgekursen wiederverwendbar; die Integration liegt eine Ebene höher (R00003).
- Header zählt bei Spaltenbreiten mit — relevant erst in R00002, hier nur Kontext.
- Kursweite Kontext-Vorgaben (betreffen R00002/R00003): Standard-Seitengröße 10 Datensätze (nur Datenzeilen, Menü/Rahmen extra), Menü `F)irst P)revious N)ext L)ast E)xit`, Navigation klemmt an den Rändern (kein Umlauf), breite Tabellen laufen ohne horizontales Scrollen über die Konsolenbreite.

### Verworfene Alternativen

- **Zuschnitt A** (nach technischem Concern, 4 Anforderungen) — zu feingliedrig, R3/R4 zu klein.
- **Zuschnitt C** (grob, 2 Anforderungen) — vermischt Einlesen und Rendering, widerspricht SRP/IODA-Fokus.
- **`CsvReader` als kombinierter I/O+Parse-Baustein** — verworfen zugunsten getrenntem `FileReader` (I/O) + `CsvParser` (pure Operation); Integration wandert nach R00003.
- **Options-Flag-CLI (`--file`/`--delimiter`)** — verworfen zugunsten positionaler Argumente exakt nach Kursvorgabe (betrifft R00003).
- **Typgebundenes Zeilenmodell (z.B. `Person`)** — verworfen, da CSV generisch bleiben muss.
