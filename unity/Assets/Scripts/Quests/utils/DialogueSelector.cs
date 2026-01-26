using UnityEngine;

/// <summary>
/// Small helper to select a dialogue from a ConditionalDialogueSet.
/// Keeps NPC scripts clean: they can ask the selector for the current DialogueText
/// based on the active quest context.
/// </summary>
public static class DialogueSelector
{
    /// <summary>
    /// Resolves a dialogue from the given set. If no rule matches, returns fallback.
    /// </summary>
    public static DialogueText Resolve(ConditionalDialogueSet set, QuestContext context, DialogueText fallback = null)
    {
        if (set == null)
            return fallback;

        DialogueText resolved = set.Resolve(context);
        return resolved != null ? resolved : fallback;
    }

    /// <summary>
    /// Convenience overload: resolves a dialogue for a specific quest within the QuestLog.
    /// If the quest is unknown or not present, the state defaults to NotStarted.
    /// </summary>
    public static DialogueText Resolve(
        ConditionalDialogueSet set,
        QuestLog questLog,
        QuestDefinition questDefinition,
        Player player = null,
        Object source = null,
        DialogueText fallback = null)
    {
        if (set == null)
            return fallback;

        QuestState state = QuestState.CreateNew();
        if (questLog != null && questDefinition != null)
            state = questLog.GetStateOrDefault(questDefinition.questId);

        QuestContext ctx = new QuestContext(questLog, questDefinition, state, player, source);
        DialogueText resolved = set.Resolve(ctx);
        return resolved != null ? resolved : fallback;
    }
}
