---
id: A00002
name: CsvViewer CLI
code: CsvViewer/
ebene: Container
eltern: A00001
repo:
status: Aktiv
herkunft: code
quellen:
  - Source/CsvViewer/CsvViewer/
  - Source/CsvViewer/CsvViewer.BL/
verwandt: [A00001]
domaene: [D00001, D00002, D00003, D00004]
---

# CsvViewer CLI

## Verantwortung

Stellt den CSV-Viewer als einzeln startbaren Konsolenprozess bereit.

## Schnittstellen

**Eingehend** — `Program.Main(string[] args)` als einziger Einstiegspunkt; Tastendrücke über `SystemConsole.ReadKey()`.

**Ausgehend** — stdout (Tabelle und Menü), stderr (`[ERROR]`-Meldungen), Exit-Code, lesender Dateizugriff über `FileReader`.

**Interner Aufbau** — der Container besteht aus zwei Assemblies, geschichtet nach Wirkungsart (IODA nach Westphal):

| Schicht | Bausteine | Eigenschaft |
|---|---|---|
| Integration | `Program` (Composition Root), `InteractiveViewer` (Loop) | verdrahtet und steuert, entscheidet fachlich nichts |
| Operation | `ArgumentsParser`, `CsvParser`, `Paginator`, `NavigationCommandMapper`, `PageNavigator`, `TableRenderer` | pur und seiteneffektfrei |
| Data | `CsvDocument`, `CsvHeader`, `CsvRow`, `CsvRowCollection`, `PagedDocument`, `CsvPageCollection`, `ViewerArguments`, `NavigationCommand`, `Result` | nur Daten, keine Logik |
| I/O-Rand | `FileReader` (BL), `SystemConsole` und `ConsoleLogger` (Entry Point) | einzige Berührung mit der Aussenwelt |

Ablauf eines Starts:

```
args[] → ArgumentsParser → FileReader → CsvParser → Paginator → InteractiveViewer
                                                                  ├→ TableRenderer
                                                                  └→ SystemConsole
```

Jeder Schritt liefert `Result<T>`; beim ersten Fehler bricht `Program` ab und bildet ihn auf einen Exit-Code ≠ 0 ab.

## Abhaengigkeiten

**Innere Struktur** — sechs Komponenten, zwei davon weiter zerlegt:

| Komponente | Frage, die sie beantwortet |
|---|---|
| [Kommandozeilen-Interpretation](A00007-kommandozeilen-interpretation.md) | Was hat der Anwender überhaupt verlangt? |
| [Dokument-Beschaffung](A00003-dokument-beschaffung.md) | Wie kommt die Datei ins Programm? |
| [Seiten-Aufteilung](A00010-seiten-aufteilung.md) | Wie wird die Datenmenge portioniert? |
| [Tabellen-Rendering](A00011-tabellen-rendering.md) | Wie sieht eine Portion als Text aus? |
| [Bedienung](A00005-bedienung.md) | Wie steuert der Mensch das? |
| [Komposition](A00006-komposition.md) | Wer steckt alles zusammen? |

Zwei Schnitte sind das Ergebnis von Korrekturen, die beim Modellieren auffielen:

- Die Kommandozeilen-Interpretation steht **vor** der Dokument-Beschaffung statt in ihr. Sie liefert zwei Werte, die auseinanderlaufen — der Dateipfad speist die Dokument-Beschaffung, die Seitengrösse die Seiten-Aufteilung ([R00006](../../Anforderungen/R00006-kommandozeilen-interpretation-trennen.md)).
- Seiten-Aufteilung und Tabellen-Rendering sind **getrennt**, obwohl beide „mit Seiten zu tun haben". Sie kennen einander nicht, haben verschiedene Aufrufer und laufen in verschiedenen Phasen; der Renderer nimmt nicht einmal das Ergebnis der Aufteilung entgegen ([R00008](../../Anforderungen/R00008-ein-topic-ein-einstiegspunkt.md)). Der frühere Sammelbaustein [Seiten-Darstellung](A00004-seiten-darstellung.md) steht auf `Veraltet`.

Sechs Komponenten sind hier kein Kompromiss, sondern das richtige Ergebnis. Die Faustregel „drei bis fünf" fragt nach einer Klammer, sie verbietet keine sechste Komponente — und eine echte Klammer gibt es hier nicht. Eine Gruppierung nach Lebenszyklus (`Startup/`, `Runtime/`) wäre erfunden: Sie ordnet nach Zeit statt nach Fachlichkeit und behauptet eine Zusammengehörigkeit, die im Code nicht existiert. Die flache Liste ist ehrlicher.

`Komposition` ist dabei reine Technik ohne fachliche Entsprechung.

**Topic oder Gruppierung** — jeder dieser sechs Ordner ist ein *Topic*: Er hat genau einen Einstiegspunkt, also einen Typ, dessen Methode von aussen gerufen wird. Ein *Gruppierungs*-Ordner hätte keinen und dürfte ausschliesslich weitere Topics enthalten. In diesem Projekt kommt keine Gruppierung vor; erst bei deutlich mehr Topics mit echter gemeinsamer Klammer wäre eine sinnvoll.

**Modell und Ordnerstruktur** — die Ordner der Geschäftslogik tragen die englische Entsprechung der Bausteinnamen:

| Baustein | Ordner |
|---|---|
| Kommandozeilen-Interpretation | `CommandLineInterpretation/` |
| Dokument-Beschaffung | `DocumentAcquisition/` |
| Seiten-Aufteilung | `Pagination/` |
| Tabellen-Rendering | `TableRendering/` |
| Bedienung | `Interaction/` |
| Komposition | `Program.cs` im Entry Point |

**Ablage innerhalb eines Topics** — jedes Topic hat genau einen Typ auf oberster Ebene, nämlich den Einstiegspunkt; darunter liegen `Data/` für Datentypen und `Operations/` für Helfer, die nur der Einstieg nutzt ([R00007](../../Anforderungen/R00007-datenobjekte-in-data-unterordner.md), [R00008](../../Anforderungen/R00008-ein-topic-ein-einstiegspunkt.md)). Einzige Ausnahme ist `TableRendering/`, das Vertrag und Umsetzung nebeneinander hält.

Zwei Ordner entsprechen bewusst **keinem** Baustein: `HostContracts/` bündelt die Verträge, die der Host erfüllt (`IConsole`, `IFileReader`, `ILogger`), und `Common/` enthält mit `Result` eine Regel statt einer Verantwortung. Beide folgen der Wirkungsart, während das Modell dem Datenfluss folgt — fachlich gehört `IFileReader` weiterhin zu [Datei-Zugriff](A00008-datei-zugriff.md), `IConsole` zu [Konsolen-Anbindung](A00014-konsolen-anbindung.md).

**Extern**

- [CsvViewer](A00001-csvviewer.md) — übergeordnetes System.
- .NET 10.0 Runtime.
- Dateisystem (lesend).
- Keine Laufzeit-Pakete. Die einzigen NuGet-Referenzen im Repo liegen in den Testprojekten.

**Abhängigkeitsrichtung** — `CsvViewer` → `CsvViewer.BL`, nie umgekehrt. Die Verträge `IConsole`, `IFileReader`, `ILogger` und `ITableRenderer` liegen in der BL; `SystemConsole` und `ConsoleLogger` implementieren sie am Rand im Entry-Point-Projekt. Dadurch enthält die Geschäftslogik keine direkte `System.Console`-Nutzung und der Loop ist mit einer Fake-Konsole ohne Terminal testbar.

## Entscheidungen

- `InteractiveViewer` liegt bewusst in der BL statt im Entry Point, damit der Loop testbar bleibt — [R00003](../../Anforderungen/R00003-interaktiver-viewer.md).
- Die Konsolen-Adapter liegen unter `HostSpecific/` im Entry-Point-Projekt, damit host-spezifische Eigenheiten nicht in die BL sickern.
- Bei umgeleiteten Kanälen weicht `SystemConsole` aus: `ReadKey()` liest zeichenweise aus dem Strom, `Clear()` wird zum No-op. Damit ist der Viewer ohne Terminal steuerbar (`printf 'nne' | csvviewer datei.csv`), was die E2E-Tests plattformunabhängig macht.

## Offene Fragen

- `CsvViewer.csproj` setzt `PublishSingleFile` und `SelfContained`, obwohl das Werkzeug laut Dialog nicht verteilt, sondern nur über `dotnet run` gestartet wird. Beides ist damit wirkungsloser Scaffolding-Rest — entfernen oder als Option belassen?
- Der Terminal-Pfad von `SystemConsole.ReadKey()` (echtes TTY) ist nicht automatisiert abgedeckt; die E2E-Tests nutzen ausschliesslich den Strom-Pfad. Soll das so bleiben oder rechtfertigt es ConPTY-Interop im Test?

## Notizen / Quellen

- Einziges Projekt mit `<OutputType>Exe</OutputType>`: `Source/CsvViewer/CsvViewer/CsvViewer.csproj`. `CsvViewer.BL` ist eine Bibliothek und wird in denselben Prozess geladen, ist also kein eigener Container.
- Composition Root: `Source/CsvViewer/CsvViewer/Program.cs`.
- Konsolen-Adapter: `Source/CsvViewer/CsvViewer/HostSpecific/IO/SystemConsole.cs`, `HostSpecific/Logging/ConsoleLogger.cs`.
- Manuelle Erprobung: `testlauf.sh` mit `beispiel.csv` im Repo-Root.
