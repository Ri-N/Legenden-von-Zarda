# Git-Konventionen

Dieses Dokument beschreibt, wie wir Branches und Commit-Messages im Projekt _Legenden von Zarda_ benennen und wie die Husky-Hooks das technisch enforce’n.

---

## Branch-Namen

**Schema:**

```text
<type>/<scope>/<beschreibung>
```

**Beispiele:**

- feature/gdd/main-quest-01
- content/asset/sword-statue
- feature/material/sword-metal-roughness
- feature/prefab/chest-loot-system
- fix/level/castle-navmesh
- techdebt/build/husky-setup
- techdebt/ui/cleanup-hud-prefabs

**type**

Art der Arbeit auf dem Branch:

- `feature` – neue Features
- `fix` – Bugfixes
- `content` – neue Inhalte (Assets, Level-Inhalte, Story-Texte, GDD-Content)
- `techdebt` – Refactorings, Aufräumen, technische Schulden

**scope**

Bereich im Projekt, orientiert an unseren Rollen:

- `gdd` – Game Design Doc, Story, Konzepte
- `asset` – 3D-Modelle allgemein
- `material` – Texturen, Shader, Materials
- `prefab` – Unity-Prefabs mit Logik
- `level` – Level-/Scene-Bau
- `ui` – Menüs, HUD
- `input` – Input-Controller & Steuerung
- `build` – Builds, Pipeline, Projekt-Infra

**beschreibung**

- `kebab-case`, nur Kleinbuchstaben, Zahlen, `-`, `_`, `.`
- sollte kurz, aber aussagekräftig sein

**Technisches Enforcement (Husky `pre-commit`)**

Der `pre-commit`-Hook (`scripts/validate-branch.sh`) checkt den Branch-Namen mit:

```regex
^(feature|fix|content|techdebt)/(gdd|asset|material|prefab|level|ui|input|build)/[a-z0-9._-]+$
```

Commits sind nur erlaubt, wenn der aktuelle Branch diesem Muster entspricht.

---

## Commit-Messages

Wir verwenden eine vereinfachte Variante von **Conventional Commits**, angepasst an unsere Bereiche.

**Schema:**

```text
<type>(<scope>): <Beschreibung>
```

**type**

- `feat` – neues Feature
- `fix` – Bugfix
- `docs` – Dokumentation (z.B. GDD, Markdown, Rollen/Prozess-Dokus)
- `refactor` – Code-/Strukturänderungen ohne neues Verhalten
- `chore` – Projekt-Setup, Konfiguration, Kleinkram
- `test` – Tests
- `build` – Build-/CI-/Pipeline-Anpassungen

**scope** (optional, aber empfohlen)

- `gdd`, `asset`, `material`, `prefab`, `level`, `ui`, `input`, `build`, `project`
- `project` nutzen wir für Projekt-Setup und allgemeine Repo-/Tooling-Änderungen (z.B. Husky, npm, Configs)

**Beispiele:**

```text
feat(asset): Add sword statue lowpoly
feat(material): Add metal shader for sword
feat(prefab): Connect sword damage script
feat(level): Place sword statue in hub scene
feat(ui): Add health bar to HUD

fix(level): Fix navmesh near castle gate
fix(input): Invert y-axis option

docs(gdd): Update level 1 story outline
docs(project): Document git conventions

chore(project): Setup Husky and commit hooks
build(build): Add Unity CI build pipeline
```

**Technisches Enforcement (Husky `commit-msg`)**

Der `commit-msg`-Hook (`scripts/validate-commit-msg.mjs`) prüft die Commit-Message mit:

```regex
^(feat|fix|docs|refactor|chore|test|build)(\((gdd|asset|material|prefab|level|ui|input|build|project)\))?: .+$
```

---

## Husky-Hooks im Überblick

Aktuell sind folgende Hooks aktiv:

- **`.husky/pre-commit`**

  - ruft `scripts/validate-branch.sh` auf
  - sorgt dafür, dass Branch-Namen unserem Schema entsprechen

- **`.husky/commit-msg`**
  - ruft `scripts/validate-commit-msg.mjs` auf
  - sorgt dafür, dass Commit-Messages unserem Schema entsprechen

Die Hooks werden automatisch nach `npm install` eingerichtet (über das `prepare`-Script in der `package.json`).
