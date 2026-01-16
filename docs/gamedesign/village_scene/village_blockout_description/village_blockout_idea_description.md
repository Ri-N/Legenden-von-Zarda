## Dorf Szene Beschreibung

---

### Struktur und Aufbau des Dorfes

Das Dorf soll folgendes beinhalten:
- Das Haus in dem der Protagonist lebt
- Den Ausrüstungsladen, in der nähe des Hauses vom Protagonisten
- Die Kampfarena, zum trainieren
- Das Gasthaus für reisende Händler und Söldner
- Mind. 3 weitere Häuser zum füllen des Dorfes
- Eine Straße die in der Mitte des Dorfes vertikal durchläuft, in der Regel besetzt für Händler zum durchfahren
- Ein großer Zaun um das Dorf
- Eingangs-/Ausgangstore oben und unten beim Dorf
- NPCs zum Interagieren

---

### Wichtige NPCs im Dorf

#### George:
- Seit Jahren im Dorf als Wache aktiv.
- Nicht gesprächig und verbringt seine meiste Zeit bei der Kampfarena wenn er gerade nicht die Tore bewacht.
- Bewundert `Stärke`.

#### Bertram:
- Der Besitzer des Ausrüstungsladen.
- Er liebt es Geld und deshalb sind die Preise in seinem Laden extrem hoch, als einziger Ausrüstungsladen im Dorf und auch im gesamten Umkreis des Waldes kann sich dies auch leisten.
- Er besitzt eine Leidenschaft für das Schmieden.
- Bewundert `Glück`.

#### Mila:
- Dorfmädchen im gleichen Alter wie der Protagonist.
- Liest gerne in ihrere Freizeit Bücher, hauptsächlich Bücher über Magie und Geschischte.
- Bewundert `Intelligenz`.

---

### Beschreibung der Assets im Dorf

- Häuser:
  - Die Häuser sind alle einstöckig also haben nur ein Erdgeschoss und sind auch Holzplanken gebaut.
<br>
- Ausrüstungsladen:
  - Der Ausrüstungsladen ist kein extra Gebäude sondern er ist offen. Das heißt man sieht direkt was alles angeboten wird und muss nicht extra ein Gebäude betreten. Man sieht Schwerter, Speere, Äxte und Schilder an der Wand hängen. Ein Amboss zum Schmieden und reparieren von Waffen ist auch zu sehen.
<br>
- Kampfarena:
  - Die Kampfarena ist ein offener Platz der mit einem kleinen Holzzaun umzingelt ist. In der Kampfarena befindet sich nur Wachen zum trainieren oder zum Turniere austragen um seine Fähigkeiten zu testen.
<br>
- Gasthaus:
  - Das Gasthaus ist im Vergleich zu den Häusern viel größer. Es ist viel Breiter und besitzt auch ingesamt 3 Etagen (EG, 1G, 2G). Im EG gibt es eine Bar mit Tischen und Stühlen wo man Essen Bestellen kann. In 1G und 2G gibt es jeweils 20 Zimmer zum die man mieten kann zum Schlafen.
<br>
- Straße:
  - Eine einfache Erdstraße wie man es aus einer Landschaft kennt. Die Straße hat sich durch das regelmäßige Reisen von Händlern durch das Dorf geformt.
<br>
- Zaun:
  - Der Zaun besteht aus Holzpfalen die das Dorf umzingeln um es vor potenziellen Monster angriffen zu schützen.
<br>
- Eingangs-/Ausgangstore:
  - Die Tore sind ebenfalls aus Holz. Es gibt eine Leiter mit der man auf das Tor hochklettern kann auf einen Spähposten. Nachts werden die Tore geschlossen und die Wachen klettern auf das Tor oben drauf und halten Ausschau.

---

### Mechanics:

#### Tageszyklus System:

- Es gibt 3 Tageszyklen: Morgens, Mittag, Abend.
<br>
- Diese rotieren nach bestimmten Aufgaben/Aktivitäten durch, z. B. Quest abgeschlossen, in der Kampfarena trainiert, eine Interaktion mit einem *wichtigem* NPC durchführen (Dialog Optionen die den Beziehungsstatus beeinflussen).
<br>
- Die Tageszyklen sollen durch die *Atmosspähre* gekennzeichnet werden, kein billiges HUD links oben das den Tageszyklus zeigt.
  - **Morgens:**
  Ein morgengrauen mit einem Rot- Orangeton. Man hört zwischendruch Vögel zwitschern, da wir uns tief in einem Wald befindet. Das fördert *Immersion* in die Spielwelt.
  - **Mittag:**
  Klares Tageslicht im Dorf. Man hört Leute reden an bestimmten Stellen im Dorf.
  - **Abend:**
  Das Licht im Dorf wird Abends hauptsächlich durch das Mondlicht und Fackeln an den Zäunen und Häusern erzeugt. Man hört Eulen zwischendurch.

#### Interaktion mit wichtigen NPCs

Wichtige NPCs haben einen Beziehungsstatus dir gegenüber. Dieser Beziehungststaus kommt in Form von Zuständen. Diese Zustände können sich ändern je nachdem wie man mit dem NPC interagiert (positiv und negativ).

Es soll folgende Zustände geben:
 - Gleichgültig:
 Der Normalzustand, die meisten Dorfbewohner kennen den Protagonisten kaum, da er sehr zurückhalten ist und weder negativ noch positiv auffält.
 <br>
 - Reserviert:
 In diesem Zustand sind die NPCs eher zurückhaltend, sie sind dem Protagonist nicht Misstrauisch gegenüber aber sie werden sich mehr zurückhalten und dir knappere aber trotzdem höfliche Antworten geben.
 <br>
 - Verwirrt:
 Die NPCs verstehen vom Protagonisten die Aktionen und das Verhalten nicht ganz, sie sehen widersprüche und können das nicht Einordnung. Dieser Zustand ist wie Gleichgültig weder positiv noch negativ, aber die NPCs werden dich mehr Hinterfragen und das wird man aus den Dialogen auch rauslesen.
 <br>
 - Wohlgesonnen:
 In diesem Zustand sind die NPCs dem Protagonisten freundlich gegenüber, die Aktionen des Protagonisten haben einen positiven Eindruck hinterlassen. Diese NPCs sind aber damit nicht "Freunde" oder ähnliches sondern haben eine positive Neigung. Die NPCs haben einen freundlicheren Ton in Dialogen und werden eventuell Alltagsinfos teilen mit denen man Quest, Ausrüstung, etc. bekommen/finden kann.
 <br>
 - Ablehnend:
 Die NPCs haben eine klare Abneigung dem Protagonsit gegenüber. Es ist kein "Hass" oder "Wütend sein", sondern eine klare Distanz zu dem Protagonisten halten wollen dass sich dadurch zeigt das man mit dem NPC erst gar nicht sprechen kann (bis ein bisschen zeit vergangen ist).

#### Zustand Dynmaik:

Gleichgültig
 |---> Reserviert
 |---> Wohlgesonnen
 |---> Verwirrt

Verwirrt
 |---> Reserviert
 |---> Wohlgesonnen
 |---> Ablehnend
 |---> Gleichgültig

Wohlgesonnen
 |---> Verwirrt
 |---> Gleichgütig

Reserviert
 |---> Verwirrt
 |---> Ablehnend

Ablehnend
 |---> Reserviert
 |---> Verwirrt 

---

### Priorisierung der Assets im Dorf

Die Assets sind in Reihenfolge der Relevanz zur Game-User-Story für das Dorf priorisiert. Diese Game-User-Story wird eine Quest für einen NPC sein bei dem man einen kleinen Auftrag erledigen muss.

1. Häuser
2. Ausrüstungsladen
3. Kampfarena
4. Straße
5. Zäune
6. Gasthaus
7. Eingangs-/Ausgangstore

---

### Beschreibung der Game-User-Story im Dorf

Die folgende Game-User-Story beschreibt eine Quest die der Protagonist für den NPC Bertram erledigt.

1. Der Spieler befindet sich in seinem Zimmer
2. Der Spieler läuft zu seinem Schwert und interagiert damit und merkt das er es mal reparieren sollte
3. Der Spieler interagiert ein 2tes mal mit dem Schwert und entscheidet sich das Schwert mitzunehmen
4. Der Spieler läuft zum Ausrüstungsladen
5. Der Spieler spricht mit Bertram und will sein Schwert erneuern lassen
6. Bertram gibt dem Spieler eine Quest, der Spieler soll ein schweres Objekt an einem Wächter (George) bringen
7. Der Spieler läuft zur Kampfarena weil sich George dort oft aufhält
8. Der Spieler übergibt das Objekt an George, und George gibt das Geld für das Objekt an dem Spieler
9. Der Spieler bringt das Geld zu Bertram und er fängt an das Schwert in einen besseren Zustand zu bringen, man kann es ein Tag später abholen.