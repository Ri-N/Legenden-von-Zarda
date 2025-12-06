#!/usr/bin/env sh

BRANCH="$(git rev-parse --abbrev-ref HEAD)"

BRANCH_REGEX='^(feature|fix|content|techdebt)/(gdd|asset|material|prefab|level|ui|input|build)/[a-z0-9._-]+$'

if echo "$BRANCH" | grep -Eq "$BRANCH_REGEX"; then
  exit 0
else
  echo "Ungültiger Branch-Name: $BRANCH"
  echo ""
  echo "Erwartetes Format:"
  echo "   <type>/<scope>/<beschreibung>[-#issue]"
  echo ""
  echo "type:  feature | fix | content | techdebt"
  echo "scope: gdd | asset | material | prefab | level | ui | input | build"
  echo ""
  echo "Beispiele:"
  echo "   feature/gdd/main-quest"
  echo "   content/asset/sword-statue"
  echo "   feature/prefab/chest-loot-system"
  echo "   fix/level/castle-navmesh"
  exit 1
fi