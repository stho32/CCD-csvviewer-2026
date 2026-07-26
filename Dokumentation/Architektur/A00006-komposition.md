---
id: A00006
name: Komposition
ebene: Komponente
eltern: A00002
repo:
status: Aktiv
herkunft: code
quellen:
  - Source/CsvViewer/CsvViewer/Program.cs
verwandt: [A00003, A00004, A00005]
domaene: []
---

# Komposition

## Verantwortung

Verdrahtet die Komponenten zu einem lauffähigen Programm.

## Schnittstellen

**Eingehend** — `Main(string[] args)`, der einzige Einstiegspunkt des Prozesses.

**Ausgehend** — Exit-Code 0 bei Erfolg, 1 bei jedem Fehler; Fehlermeldungen über den Logger auf stderr.

## Abhaengigkeiten

Hängt von allen drei fachlichen Komponenten ab — [Dokument-Beschaffung](A00003-dokument-beschaffung.md), [Seiten-Darstellung](A00004-seiten-darstellung.md), [Bedienung](A00005-bedienung.md) — sowie von den konkreten Adaptern `SystemConsole` und `ConsoleLogger`.

Diese Allabhängigkeit ist **kein** Gottbaustein-Verstoss: Genau das ist die Aufgabe eines Composition Root. Umgekehrt kennt ihn niemand.

## Entscheidungen

Fachliche Entscheidungen trifft dieser Baustein keine. Er kennt als einziger die konkreten Implementierungen und ist der einzige Ort, an dem ein `Result` in einen Exit-Code übersetzt wird.

## Offene Fragen

Keine.

## Notizen / Quellen

Ohne Kinder — 26 Zeilen reine Verdrahtung, eine Verfeinerung hätte keinen Erkenntniswert.
