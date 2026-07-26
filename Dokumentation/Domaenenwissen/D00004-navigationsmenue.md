---
id: D00004
name: Navigationsmenü
status: Aktiv
sprache: de
verwandt: [D00002]
---

# Navigationsmenü

## Definition

Die Menüzeile und Tastensteuerung, mit der im interaktiven Viewer zwischen den Seiten gewechselt wird. Der Viewer verhält sich wie ein Pager: Vor jedem Zeichnen wird die Konsole gelöscht und die aktuelle Seite samt Menü neu aufgebaut.

## Bestandteile

**Menüzeile** — steht unter der Tabelle, Wortlaut exakt:

```
F)irst page, P)revious page, N)ext page, L)ast page, E)xit
```

**Eingabe** — ein einzelner Tastendruck, **ohne** Enter, groß-/kleinschreibungs-unabhängig.

**Tastenbelegung:**

| Taste | Wirkung |
|---|---|
| `F` | erste Seite |
| `P` | vorherige Seite |
| `N` | nächste Seite |
| `L` | letzte Seite |
| `E` | Programm endet mit Exit-Code 0 |

**Randverhalten:**

- `N` auf der letzten Seite und `P` auf der ersten Seite lassen die Anzeige stehen — die Navigation **klemmt**, es gibt keinen Umlauf.
- Eine ungültige Taste wird **still ignoriert**: Die Seite wird unverändert neu gezeichnet, ohne Hinweiszeile und ohne Fehlerton.

## Beziehungen

- [Seiten-Modell](D00002-seiten-modell.md) — liefert die Seitenzahl, an deren Rändern die Navigation klemmt.

## Vorkommen in Anforderungen

- [R00003](../../Anforderungen/R00003-interaktiver-viewer.md) — führend: Menü, Tastenbelegung, Klemmen, Ignorieren, Löschen der Konsole.
- [R00001](../../Anforderungen/R00001-csv-datei-einlesen.md) — nennt Menü-Wortlaut und Klemm-Verhalten als kursweite Kontextvorgabe.
- [R00002](../../Anforderungen/R00002-seite-als-tabelle-rendern.md) — grenzt das Menü ausdrücklich aus dem Rendering aus.

## Notizen / Quellen

- Menü-Wortlaut und Klemm-Verhalten sind Vorgaben der CCD-Kursaufgabe „CSV Viewer I".
- Verworfen: Zeilen-Eingabe mit Enter, Anhängen statt Löschen der Ausgabe, Hinweiszeile bei ungültiger Taste.
- Die Tastenzuordnung ist von der Konsole entkoppelt, damit der Loop ohne echte Konsole testbar bleibt.
