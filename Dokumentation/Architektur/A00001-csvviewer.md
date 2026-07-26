---
id: A00001
name: CsvViewer
code: CsvViewer.sln
ebene: System
eltern:
repo:
status: Aktiv
herkunft: code
quellen:
  - Source/CsvViewer/CsvViewer.sln
  - README.md
  - Anforderungen/
verwandt: [A00002]
domaene: [D00001, D00002, D00003, D00004]
---

# CsvViewer

## Verantwortung

Zeigt eine semikolon-getrennte CSV-Datei seitenweise als ausgerichtete Texttabelle im Terminal an.

## Schnittstellen

**Eingehend**

| Von | Wie |
|---|---|
| Anwender | zwei positionale CLI-Argumente: Dateipfad (erforderlich), Seitengröße (optional, Default 10) |
| Anwender | einzelne Tastendrücke am Terminal; bei umgeleiteter Eingabe ein Zeichen je Tastendruck aus dem Strom |
| Dateisystem | eine UTF-8-kodierte CSV-Datei mit `;` als Feldtrenner, lesend |

**Ausgehend**

| Nach | Wie |
|---|---|
| Terminal | die aktuelle Seite als Texttabelle plus Menüzeile, stdout |
| Terminal | Fehlermeldungen mit Präfix `[ERROR]`, stderr |
| Aufrufer | Exit-Code 0 bei regulärem Beenden, ≠ 0 bei jedem Fehler |

Es gibt keine Netzwerk-Schnittstelle, keine Datenbank, keine Konfigurationsdatei und kein Fremdsystem.

## Abhaengigkeiten

- [CsvViewer CLI](A00002-csvviewer-cli.md) — der einzige Container dieses Systems.
- Dateisystem (lesend) — einzige externe Ressource.
- Terminal — Ein- und Ausgabe.

## Entscheidungen

Es existieren keine ADRs unter `Dokumentation/ADRs/`. Die tragenden Entscheidungen samt verworfener Alternativen sind stattdessen in den Anforderungen festgehalten:

- Positionale Argumente statt Flag-CLI, `CommandLineParser`-Library verworfen — [R00003](../../Anforderungen/R00003-interaktiver-viewer.md)
- Generisches Datenmodell ohne Fachklassen — [R00001](../../Anforderungen/R00001-csv-datei-einlesen.md)
- Rendering als pure Operation mit String-Rückgabe statt direkter Konsolenausgabe — [R00002](../../Anforderungen/R00002-seite-als-tabelle-rendern.md)

## Offene Fragen

- Die Wiederverwendbarkeit der Geschäftslogik (`FileReader` ist bewusst CSV-agnostisch, R00001) ist **kein** verbindliches Architekturziel — ein Folgekurs „CSV Viewer II" ist nicht geplant, aber auch nicht ausgeschlossen. Falls er kommt, ist neu zu bewerten, ob die BL als eigenständig konsumierbare Bibliothek gepflegt werden soll.

## Notizen / Quellen

**Randbedingung Teststrategie** — die beiden Testprojekte sind bewusst *keine* eigenen Bausteine, da sie kein Deployment-Artefakt des Systems sind. Als Randbedingung des Systems gilt:

- Drei Ebenen: Unit (`CsvViewer.BL.Tests`), Integration und E2E (`CsvViewer.BL.IntegrationTests`).
- Hand-geschriebene Mocks statt Mocking-Framework (`Mocks/TestConsole.cs`, `Mocks/FailingTableRenderer.cs`).
- Stand zuletzt: 92 Tests, 93,2 % Line Coverage über beide Ebenen.
- Ein Test ist plattformbedingt übersprungen (Unix-Dateirechte unter Windows nicht prüfbar).

**Fachliches Vokabular** — die Begriffe des Systems sind separat modelliert: [CSV-Dokument](../Domaenenwissen/D00001-csv-dokument.md), [Seiten-Modell](../Domaenenwissen/D00002-seiten-modell.md), [Tabellen-Ausgabeformat](../Domaenenwissen/D00003-tabellen-ausgabeformat.md), [Navigationsmenü](../Domaenenwissen/D00004-navigationsmenue.md).

**Aus dem Dialog bestätigt** (`herkunft: dialog`, 2026-07-26):

- „CSV Viewer II" ist derzeit nicht geplant, kann aber noch kommen.
- Das Werkzeug wird nicht verteilt, sondern nur über `dotnet run` gestartet.
- Die Testprojekte sollen nicht als Bausteine im Modell erscheinen.

**Herkunft im Übrigen** — Zweck, Schnittstellen und Struktur sind aus `README.md`, den Anforderungen und dem Quellcode belegt und dort nachprüfbar.
