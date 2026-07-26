---
id: D00003
name: Tabellen-Ausgabeformat
status: Aktiv
sprache: de
verwandt: [D00001, D00002]
---

# Tabellen-Ausgabeformat

## Definition

Das zeichengenaue Textformat, in dem eine Seite — Kopfzeile plus Datensatz-Teilmenge — als Tabelle dargestellt wird. Das Format ist durch das Beispiel der CCD-Kursaufgabe festgelegt und nicht konfigurierbar.

```
Name |Age|City    |
-----+---+--------+
Peter|42 |New York|
Paul |57 |London  |
Mary |35 |Munich  |
```

## Bestandteile

**Zellen und Rahmen:**

- Zelle = Wert, rechts mit Leerzeichen auf Spaltenbreite aufgefüllt, gefolgt von `|`.
- Werte sind durchgängig **linksbündig**. Keine zusätzlichen Padding-Leerzeichen um den Inhalt.
- **Kein** führendes oder äußeres `|`, **keine** obere oder untere Rahmenlinie.
- Genau **eine** Trennlinie zwischen Kopf- und Datenzeilen: je Spalte `-` in Spaltenbreite, gefolgt von `+`.
- Alle `|`-Trenner stehen spaltenweise bündig untereinander.

**Spaltenbreite:**

- Spaltenbreite = längster Wert der Spalte, **einschließlich des Spaltennamens**.
- Berechnet über genau die Zeilen der aktuellen Seite, nicht über das Gesamtdokument.
- Folge: Dieselbe Spalte kann auf verschiedenen Seiten unterschiedlich breit sein.

**Sonderfälle:**

| Fall | Ausgabe |
|---|---|
| Seite mit 0 Datensätzen | nur Kopfzeile und Trennlinie |
| leerer Zellwert | reine Auffüll-Leerzeichen, Spaltenbreite bleibt erhalten |

**Kein horizontales Scrollen** — breite Tabellen laufen über die Konsolenbreite hinaus. Das ist gewollt, nicht ein offener Mangel.

Das Rendern ist eine pure Operation: Es liefert die fertige Tabelle als Zeichenkette und gibt selbst nichts auf der Konsole aus. Die Ausgabe des Strings gehört zur Integrationsschicht.

## Beziehungen

- [CSV-Dokument](D00001-csv-dokument.md) — liefert Kopfzeile und Datensätze; das Rendern arbeitet rein positionsbasiert ohne Fachmodell.
- [Seiten-Modell](D00002-seiten-modell.md) — bestimmt, welche Zeilen in eine Breitenberechnung eingehen.

## Vorkommen in Anforderungen

- [R00002](../../Anforderungen/R00002-seite-als-tabelle-rendern.md) — führend: Format, Spaltenbreiten, Sonderfälle.
- [R00003](../../Anforderungen/R00003-interaktiver-viewer.md) — jede Seite berechnet ihre Spaltenbreiten unabhängig über den Renderer.
- [R00001](../../Anforderungen/R00001-csv-datei-einlesen.md) — hält als Kontext fest, dass die Kopfzeile bei den Spaltenbreiten mitzählt.

## Notizen / Quellen

- Zeichengenau festgelegt durch das Beispiel der CCD-Kursaufgabe „CSV Viewer I".
- Verworfen: Vollrahmen mit `+`-Ecken, Trennlinie ohne `+`, rechtsbündige oder zahlenspezifische Ausrichtung.
