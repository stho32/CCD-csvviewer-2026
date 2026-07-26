---
id: D00001
name: CSV-Dokument
status: Aktiv
sprache: de
verwandt: [D00002, D00003]
---

# CSV-Dokument

## Definition

Der Inhalt einer eingelesenen CSV-Datei als fachlich neutrale Struktur: eine Kopfzeile mit den Spaltennamen plus eine geordnete Liste von Datensätzen. Jeder Datensatz hält seine Feldwerte positionsbasiert in Kopfzeilen-Reihenfolge.

Es gibt bewusst keine Bindung an ein konkretes Fachmodell (keine `Person`-Klasse o.ä.) — Paging und Rendering arbeiten ohne Kenntnis der konkreten Spalten. Das CSV-Dokument ist reine Data-Schicht ohne Logik.

## Bestandteile

- **Kopfzeile** — die Spaltennamen, gebildet aus der ersten Zeile der Datei.
- **Datensatz** — eine Datenzeile; Feldwerte sind ausschließlich über ihre Position zugreifbar, nicht über Spaltennamen.
- **Datensatzliste** — alle Datensätze in unveränderter Eingabereihenfolge.

Feste Format-Vorgaben (Kursvorgabe, nicht konfigurierbar):

- Kodierung UTF-8.
- Feldtrenner Semikolon `;`.
- Erste Zeile ist immer die Kopfzeile.
- Zellinhalte werden unverändert übernommen — kein Quoting, keine Escapes, keine Zeilenumbrüche innerhalb eines Feldes.

Strukturelle Validierung beim Einlesen:

| Fall | Ergebnis |
|---|---|
| Datei fehlt oder ist nicht lesbar | Fehler, kein Weiterverarbeiten |
| Komplett leere Datei | Fehler |
| Nur Kopfzeile, keine Datensätze | **gültig**, Dokument mit 0 Datensätzen |
| Datenzeile mit abweichender Feldanzahl | Fehler, benennt die betroffene Zeilennummer |

Fehler werden durchgängig über das Result-Muster signalisiert, nicht über Exceptions über Bausteingrenzen.

## Beziehungen

- [Seiten-Modell](D00002-seiten-modell.md) — teilt die Datensätze eines CSV-Dokuments in Seiten auf; die Kopfzeile bleibt davon unberührt und existiert genau einmal.
- [Tabellen-Ausgabeformat](D00003-tabellen-ausgabeformat.md) — rendert Kopfzeile und eine Datensatz-Teilmenge als Text.

## Vorkommen in Anforderungen

- [R00001](../../Anforderungen/R00001-csv-datei-einlesen.md) — führend: definiert Struktur, Format und Validierung.
- [R00002](../../Anforderungen/R00002-seite-als-tabelle-rendern.md) — rendert Kopfzeile und Datensätze positionsbasiert.
- [R00003](../../Anforderungen/R00003-interaktiver-viewer.md) — verdrahtet Einlesen und Parsen im Composition Root.

## Notizen / Quellen

- Delimiter `;`, UTF-8 und „erste Zeile = Kopfzeile" sind feste Vorgaben der CCD-Kursaufgabe „CSV Viewer I" und stehen nicht zur Diskussion.
- Der Datei-Zugriff ist absichtlich CSV-agnostisch von der Struktur-Interpretation getrennt: Lesen liefert Zeilen, das Parsen erzeugt daraus das Dokument.
- Ein typgebundenes Zeilenmodell wurde verworfen — CSV muss generisch bleiben.
