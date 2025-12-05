# Rollen & Prozesse im Projekt

Dieses Dokument beschreibt die Rollen im Team und die wichtigsten Workflows zwischen ihnen.

## Rollen & Teammitglieder

```mermaid
flowchart LR
    subgraph Teammitglieder
        N[Noah]
        Na[Nalin]
        B[Baki]
        E[Emmi]
        Es[Esra]
    end

    subgraph Rollen
        GD[3D Game Designer\n(Game Design Doc,\nLevel Blockout, Styleguide,\nMockups, Material-Defs)]
        Mod[3D Modeller\n(Hard Surface Modeling)]
        Mat[3D Material Artist\n(Shader & Textures)]
        LB[Level Builder\n(Scene-Orga, Terrain,\nLighting, Executables, VFX)]
        IO[Unity IO Dev\n(GUI Menu & HUD,\nInput-Controller)]
        Pref[Unity Prefab Dev\n(Instantiable Assets\nwith Code)]
    end

    %% Zuordnung bevorzugter Verantwortlichkeiten
    Na --> GD

    B --> Mod

    E --> Mat

    Es --> LB

    N --> IO
    N --> Pref
```

---

## 2. High-Level-Spiel-Workflow (von Idee zu Build)

Hier ein Gesamtprozess, wie ein Feature/Level/Asset durchs Team fließt.

````markdown
## High-Level-Prozess: Von Game Design zu spielbarem Build

```mermaid
flowchart LR
    GD[3D Game Designer\n(Nalin)] -->|GDD, Styleguide,\nLevelkonzept| Mod[3D Modeller\n(Baki)]
    Mod -->|3D-Modelle (ohne Materialien)| Mat[3D Material Artist\n(Emmi)]
    Mat -->|fertige Meshes mit Texturen,\nShader-Setups| Pref[Unity Prefab Dev\n(Noah)]
    Pref -->|Prefabs mit Logik,\nKomponenten| LB[Level Builder\n(Esra)]
    LB -->|Szenen, Lighting,\nVFX, Builds| IO[Unity IO Dev\n(Noah)]
    IO -->|Menüs, HUD,\nInput-Konnektivität| Build[Spielbare Builds\n(PC/Export)]

    %% Rückkopplungsschleifen
    Build -->|Feedback zu Spielgefühl,\nLesbarkeit, Performance| GD
```
````

**Interpretation (kurz):**

- **Game Designer (Nalin)**: definiert, _was_ gebaut wird (GDD, Styleguide, Level-Idee).
- **Modeller (Baki)**: erstellt die 3D-Geometrie.
- **Material Artist (Emmi)**: macht Texturen, Shader, Materials.
- **Prefab Dev (Noah)**: bringt Assets als wiederverwendbare Prefabs in Unity, inkl. Scripts.
- **Level Builder (Esra)**: baut Szenen, platziert Prefabs, kümmert sich um Lighting, Terrain, VFX, Exe-Builds.
- **IO Dev (Noah)**: verbindet das Ganze mit Menüs, HUD, Input und finalem Spielerlebnis.

---

## 3. Detaillierter Asset-Prozess (z.B. „Prop“)

Das passt sehr gut zu deinem Beispiel:

> Modeller entwirft 3D-Modell → Material Artist reviewt → Material Artist texturiert → usw.

````markdown
## Detailprozess: 3D-Asset-Pipeline (Prop)

```mermaid
sequenceDiagram
    participant GD as 3D Game Designer<br/>(Nalin)
    participant Mod as 3D Modeller<br/>(Baki)
    participant Mat as 3D Material Artist<br/>(Emmi)
    participant Pref as Unity Prefab Dev<br/>(Noah)
    participant LB as Level Builder<br/>(Esra)

    GD->>GD: Asset-Idee im Game Design Doc (GDD) definieren<br/>+ Styleguide/Materialvorgaben
    GD->>Mod: Asset-Anforderung erstellen<br/>(Beschreibung, Referenzen, Maße, LODs)

    Mod->>Mod: Blockout & Highpoly/Lowpoly Modeling
    Mod->>GD: Modell-Preview zur Abstimmung (Screenshots/Turntable)
    GD-->>Mod: Feedback (Form, Silhouette, Größe)

    Mod->>Mat: Finales Mesh mit UVs übergeben<br/>(inkl. Namenskonventionen/Ordnerstruktur)

    Mat->>Mat: Baking (Normal, AO, etc.)<br/>+ Texturing + Shader-Setup
    Mat->>Mod: Technischer Review (Fehler in UVs, Topologie?)
    Mod-->>Mat: Fixes bei Bedarf, neues Mesh liefern

    Mat->>Pref: Fertiges Asset-Paket übergeben<br/>(FBX/Blend, Texturen, Material-Settings)

    Pref->>Pref: Import nach Unity, Material-Zuweisung<br/>+ Prefab erstellen + Scripts/Components anhängen
    Pref->>LB: Prefab für Level freigeben (Dokumentation: Name, Purpose, Performance-Hinweise)

    LB->>LB: Platzierung im Level, Tests in der Szene<br/>(Lighting, Scale, Kollisionsprüfung)
    LB->>GD: Visuelle Abnahme im Level-Kontext

    GD-->>LB: Feedback zu Lesbarkeit/Gameplay-Impact
    LB->>Pref: ggf. Anpassungen an Prefab (Collider, LOD)
    Pref-->>Mat: ggf. Anpassungen an Material (Performance, Lesbarkeit)
```
````

---

## 4. Level-/Scene-Workflow

Hier eine Ansicht speziell für Level-Design und -Bau.

````markdown
## Detailprozess: Level-/Scene-Entwicklung

```mermaid
flowchart TB
    A[Game Design Doc<br/>(Levelziele, Core-Mechanics)]:::GD
    B[Level-Blockout<br/>(Whitebox)]:::GD
    C[Definition benötigter Assets<br/>(Props, Umgebung, Interactables)]:::GD
    D[3D Modeling & Materials<br/>(Modeller + Material Artist)]:::Art
    E[Prefab-Erstellung<br/>(Prefab Dev)]:::Code
    F[Level Assembly<br/>(Level Builder)]:::Level
    G[Gameplay-Tests<br/>(alle)]:::Test
    H[Balancing & Polishing<br/>(GD + LB + IO)]:::Level
    I[Build-Erstellung & Bereitstellung]:::Build

    A --> B --> C --> D --> E --> F --> G --> H --> I

    classDef GD fill:#fdd;
    classDef Art fill:#dfd;
    classDef Code fill:#ddf;
    classDef Level fill:#ffd;
    classDef Test fill:#eef;
    classDef Build fill:#fed;
```
````

---

## 5. UI / HUD / Input-Prozess (Unity IO Dev)

Damit der IO-Dev-Part sauber dokumentiert ist, noch ein Flow nur für Menüs/HUD:

````markdown
## Detailprozess: UI, HUD & Input

```mermaid
sequenceDiagram
    participant GD as 3D Game Designer<br/>(Nalin)
    participant IO as Unity IO Dev<br/>(Noah)
    participant Pref as Unity Prefab Dev<br/>(Noah)
    participant LB as Level Builder<br/>(Esra)

    GD->>GD: UI-/HUD-Anforderungen im GDD definieren<br/>(Mockups, Wireframes, Zustände)
    GD->>IO: UI-Konzept + Mockups übergeben

    IO->>IO: UI-Struktur in Unity planen<br/>(Scenes, Canvas, Navigation)
    IO->>Pref: UI-Komponenten als Prefabs definieren<br/>(Buttons, Panels, HUD-Elements)
    Pref-->>IO: Fertige UI-Prefabs mit Scripts

    IO->>LB: UI/HUD in Szenen integrieren<br/>(z.B. HUD im Level, Menüs in separaten Scenes)
    LB->>IO: Rückmeldung zu Lesbarkeit, Platzierung

    IO->>IO: Input-Controller implementieren<br/>(Keyboard/Mouse/Controller Mapping)
    IO->>GD: Review von Bedienbarkeit & UX

    GD-->>IO: Feedback (Usability, Verständlichkeit)
    IO->>IO: Anpassungen & Feintuning
```
````
