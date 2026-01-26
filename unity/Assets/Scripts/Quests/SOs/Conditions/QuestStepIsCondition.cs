using UnityEngine;

[CreateAssetMenu(
    fileName = "QuestStepIsCondition",
    menuName = "Scriptable Objects/Quests/Conditions/QuestStepIsCondition")]
public class QuestStepIsCondition : QuestCondition
{
    [Header("Quest")]
    [Tooltip("Optional. If set, this condition checks this specific quest.")]
    [SerializeField] private QuestDefinition questDefinition;

    [Tooltip("Optional override. If set, takes precedence over QuestDefinition + context quest.")]
    [SerializeField] private string questIdOverride;

    [Header("Step")]
    [Tooltip("Step id that must match the current quest step")]
    [SerializeField] private string requiredStepId;

    public override bool Evaluate(QuestContext context)
    {
        if (string.IsNullOrWhiteSpace(requiredStepId))
            return false;

        string questId = ResolveQuestId(context);

        // Prefer QuestLog state (single source of truth)
        if (!string.IsNullOrWhiteSpace(questId) && context.QuestLog != null)
        {
            QuestState state = context.QuestLog.GetStateOrDefault(questId);
            return string.Equals(state.CurrentStepId, requiredStepId, System.StringComparison.Ordinal);
        }

        // Fallback to context state
        return string.Equals(context.QuestState.CurrentStepId, requiredStepId, System.StringComparison.Ordinal);
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
