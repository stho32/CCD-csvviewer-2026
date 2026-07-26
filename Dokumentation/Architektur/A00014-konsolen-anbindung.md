---
id: A00014
name: Konsolen-Anbindung
code: SystemConsole
ebene: Komponente
eltern: A00005
repo:
status: Aktiv
herkunft: code
quellen:
  - Source/CsvViewer/CsvViewer.BL/HostContracts/IConsole.cs
  - Source/CsvViewer/CsvViewer.BL/HostContracts/ILogger.cs
  - Source/CsvViewer/CsvViewer/HostSpecific/
verwandt: [A00013, A00008]
domaene: []
---

# Konsolen-Anbindung

## Verantwortung

Verbindet den Viewer mit dem echten Terminal.

## Schnittstellen

**Verträge in der Geschäftslogik** — `IConsole` mit `Clear()`, `Write(string)`, `ReadKey()`; `ILogger` mit `Error(string)`.

**Umsetzungen im Entry Point** — `SystemConsole` auf `System.Console`, `ConsoleLogger` schreibt `[ERROR] …` auf stderr.

Verhalten bei umgeleiteten Kanälen:

| Kanal umgeleitet | Verhalten |
|---|---|
| Eingabe | `ReadKey()` liest zeichenweise aus dem Strom statt vom Terminal; endet der Strom ohne `E`, wird der Abbruch gemeldet |
| Ausgabe | `Clear()` wird zum No-op — es gibt keinen Bildschirm zu leeren |

Dadurch ist der Viewer per `printf 'nne' | csvviewer datei.csv` steuerbar.

## Abhaengigkeiten

Terminal beziehungsweise die Standard-Datenströme. Wird ausschliesslich über die Verträge benutzt, nie über die konkreten Typen — Ausnahme ist [Komposition](A00006-komposition.md), die sie als einzige instanziiert.

**Abhängigkeitsrichtung** — die Verträge liegen in der Geschäftslogik, die Umsetzungen im Entry-Point-Projekt. Damit zeigt die Abhängigkeit nach innen zur Fachlichkeit, und die Geschäftslogik enthält keine einzige direkte `System.Console`-Nutzung.

## Entscheidungen

Die Adapter liegen unter `HostSpecific/`, damit host-spezifische Eigenheiten nicht in die Geschäftslogik sickern.

Der Terminal-Pfad von `ReadKey()` bleibt bewusst ohne automatisierten Test. Die E2E-Tests fahren ausschliesslich über den Strom-Pfad; der Terminal-Zweig ist eine einzelne Abfrage (`Console.IsInputRedirected`) und wird bei jedem manuellen Start über `testlauf.sh` durchlaufen. Ein Pseudo-Terminal im Test (ConPTY-Interop) wäre rund 150 Zeilen P/Invoke und damit selbst fehleranfällige Infrastruktur — der Aufwand steht nicht im Verhältnis zum abgedeckten Risiko.

## Offene Fragen

Keine.

## Notizen / Quellen

`IConsole` liegt im Ordner `IO/`, seit `ed2fa03` [R00004] zusammen mit dem ebenfalls dorthin gehörenden Vertrag `IFileReader` — der Ordner enthält damit nur noch Verträge, die Umsetzungen liegen geschlossen unter `HostSpecific/`. Derselbe Adapter-Schnitt wie bei [Datei-Zugriff](A00008-datei-zugriff.md).
