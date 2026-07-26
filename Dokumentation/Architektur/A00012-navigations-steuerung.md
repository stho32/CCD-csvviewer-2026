---
id: A00012
name: Navigations-Steuerung
code: PageNavigator
ebene: Komponente
eltern: A00005
repo:
status: Aktiv
herkunft: code
quellen:
  - Source/CsvViewer/CsvViewer.BL/Interaction/Operations/
verwandt: [A00010]
domaene: [D00004]
---

# Navigations-Steuerung

## Verantwortung

Bestimmt aus einem Tastendruck den nächsten Seitenindex.

## Schnittstellen

**Eingehend** — `NavigationCommandMapper.Map(char key)` und `PageNavigator.Apply(NavigationCommand command)`.

**Ausgehend** — `Result<NavigationCommand>` beziehungsweise `Result<int>` mit dem neuen Seitenindex.

Verhalten: `F`/`P`/`N`/`L`/`E` gross wie klein; alles andere wird zu `None` und lässt den Index unverändert. An den Rändern wird geklemmt — `N` auf der letzten und `P` auf der ersten Seite bleiben stehen, es gibt keinen Umlauf.

Datentyp dieses Bausteins: `NavigationCommand`.

## Abhaengigkeiten

Keine. Der Mapper ist pur; der Navigator hält als einzigen Zustand den aktuellen Seitenindex.

## Entscheidungen

Tastenzuordnung und Indexfortschaltung sind von der Konsole entkoppelt, damit beides ohne Terminal prüfbar ist.

## Offene Fragen

Keine.

## Notizen / Quellen

Tastenbelegung und Klemmverhalten im [Navigationsmenü](../Domaenenwissen/D00004-navigationsmenue.md).
