---
id: A00013
name: Viewer-Ablauf
code: InteractiveViewer
ebene: Komponente
eltern: A00005
repo:
status: Aktiv
herkunft: code
quellen:
  - Source/CsvViewer/CsvViewer.BL/Interaction/InteractiveViewer.cs
  - Source/CsvViewer/CsvViewer.BL/Interaction/Data/
verwandt: [A00011, A00012, A00014]
domaene: [D00004]
---

# Viewer-Ablauf

## Verantwortung

Hält den interaktiven Zyklus am Laufen, bis der Anwender beendet.

## Schnittstellen

**Eingehend** — `InteractiveViewer.Run(PagedDocument document)`.

**Ausgehend** — `Result`: Erfolg beim regulären Beenden über `E`, sonst die Meldung des ersten fehlgeschlagenen Schritts.

Der Zyklus: Konsole leeren → aktuelle Seite rendern und samt Menüzeile schreiben → Taste lesen → auf Befehl abbilden → Seitenindex fortschalten. Bricht ein Schritt ab, endet der Ablauf mit dessen Fehler.

## Abhaengigkeiten

Bekommt beide Mitspieler als Abstraktion in den Konstruktor gereicht:

- [Tabellen-Rendering](A00011-tabellen-rendering.md) über `ITableRenderer`
- [Konsolen-Anbindung](A00014-konsolen-anbindung.md) über `IConsole`

Nutzt ausserdem [Navigations-Steuerung](A00012-navigations-steuerung.md) direkt (pure Typen, keine Abstraktion nötig).

Dieser Baustein ist Integration — er trifft keine fachliche Entscheidung selbst, sondern reiht die Schritte auf.

## Entscheidungen

Liegt in der Geschäftslogik statt im Entry Point. Dadurch ist der Zyklus mit einer Test-Konsole ohne echtes Terminal prüfbar, während im Entry Point nur die Verdrahtung bleibt ([R00003](../../Anforderungen/R00003-interaktiver-viewer.md)).

## Offene Fragen

Keine.

## Notizen / Quellen

Hält die Menü-Konstante `F)irst page, P)revious page, N)ext page, L)ast page, E)xit` als einzige Textkonstante der Bedienung.
