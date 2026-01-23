# Village Scene Blockout – Dokumentation

## 1. Übersicht
Kurze Übersicht zum Zweck des Dorf-Blockouts und dem Kontext im Spiel.

- **Szenentyp:** Dorf (Außenareal)
- **Zweck:**  
  - Zentrale Hub-Area des Spiels
  - Verbindung zwischen Story, Quests und Gameplay-Systemen  
  - Einführung wichtiger NPCs  
  - Trainings-, Handels- und Sozialraum
- **Hinweis:**  
  Das Dorf dient als lebendige Spielwelt. Die hier beschriebene Game-User-Story **SOLL** spielbar sein, ist aber **nicht zwingend erforderlich**, um das Spiel voranzubringen. Der Spieler kann das Dorf frei erkunden, Aktivitäten ignorieren oder gezielt nutzen.

---

## 2. Interaktions- & Navigationssystem

### Steuerung
- **Bewegung:** WASD (Standard)
- **Interaktionstaste:** `E`
- **Mindestdistanz:** < 0.75m (größer als bei Innenräume)

### Interaktions-Feedback
- Visueller Hinweis `[E]` bei Objekten/NPCs  
- Leichte Hervorhebung (Outline / Light Tint)  
- Dialogfenster bei NPC interaktion oder direkte Aktion

---

## 3. Tageszyklus & Atmosphäre

### Tagesphasen
- **Morgen:**
  - Warmes Licht
  - Vogelgeräusche

- **Mittag:**  
  - Klares Licht  
  - Dorfalltag hörbar  

- **Abend:**  
  - Mondlicht  
  - Fackeln  
  - Eulen  

**Hinweis:**  
Der Tageszyklus wird **rein atmosphärisch** vermittelt **KEIN** HUD.

---

## 4. Wichtige NPCs im Dorf

### Gregory – Wache
- Aufenthaltsort: Kampfarena / Tore  
- Persönlichkeit: Still, diszipliniert  
- Bewundert: `Stärke`

**Interaktionsrolle:**  
- Trainings-Quest  
- Kampfrelevante Dialoge  

---

### Bertram – Ausrüstungshändler
- Aufenthaltsort: Ausrüstungsladen  
- Persönlichkeit: Gierig, aber leidenschaftlicher Schmied  
- Bewundert: `Glück`

**Interaktionsrolle:**  
- Händler
- Questgeber
- Upgrade-System

---

### Mila – Dorfbewohnerin
- Aufenthaltsort: Haus / Dorfbereich
- Persönlichkeit: Ruhig, neugierig
- Bewundert: `Intelligenz`

**Interaktionsrolle:**  
- Lore / Backgorund Informationen
- Optionale Nebenquests

---

## 5. Dorf Mechanics

## 5.1 Beziehungsstatus-System (NPCs)

### Zustände
- Gleichgültig  
- Reserviert
- Verwirrt
- Wohlgesonnen
- Ablehnend

### Zustandsdynamik
- Gleichgültig → Reserviert / Wohlgesonnen / Verwirrt
- Verwirrt → Reserviert / Wohlgesonnen / Ablehnend / Gleichgültig
- Wohlgesonnen → Verwirrt / Gleichgültig
- Reserviert → Verwirrt / Ablehnend
- Ablehnend → Reserviert / Verwirrt

---

## 5.2 Dorf-Aktivitäten & Zeitfortschritt

Im Dorf gibt es Aktivitäten, mit denen der Spieler bewusst Zeit verstreichen lassen kann.  
Diese Aktivitäten dienen der **Charakterentwicklung**, **Beziehungsarbeit** und dem **Tageszyklus-Management**.

Eine Aktivität:
- Verändert Werte
- Beeinflusst Beziehungen
- Lässt die Zeit voranschreiten

---

### Aktivität: Training in der Kampfarena

**Ort:** Kampfarena  
**NPCs:** Gregory, Wachen  

**Effekt:**
- Stärke +1
- Tageszeit:
  - Morgen → Mittag
  - Mittag → Abend

**Beschreibung:**
- Körperlich fordernd
- Kurze Trainingssequenzen
- Kein Dialog nötig oder sehr reduziert

**Design-Ziel:**
- Mechanische Verbesserung
- Spiegelung von Gregorys Werten (Stärke)

---

### Aktivität: Arbeiten im Gasthaus

**Ort:** Gasthaus  
**NPCs:** Wirt, Reisende  

**Effekt:**
- Kleine Menge Geld
- Tageszeit:
  - Morgen → Mittag
  - Mittag → Abend

**Beschreibung:**
- Tragen
- Aufräumen
- Bedienen

**Design-Ziel:**
- Gefühl von Alltag
- Langsamer, ehrlicher Fortschritt

---

### Aktivität: Bücher lesen im Zimmer

**Ort:** Zimmer (Schreibtisch)  

**Effekt:**
- Intelligenz +1
- Tageszeit:
  - Morgen → Mittag
  - Mittag → Abend

**Beschreibung:**
- Ruhige Aktivität
- Innerer Monolog
- Lore-Fragment möglich

**Design-Ziel:**
- Charaktervertiefung
- Verbindung zu Mila (indirekt)

---

### Aktivität: Langer Dialog mit Mila

**Ort:** Dorf / Haus von Mila  

**Effekt:**
- Beziehungsstatus zu Mila verbessert sich
- Tageszeit:
  - Morgen → Mittag
  - Mittag → Abend

**Beschreibung:**
- Ruhiger Dialog
- Themen:
  - Bücher
  - Magie
  - Geschichte

**Design-Ziel:**
- Soziale Progression
- Vorbereitung auf Nebenquests

---

### System-Hinweis

- Der Spieler entscheidet bewusst:
  - *Was* er tut
  - *Wann* die Zeit vergeht
- Zeit ist eine Ressource
- Nicht alle Aktivitäten sind an jedem Tagesabschnitt verfügbar

---

## 6. Dorf – Struktur & Layout

### Gesamtübersicht Dorf Blockout:
![Village Blockout Übersicht](../village_blockout_screenshots/Initial_Village_Blockout.png)

### Annotierte Übersicht (Zonen & Assets):
![Village Blockout Annotation](../village_blockout_screenshots/Initial_Village_Blockout_Asset_Naming.png)

### Zonen-Übersicht:
| Zone | Beschreibung |
|------|--------------|
| Wohnbereich (West) | Häuser inklusive Haus des Protagonisten und von Mila |
| Handelsbereich (Süd-Westlich) | Ausrüstungsladen und Haus von Bertram |
| Trainingsbereich (Süd-Östlich) | Kampfarena und Aufenthaltsort von Gregory |
| Wohnbereich (Ost) | Häuser von einfachen NPCs/Dorfbewohnern |
| Gasthausbereich (Nord-Östlich) | Gasthaus und Aufenthaltsort von Händler und Söldnern |

### Infrastruktur:
| Asset | Beschreibung |
|-------|--------------|
| Straße | Läuft in der Mitte durch das Dorf hindurch, hauptsächlich von fahrenden Händlern besetzt |
| Zaun | Eine großer Zaun um das Dorf einmal drum, zur Verteidigung von Wildtieren und Monstern nachts |
| Tore (nicht im Blockout drin) | Beide Toren am beiden Enden der Straße schließen Abends zur Sicherheit des Dorfes |

---

## 7. Game-User-Story Ablauf – Dorf Quest (Bertram)

Die folgende Game-User-Story startet im **Zimmer des Protagonisten** und führt den Spieler anschließend ins Dorf.

Sie verbindet die Room-Szene direkt mit der Dorf-Szene und nutzt das Schwert als zentrales Gameplay-Element.

### Hinweis zu Protagonist und NPCs darstellung

#### Der Protagonist ist mit einem Kegel in dem Blockout repräsentiert:
![Protagonist darstellung](../village_blockout_screenshots/Protagonist_Representation.png)

#### NPCs sind mit einem umgedrehten Kegel repräsentiert:
![NPCs darstellung](../village_blockout_screenshots/NPC_Representation.png)

---

### Schritt 1: Im Zimmer aufwachen
![Schritt 1 – Zimmer Start](../village_blockout_screenshots/Step_1_Room_Start.png)

> Der Spieler befindet sich in seinem Zimmer.

---

### Schritt 2: Interaktion mit dem Schwert
![Schritt 2 – Schwert im Zimmer](../village_blockout_screenshots/Step_2_Room_Sword.png)

> Der Spieler interagiert mit dem rostigen Schwert im Zimmer zweimal.

**Dialog:**

`Erste Interaktion:`
```text
*Du blickst auf das Schwert*
"..."
"Rost."
"Mehr Rost als Erinnerung."
"..."
"Früher war es schwer."
"Jetzt fühlt es sich nur noch müde an."
"..."
"Vielleicht lässt sich noch etwas retten."
```

`Zweite Interaktion:`
```text
"Allein lassen bringt nichts."
"Mitnehmen auch nicht."
"..."

*Willst du das Schwert mitnehmen?*
> Ja
> Nein
```

<br>

Wenn der Spieler Ja drückt dann hat er das Schwert in seinem Iventar. Er will es zum Ausrüstungsladen bringen um es reparieren zu lassen.

---

### Schritt 3: Zimmer verlassen
![Schritt 3 – Zimmer verlassen](../village_blockout_screenshots/Step_3_Room_Exit.png)

> Der Spieler verlässt das Zimmer und betritt das Dorf.

**Dialog:**
```text
*Willst du rausgehen?*
> Ja
> Nein

> Nein:
"Draußen ist es laut."
"Hier drin ist es ruhig."
"..."
"Zu ruhig."

> Ja:
*Kein Dialog, du verlässt das Zimmer*
``` 

---

### Schritt 4: Ankunft im Dorf

#### Nördliche Sicht:
![Schritt 4 – Dorf Ankunft (Nördliche Sicht)](../village_blockout_screenshots/Step_4_Village_Enter_North_Direction.png)

#### Östliche Sicht:
![Schritt 4 – Dorf Ankunft (Östliche Sicht)](../village_blockout_screenshots/Step_4_Village_Enter_East_Direction.png)

#### Südliche Sicht:
![Schritt 4 – Dorf Ankunft (Südliche Sicht)](../village_blockout_screenshots/Step_4_Village_Enter_South_Direction.png)

#### Vogelperspektive:
![Schritt 4 – Dorf Ankunft (Vogelperspektive)](../village_blockout_screenshots/Step_4_Village_Enter_Bird_Perspective.png)

> Der Spieler steht vor seinem Haus

---

### Schritt 5: Weg zum Ausrüstungsladen
![Schritt 5 – Weg zum Shop](../village_blockout_screenshots/Step_5_Home_To_Shop.png)

> Der Spieler läuft entlang der Straße zum Ausrüstungsladen.

---

### Schritt 6: Gespräch mit Bertram
![Schritt 6 – Gespräch mit Bertram](../village_blockout_screenshots/Step_6_Armory_Shop_Bertram.png)

> Der Spieler spricht Bertram auf das beschädigte Schwert an.

**Dialog:**

`Protagnist:`
```text
*Du legst das Schwert auf den Tresen*
"..."
"Es war mal besser."
```

<br>

`Bertram:`
```text
*Bertram betrachtet die Klinge*
"Hm."
"..."
"War?"
"..."
"Das Ding wurde lange nicht gepflegt."
"Und lange nicht benutzt."
```

---

### Schritt 6.5: Quest erhalten
![Schritt 6.5 – Quest erhalten](../village_blockout_screenshots/Step_6_Armory_Shop_Bertram.png)

> Bertram bittet den Spieler, ein schweres Objekt zu Gregory zu bringen.

**Dialog:**

`Bertram:`
```text
*Bertram lehnt sich zurück*
"Ich kann es richten."
"..."
"Aber nicht umsonst."
```

```text
"Bring Gregory das hier."
"Schwer."
"Unhandlich."
"..."
"Er schuldet mir noch etwas."
"Du bringst es ihm."
"Er gibt dir das Geld."
"Ich repariere das Schwert."
```

<br>

`Protagonist (intern):`
```text
"..."
"Natürlich schuldet jemand Bertram etwas."
```

<br>

Nachdem Gespräch hat der Spieler jetzt ein schweres Objekt in seinem Inventar. Dieses muss er zu Gregory bringen.

Quest-Ziel:
- Übergabe an Gregory in der Kampfarena  

---

### Schritt 7: Weg zur Kampfarena
![Schritt 7 – Weg zur Arena](../village_blockout_screenshots/Step_7_Armory_Shop_To_Arena_Gregory.png)

> Der Spieler bewegt sich zur Kampfarena.

---

### Schritt 8: Übergabe an Gregory
![Schritt 8 – Übergabe an Gregory](../village_blockout_screenshots/Step_8_Arena_Gregory_Item_Transfer.png)

> Der Spieler übergibt das Objekt an Gregory.

**Dialog:**

`Protagonist:`
```text
*Du stellst das Objekt ab*
"Von Bertram."
```

<br>

`Gregory:`
```text
*Gregory mustert dich*
"..."
"Schwer genug."
```

```text
"Er hätte selbst kommen können."
"..."
"Aber er schickt lieber andere."
```

```text
*Gregory reicht dir das Geld*
"Das ist alles."
```

<br>

`Protagonist:`
```text
"..."
"Spricht nicht viel."
"Passt zur Arena."
```

Der Spieler hat jetzt das Geld von Gregory bekommen, dass er dann zu Bertram bringen soll.

---

### Schritt 9: Rückkehr zu Bertram
![Schritt 9 – Rückkehr zu Bertram](../village_blockout_screenshots/Step_9_Arena_To_Armory_Shop.png)

> Der Spieler kehrt mit dem Geld zum Ausrüstungsladen zurück.

Fokus:
- Wiederholter Weg  
- Gefühl von Fortschritt  

---

### Schritt 10: Schwertreparatur starten
![Schritt 10 – Schwert Reparatur](../village_blockout_screenshots/Step_10_Armory_Shop_Sword_Repair.png)

> Bertram beginnt mit der Reparatur des Schwertes.

**Dialog:**

`Bertram:`
```text
*Bertram nimmt das Geld*
"Gut."
```

```text
*Bertram schaut sich das Schwert nochmal genauer an*
"Ein Tag."
"Mehr nicht."
"..."
"Danach ist es wieder brauchbar."
"Nicht neu."
"Aber ehrlich."
```

<br>

`Protagonist (intern):`
```text
"..."
"Reicht."
```

Die Quest ist damit abgeschlossen und das Schwert kann nach einem Tageszyklus abgeholt werden.

---

## 7.1 Quest-Abzweigungen – Bertram Quest (Geldentscheidung)

Nach der Übergabe des Geldes von Gregory erhält der Protagonist eine für ihn **große Menge Geld**.  
An diesem Punkt öffnet sich eine bewusste Entscheidungsmöglichkeit, die **Charakterentwicklung**, **Beziehungen** und **zukünftige Spielverläufe** beeinflusst.

Diese Abzweigungen sind **optional**, aber haben spürbare Konsequenzen.

---

### Ausgangspunkt

- Der Spieler besitzt:
  - Das Geld von Gregory
- Bertra, besitzt:
  - Das rostige Schwert
- Ursprüngliches Quest-Ziel:
  - Geld zu Bertram zurückbringen

Ab hier ist der Spieler **nicht gezwungen**, Bertram sofort aufzusuchen.

---

### Abzweigung A: Geld zu Bertram bringen

**Beschreibung:**
- Der Spieler kehrt direkt zum Ausrüstungsladen zurück
- Übergibt das Geld wie vereinbart

**Konsequenzen:**
- Quest wird regulär abgeschlossen
- Bertram repariert das Schwert
- Beziehungsstatus zu Bertram:
  - Kann sich zu *Wohlgesonnen* entwickeln
- Keine negativen Effekte

**Charakterart:**
- Pflichtbewusst  
- Verlässlich  
- Konfliktvermeidend  

---

### Abzweigung B: Geld verstecken

**Beschreibung:**
- Der Spieler kehrt nach Hause zurück
- Versteckt das Geld unter dem Bett

**Konsequenzen:**
- Quest bleibt offen
- Bertram wartet
- Spätere Dialoge können sich verändern
- Optional:
  - Innerer Monolog über Angst, Sicherheit, Misstrauen

**Charakterart:**
- Unsicher  
- Vorsichtig  
- Misstrauisch gegenüber Autoritäten  

---

### Abzweigung C: Geld verschenken

**Beschreibung:**
- Der Spieler verschenkt Teile oder das gesamte Geld an zufällige Dorfbewohner

**Mögliche Ziele:**
- Einfache NPCs
- Bedürftige Dorfbewohner
- Händler oder Reisende

**Konsequenzen:**
- Beziehungsstatus zu einzelnen NPCs verbessert sich
- Geld fehlt für Bertram
- Bertram reagiert später ablehnend oder verärgert
- Quest kann:
  - Verzögert
  - Teurer
  - Oder nur teilweise abgeschlossen werden

**Charakterart:**
- Großzügig  
- Idealistisch
- Kurzsichtig oder bewusst selbstlos  

---

### Abzweigung D: Geld ausgeben

**Beschreibung:**
- Der Spieler nutzt das Geld für:
  - Essen im Gasthaus
  - Kleine Dienstleistungen
  - Komfort

**Konsequenzen:**
- Temporäre Vorteile
- Kein Geld für Bertram
- Quest verzögert sich
- Bertram reagiert negativ

**Charakterart:**
- Impulsiv  
- Gegenwartsorientiert  
- Konfliktscheu  

---

### Design-Hinweis

- Keine Abzweigung ist „richtig“ oder „falsch“
- Jede Entscheidung beeinflusst:
  - Beziehungen
  - Dialoge
  - Spielerwahrnehmung der Welt
- Konsequenzen treten **nicht sofort**, sondern **zeitversetzt** auf

---

## 8. Asset-Übersicht mit Referenzen & Beschreibung

### 8.1 Häuser
![Haus Blockout](../village_blockout_reference_assets/house_reference.png)
**Source:** https://www.artstation.com/marketplace/p/xYJX/wooden-hut-with-interior

- **Stil:** Holz, einstöckig
- **Maße:**
  - Breite: 8 Meter
  - Länge: 6.5 Meter
  - Höhe: 8 Meter
- **Funktion:** Worldbuilding, NPC-Wohnorte

---

### 8.2 Ausrüstungsladen
![Ausrüstungsladen Blockout](../village_blockout_reference_assets/armor_shop_reference.webp)
**Source:** https://www.behance.net/gallery/23946963/Medieval-Blacksmith-Shop-Lowpoly-game-asset

- **Maße:**
  - Breite: 8 Meter
  - Länge: 4 Meter
  - Höhe: 5 Meter
- **Typ:** Offener Stand
- **Sichtbare Assets:**
  - Waffen an Wänden
  - Amboss  
- **Gameplay:** Kaufen, Reparieren, Quests
- **NPCs**: Bertram
- **Hinweis:** Das Referenz Bild entspricht nicht genau wie der Ausrüstungsladen aussehen soll, aber es gibt schonmal eine gute Übersicht wie es Aussehen kann. Die Form des Ladens sollte sich eher an das Blockout orientieren.

---

### 8.3 Kampfarena
![Arena Blockout](../village_blockout_reference_assets/arena_reference.jpg)
**Source:** https://www.turbosquid.com/3d-models/medieval-wooden-fight-arena-pbr-model-2359589

- **Struktur:** Offener Platz mit Holzzaun  
- **Maße**:
  - Ein Kreis mit einem durchmesser von 40 Metern
- **Nutzung:**  
  - Training
  - Turniere
- **Gameplay:**
  - Trainieren für Stats verbessern
- **NPCs:** Gregory & Wachen
- **Hinweis:** In dieser Referenz in dem Assets sieht man noch Sitzplätze die sollen nicht da sein. Es ist eine komplett `offene` kleine Kampf-/Trainingsarena für die Wachen, also die Arena besteht **nur** aus dem Holzzaun.

---

### 8.4 Gasthaus
![Gasthaus Blockout](../village_blockout_reference_assets/inn_reference.webp)
**Source:** https://assetstore.unity.com/packages/3d/environments/fantasy/pb-sword-shield-inn-63205

- **Maße:**
  - Breite: 35 meter
  - Länge 25 Meter
  - Höhe 24 Meter
- **Größe:** Größer als Wohnhäuser
- **Etagen:** EG, 1G, 2G
- **Gameplay:**
  - Schlafen
  - Dialoge 
  - Story-Events
- **NPCs:** Dorfbewohner, Fahrende Händler, Söldner
- **Hinweis:** Die Verlinkung ist zu dem Game Asset aus dem referenz Bild. Es ist ein kostenloses Asset aus dem Unity Asset Store, kann verwendet werden wenn die Zeit nicht mehr reicht.

---

### 8.5 Straße
![Straße Blockout](../village_blockout_reference_assets/dirt_road_reference.webp)
**Source:** https://www.cgtrader.com/3d-models/scanned/various/16k-dry-dirt-road-material-n14-12-meters

- **Material:** Erde
- **Maße:**
  - Länge: 300 Meter (Einmal durch das ganze Dorf)
  - Breite: 25 Meter
- **Funktion:**
  - Navigation
  - Händlerbewegung

---

### 8.6 Zaun & Tore
![Zaun Blockout](../village_blockout_reference_assets/wall_gate_reference.jpg)
**Source:** https://www.youtube.com/watch?v=QwPD6GkaJCA

- **Material:** Holz
- **Zaun Höhe:** 30 Meter
- **Gameplay:**
  - Tag-/Nacht-Zyklus
  - Tore nachts geschlossen
- **Hinweis:** Das Youtube Video zeigt wie man die Zäune und Tore machen kann.

---

### Hinweise zu den Assets

#### Referenzbilder:
Zu allen Referenzbildern gibt es einen Link zu der Website aus dem ich das Bild runtergeladen habe. Die Assets *müssen nicht* genauso aussehen. Sie dienen als Orientierung zur Modellierung/Texturierung der Assets.

#### Maße:
In den Asset-Referenzen sind die Maßen angegeben die das Asset **haben** soll. Die Maße müssen aber nicht zu 100% einstimmen mit dem fertigem Modell, es dient lediglich als Orienterung.

---

## 9. Asset-Priorisierung

| Priorität | Asset | Begründung |
|-----------|-------|------------|
| 1 | Haus | Startpunkt wenn man das Zimmer verlässt |
| 2 | Aurüstungsladen | Zweiter Ort in der Game-User-Story |
| 3 | Kampfarena | Dritter Ort der Game-User-Story |
| 4 | Straße | Atmosphäre/Worldbuilding |
| 5 | Zaun + Tore | Gibt dem Dorf nochmals eine "größere" Bedeutung |
| 6 | Gasthaus | Nice to have aber nicht wichtig für die Game-User-Story |