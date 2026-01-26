
using System.Collections.Generic;

using UnityEngine;

/// <summary>
/// Holds a set of dialogue rules and selects the first matching DialogueText
/// based on quest conditions and current game state.
/// Used by NPCs to route to the correct dialogue depending on quest progress.
/// </summary>
[CreateAssetMenu(fileName = "ConditionalDialogueSet", menuName = "Scriptable Objects/Dialogue/ConditionalDialogueSet")]
public class ConditionalDialogueSet : ScriptableObject
{
    [Tooltip("Dialogue rules are evaluated in order. The first matching rule is used.")]
    public List<DialogueRule> rules = new();

    /// <summary>
    /// Returns the first DialogueText whose conditions evaluate to true.
    /// Returns null if no rule matches.
    /// </summary>
    public DialogueText Resolve(QuestContext context)
    {
        if (rules == null || rules.Count == 0)
            return null;

        for (int i = 0; i < rules.Count; i++)
        {
            DialogueRule rule = rules[i];
            if (rule == null)
                continue;

            if (rule.Matches(context))
                return rule.dialogue;
        }

        return null;
    }
}
