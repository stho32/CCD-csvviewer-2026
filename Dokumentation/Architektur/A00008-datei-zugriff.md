---
id: A00008
name: Datei-Zugriff
code: FileReader
ebene: Komponente
eltern: A00003
repo:
status: Aktiv
herkunft: code
quellen:
  - Source/CsvViewer/CsvViewer.BL/HostContracts/IFileReader.cs
  - Source/CsvViewer/CsvViewer/HostSpecific/IO/FileReader.cs
verwandt: [A00009, A00014]
domaene: [D00001]
---

# Datei-Zugriff

## Verantwortung

Liest eine Textdatei zeilenweise als UTF-8 von der Platte.

## Schnittstellen

**Vertrag in der Geschäftslogik** — `IFileReader.ReadLines(string path)`.

**Umsetzung im Entry Point** — `FileReader` liest die Datei als UTF-8 über `System.IO`.

**Ausgehend** — `Result<IReadOnlyList<string>>`; fehlende, leer benannte oder nicht lesbare Dateien werden zu einer Fehlermeldung statt zu einer Exception.

## Abhaengigkeiten

Dateisystem (lesend). Sonst keine.

Dieser Baustein kennt **kein** CSV — kein Semikolon, keine Kopfzeile. Die Deutung übernimmt [CSV-Interpretation](A00009-csv-interpretation.md).

**Abhängigkeitsrichtung** — der Vertrag liegt in der Geschäftslogik, die Umsetzung im Entry-Point-Projekt. Damit folgt der Datei-Zugriff demselben Muster wie [Konsolen-Anbindung](A00014-konsolen-anbindung.md): Jeder Berührungspunkt mit der Aussenwelt ist ein Host-Adapter, die Geschäftslogik bleibt frei von `System.IO` und `System.Console`.

## Entscheidungen

Bewusst CSV-agnostisch gehalten, damit er unabhängig vom Format wiederverwendbar bleibt ([R00001](../../Anforderungen/R00001-csv-datei-einlesen.md)).

Die Implementierung wurde mit `ed2fa03` [R00004] aus der Geschäftslogik in den Host verschoben. `IFileReader` ist damit die verbleibende Abstraktion, nicht ein Überbleibsel — dieselbe Rolle, die `IConsole` und `ILogger` spielen.

## Offene Fragen

Keine.

## Notizen / Quellen

**Fachlich vs. physisch** — fachlich gehört dieser Baustein zur [Dokument-Beschaffung](A00003-dokument-beschaffung.md), weil er den ersten Schritt im Datenfluss liefert. Physisch liegt seine Umsetzung beim Host, zusammen mit den anderen Adaptern. Beides ist richtig und widerspricht sich nicht: Der Datenfluss bestimmt die Zugehörigkeit im Modell, die Wirkungsart den Ablageort im Code.

Nach der Verschiebung enthält `CsvViewer.BL/IO/` nur noch Verträge (`IConsole`, `IFileReader`) — der Ordner ist dadurch kohärenter als zuvor.
