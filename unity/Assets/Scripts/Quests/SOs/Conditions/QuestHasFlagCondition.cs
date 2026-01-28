using UnityEngine;

[CreateAssetMenu(
    fileName = "QuestHasFlagCondition",
    menuName = "Scriptable Objects/Quests/Conditions/QuestHasFlagCondition")]
public class QuestHasFlagCondition : QuestCondition
{
    [Header("Quest")]
    [Tooltip("Optional. If set, this condition checks this specific quest.")]
    [SerializeField] private QuestDefinition questDefinition;

    [Tooltip("Optional override. If set, takes precedence over QuestDefinition + context quest.")]
    [SerializeField] private string questIdOverride;

    [Header("Flag")]
    [SerializeField] private QuestFlag flag;

    [Tooltip("If true: the flag must be set. If false: the flag must NOT be set.")]
    [SerializeField] private bool requiredValue = true;

    public override bool Evaluate(QuestContext context)
    {
        string questId = ResolveQuestId(context);

        // If we can't resolve a quest id, we can only fall back to the context state.
        if (string.IsNullOrWhiteSpace(questId))
            return EvaluateAgainstState(context.QuestState);

        // Prefer reading the live state from QuestLog (source of truth).
        if (context.QuestLog != null)
        {
            QuestState state = context.QuestLog.GetStateOrDefault(questId);
            return EvaluateAgainstState(state);
        }

        // Fallback: if no QuestLog was provided, use the context state.
        return EvaluateAgainstState(context.QuestState);
    }

    private bool EvaluateAgainstState(QuestState state)
    {
        bool has = state.HasFlag(flag.ToString());
        return requiredValue ? has : !has;
    }

    private string ResolveQuestId(QuestContext context)
    {
        if (!string.IsNullOrWhiteSpace(questIdOverride))
            return questIdOverride;

        if (questDefinition != null && !string.IsNullOrWhiteSpace(questDefinition.questId))
            return questDefinition.questId;

        if (context.QuestDefinition != null && !string.IsNullOrWhiteSpace(context.QuestDefinition.questId))
            return context.QuestDefinition.questId;

        return string.Empty;
    }
}
