# Architektur-Karte — CsvViewer

> **Generiert.** Diese Datei ist eine Projektion des Architektur-Modells unter
> `Dokumentation/Architektur/` und wird von `/architektur karte` neu geschrieben.
> Änderungen hier gehen verloren — pflege stattdessen den jeweiligen Baustein.

## Vogelperspektive

CsvViewer zeigt eine semikolon-getrennte CSV-Datei seitenweise als ausgerichtete Texttabelle im Terminal an. Der Anwender übergibt beim Start einen Dateipfad und optional eine Seitengröße und blättert dann mit einzelnen Tastendrücken durch die Datensätze.

Das System ist ein einzelner Konsolenprozess ohne Netzwerk, ohne Datenbank und ohne Konfigurationsdatei. Die einzige externe Ressource ist die CSV-Datei selbst, die einzige Schnittstelle nach außen das Terminal.

Innen ist es nach Wirkungsart geschichtet: reine Operationen rechnen, Adapter berühren die Außenwelt, und ein Composition Root verdrahtet beides. Fehler wandern als `Result`-Objekte nach oben statt als Exceptions — nur `Program` übersetzt sie in Exit-Codes.

14 Bausteine, davon einer veraltet. Der Linktitel ist der Name im Code, die Folgezeile nennt die fachliche Bezeichnung.

## 1. `CsvViewer/`

CsvViewer CLI — Stellt den CSV-Viewer als einzeln startbaren Konsolenprozess bereit.

Der Schnitt folgt dem Weg der Daten: verstehen was verlangt wurde, das Dokument beschaffen, es portionieren, eine Portion als Text formen, den Anwender blättern lassen. Die Verdrahtung steht daneben statt darüber.

Besteht aus:

- **1.1** [`CommandLineInterpretation/`](Dokumentation/Architektur/A00007-kommandozeilen-interpretation.md)
  Kommandozeilen-Interpretation — Übersetzt die Prozessargumente in validierte Viewer-Parameter.
- **1.2** [`DocumentAcquisition/`](Dokumentation/Architektur/A00003-dokument-beschaffung.md)
  Dokument-Beschaffung — Macht aus einem Dateipfad ein geprüftes CSV-Dokument.
- **1.3** [`Pagination/`](Dokumentation/Architektur/A00010-seiten-aufteilung.md)
  Seiten-Aufteilung — Zerlegt die Datensätze eines CSV-Dokuments in Seiten fester Größe.
- **1.4** [`TableRendering/`](Dokumentation/Architektur/A00011-tabellen-rendering.md)
  Tabellen-Rendering — Formt eine Seite in eine ausgerichtete Texttabelle.
- **1.5** [`Interaction/`](Dokumentation/Architektur/A00005-bedienung.md)
  Bedienung — Lässt den Anwender durch die Seiten blättern.
- **1.6** [`Program.cs`](Dokumentation/Architektur/A00006-komposition.md)
  Komposition — Verdrahtet die Komponenten zu einem lauffähigen Programm.

Zwei dieser Schnitte sind Korrekturen aus dem Modellieren selbst: **1.1** stand ursprünglich unter **1.2**, obwohl die Seitengröße gar nicht dorthin fließt. Und **1.3** und **1.4** lagen in einem gemeinsamen Ordner „Seiten-Darstellung", obwohl sie einander nicht kennen — der Renderer nimmt nicht einmal das Ergebnis der Aufteilung entgegen.

## 1.2 `DocumentAcquisition/`

Dokument-Beschaffung — Macht aus einem Dateipfad ein geprüftes CSV-Dokument.

Datei-Zugriff und Interpretation sind bewusst getrennt: Der eine kennt kein CSV, der andere kein Dateisystem. Dadurch ist das Lesen formatunabhängig wiederverwendbar und das Parsen ohne Datei testbar.

Besteht aus:

- **1.2.1** [`FileReader`](Dokumentation/Architektur/A00008-datei-zugriff.md)
  Datei-Zugriff — Liest eine Textdatei zeilenweise als UTF-8 von der Platte.
- **1.2.2** [`CsvParser`](Dokumentation/Architektur/A00009-csv-interpretation.md)
  CSV-Interpretation — Interpretiert Textzeilen als Kopfzeile mit zugehörigen Datensätzen.

## 1.5 `Interaction/`

Bedienung — Lässt den Anwender durch die Seiten blättern.

Der Ablauf liegt in der Geschäftslogik, nicht im Entry Point — nur so ist der Zyklus mit einer Test-Konsole ohne echtes Terminal prüfbar. Die echte Konsole erreicht ihn ausschließlich über den Vertrag `IConsole`.

Besteht aus:

- **1.5.1** [`PageNavigator`](Dokumentation/Architektur/A00012-navigations-steuerung.md)
  Navigations-Steuerung — Bestimmt aus einem Tastendruck den nächsten Seitenindex.
- **1.5.2** [`InteractiveViewer`](Dokumentation/Architektur/A00013-viewer-ablauf.md)
  Viewer-Ablauf — Hält den interaktiven Zyklus am Laufen, bis der Anwender beendet.
- **1.5.3** [`SystemConsole`](Dokumentation/Architektur/A00014-konsolen-anbindung.md)
  Konsolen-Anbindung — Verbindet den Viewer mit dem echten Terminal.

## Kontext

Der Viewer ist kein Dienst, der auf Anfragen wartet, sondern ein Programm mit Anfang und Ende. Deshalb sind Start und Ende eigene Ereignisse — dazwischen liegt ein Zyklus, der beliebig oft durchlaufen wird oder gar nicht.

```mermaid
flowchart TB
  Start(["Start<br/><i>csvviewer datei.csv [seitengröße]</i>"])
  Anwender(["Anwender am Terminal"])
  FS[("Dateisystem")]
  Ende(["Ende<br/><i>Exit-Code</i>"])

  subgraph S["CsvViewer"]
    App["CsvViewer/<br/><i>.NET 10, Konsolenprozess</i>"]
  end

  Start -- "1 · Dateipfad, Seitengröße" --> App
  App -- "2 · liest CSV, einmalig" --> FS
  App -- "3 · Tabelle + Menü zeichnen" --> Anwender
  Anwender -- "4 · Tastendruck" --> App
  App -- "5a · nach E) — Exit 0" --> Ende
  App -- "5b · Argument- oder Dateifehler — Exit ≠ 0" --> Ende
```

Schritt 3 und 4 bilden den Zyklus: Nach jedem Tastendruck wird die Konsole geleert und neu gezeichnet. Bei **5b** wird er nie betreten — ungültige Argumente oder eine fehlerhafte Datei beenden das Programm, bevor die erste Tabelle erscheint.

## Datenfluss

Der Ablauf zerfällt in zwei Phasen mit unterschiedlichem Takt: Die Schritte 1–4 laufen **genau einmal** beim Start, die Schritte 5 und 6 wiederholen sich bei **jedem Tastendruck**. Jeder Startschritt kann mit einem Fehler-`Result` abbrechen — dann endet das Programm, ohne dass der Zyklus je beginnt.

```mermaid
flowchart TB
  Start(["Start"])
  Prog["Program.cs<br/><i>Composition Root</i>"]
  CLI["CommandLineInterpretation/"]
  DOC["DocumentAcquisition/"]
  PAG["Pagination/"]
  REN["TableRendering/"]
  INT["Interaction/"]
  Ende(["Ende<br/><i>Exit-Code</i>"])

  Start -- "args[]" --> Prog
  Prog -- "1 · Argumente" --> CLI
  CLI -- "2 · Dateipfad" --> DOC
  CLI -- "3 · Seitengröße" --> PAG
  DOC -- "4 · CSV-Dokument" --> PAG
  PAG -- "5 · aufgeteilte Seiten" --> INT
  INT -- "6 · Seite rendern, je Tastendruck" --> REN
  REN -- "7 · Tabellentext" --> INT
  INT -- "8 · Ergebnis des Ablaufs" --> Prog
  Prog -- "Exit 0 oder ≠ 0" --> Ende

  CLI -. "Fehler-Result" .-> Prog
  DOC -. "Fehler-Result" .-> Prog
  PAG -. "Fehler-Result" .-> Prog
```

| Phase | Schritte | Takt |
|---|---|---|
| Start | 1–5 | einmalig, streng nacheinander |
| Betrieb | 6–7 | je Tastendruck, beliebig oft — oder nie |
| Ende | 8 | einmalig |

Die Trennung von `Pagination/` und `TableRendering/` wird hier sichtbar: Die Aufteilung läuft in Schritt 5 genau einmal, das Rendern in Schritt 6 bei jedem Zeichenvorgang. Daher kann dieselbe Spalte auf verschiedenen Seiten unterschiedlich breit sein.

Die gestrichelten Kanten sind kein eigener Kontrollfluss, sondern das `Result`-Muster: Jeder Schritt gibt Erfolg oder Fehler an den Composition Root zurück, und nur dieser übersetzt ihn in einen Exit-Code.

## Ablage-Regeln

Es gibt zwei Sorten von Ordnern, unterschieden an einem nachzählbaren Merkmal — der Zahl der **Einstiegspunkte**, also der Typen, deren Methoden von außerhalb des Ordners gerufen werden. Datentypen zählen nicht mit.

| Sorte | Einstiegspunkte | Inhalt |
|---|---|---|
| **Topic** | genau einer | Einstieg oben, darunter `Data/` und `Operations/` |
| **Gruppierung** | keiner | ausschließlich Topics oder weitere Gruppierungen |

```
Topic/
  <Einstiegspunkt>.cs    allein oben
  Data/                  Datentypen des Topics
  Operations/            Helfer, die nur der Einstieg nutzt
```

Alle sechs Ordner dieses Projekts sind Topics; eine Gruppierung kommt hier nicht vor. Einzige Ausnahme von der Ein-Datei-Regel ist `TableRendering/`, das `ITableRenderer` und `TableRenderer` nebeneinander hält — Vertrag und einzige Umsetzung zu trennen würde nichts sichtbar machen.

Die Zahl der Topics ist nach oben offen. Begrenzt wird nur die Breite einer Ebene: Wird sie unübersichtlich, sucht man eine echte fachliche Klammer und führt eine Gruppierung ein. Findet sich keine, bleibt die Ebene flach — ein erfundener Zwischenordner behauptet eine Zusammengehörigkeit, die es nicht gibt.

## Ordner ohne Baustein

| Ordner | Warum |
|---|---|
| `HostContracts/` | Bündelt die Verträge, die der Host erfüllt (`IConsole`, `IFileReader`, `ILogger`) — eine Ablage nach Wirkungsart. Fachlich gehört `IFileReader` zu **1.2.1**, `IConsole` zu **1.5.3** |
| `Common/` | Enthält mit `Result` eine Regel statt einer Verantwortung |

## Reifegrad

| Ebene | Bausteine | davon code | dialog | vermutung |
|---|---|---|---|---|
| System | 1 | 1 | 0 | 0 |
| Container | 1 | 1 | 0 | 0 |
| Komponente | 12 | 12 | 0 | 0 |

```
Veraltet: A00004 Seiten-Darstellung — aufgegangen in 1.3 und 1.4 (R00008)
Ohne eigenen Abschnitt: 1.1, 1.3, 1.4, 1.6 und alle Blätter — dort ist die Substanz erschöpft
Offene Fragen gesamt: 3  (A00001: 1, A00002: 2)
Bausteine ohne Quellen: A00004 (veraltet, planmässig)
Quellen-Pfade, die nicht mehr existieren: keine
```
