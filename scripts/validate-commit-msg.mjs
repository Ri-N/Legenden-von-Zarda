// scripts/validate-commit-msg.mjs
import { readFileSync } from "node:fs";

const commitMsgFile = process.argv[2];

if (!commitMsgFile) {
  console.error("Kein Commit-Message-File übergeben.");
  process.exit(1);
}

const message = readFileSync(commitMsgFile, "utf8").trim();

// Regex: Conventional Commit Style, angepasst an euer Projekt
const commitRegex =
  /^(feat|fix|docs|refactor|chore|test|build)(\((gdd|asset|material|prefab|level|ui|input|build|project)\))?: .+$/;

if (!commitRegex.test(message)) {
  console.error("Ungültige Commit-Message:");
  console.error(`   "${message}"`);
  console.error("");
  console.error("Erlaubtes Format:");
  console.error("   <type>(<scope>): <Beschreibung>");
  console.error("");
  console.error("Beispiele:");
  console.error("   feat(asset): Add sword statue lowpoly");
  console.error("   fix(level): Fix navmesh near castle gate");
  console.error("   docs(gdd): Update level 1 story outline");
  console.error("   feat(ui): Add pause menu");
  console.error("   chore(project): Setup Husky and commit hooks");
  process.exit(1);
}