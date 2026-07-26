---
id: A00006
name: Komposition
code: Program.cs
ebene: Komponente
eltern: A00002
repo:
status: Aktiv
herkunft: code
quellen:
  - Source/CsvViewer/CsvViewer/Program.cs
verwandt: [A00003, A00005, A00007, A00010, A00011]
domaene: []
---

# Komposition

## Verantwortung

Verdrahtet die Komponenten zu einem lauffähigen Programm.

## Schnittstellen

**Eingehend** — `Main(string[] args)`, der einzige Einstiegspunkt des Prozesses.

**Ausgehend** — Exit-Code 0 bei Erfolg, 1 bei jedem Fehler; Fehlermeldungen über den Logger auf stderr.

## Abhaengigkeiten

Hängt von allen fachlichen Komponenten ab — [Kommandozeilen-Interpretation](A00007-kommandozeilen-interpretation.md), [Dokument-Beschaffung](A00003-dokument-beschaffung.md), [Seiten-Aufteilung](A00010-seiten-aufteilung.md), [Tabellen-Rendering](A00011-tabellen-rendering.md), [Bedienung](A00005-bedienung.md) — sowie von den konkreten Adaptern `SystemConsole` und `ConsoleLogger`.

Diese Allabhängigkeit ist **kein** Gottbaustein-Verstoss: Genau das ist die Aufgabe eines Composition Root. Umgekehrt kennt ihn niemand.

## Entscheidungen

Fachliche Entscheidungen trifft dieser Baustein keine. Er kennt als einziger die konkreten Implementierungen und ist der einzige Ort, an dem ein `Result` in einen Exit-Code übersetzt wird.

## Offene Fragen

Keine.

## Notizen / Quellen

Ohne Kinder — 26 Zeilen reine Verdrahtung, eine Verfeinerung hätte keinen Erkenntniswert.
