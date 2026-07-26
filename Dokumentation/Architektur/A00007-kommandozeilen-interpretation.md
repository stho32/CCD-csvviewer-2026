---
id: A00007
name: Kommandozeilen-Interpretation
ebene: Komponente
eltern: A00002
repo:
status: Aktiv
herkunft: code
quellen:
  - Source/CsvViewer/CsvViewer.BL/CommandLineInterpretation/
verwandt: [A00010]
domaene: [D00002]
---

# Kommandozeilen-Interpretation

## Verantwortung

Übersetzt die Prozessargumente in validierte Viewer-Parameter.

## Schnittstellen

**Eingehend** — `ArgumentsParser.Parse(string[] args)`.

**Ausgehend** — `Result<ViewerArguments>` mit Dateipfad und Seitengröße; bei Verstoss eine Usage- oder Validierungsmeldung.

Regeln: erstes Argument Pfad (erforderlich), zweites Seitengröße (optional, Default 10). Mehr als zwei Argumente, nicht-numerische oder Werte ≤ 0 führen zum Abbruch — es wird ausdrücklich nicht stillschweigend auf den Default zurückgefallen.

## Abhaengigkeiten

Keine. Pure Operation ohne I/O — `string[]` rein, `Result<ViewerArguments>` raus.

## Entscheidungen

Positionale Argumente ohne Parser-Library. Eine Flag-CLI (`--file`) wurde in [R00003](../../Anforderungen/R00003-interaktiver-viewer.md) ausdrücklich verworfen, das Paket `CommandLineParser` entfernt.

## Offene Fragen

Keine.

## Notizen / Quellen

Die Regel „Seitengröße zählt nur Datenzeilen" steht im [Seiten-Modell](../Domaenenwissen/D00002-seiten-modell.md).
