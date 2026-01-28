using UnityEngine;

[CreateAssetMenu(
    fileName = "QuestStatusIsCondition",
    menuName = "Scriptable Objects/Quests/Conditions/QuestStatusIsCondition")]
public class QuestStatusIsCondition : QuestCondition
{
    [Header("Quest")]
    [Tooltip("Optional. If set, this condition checks this specific quest.")]
    [SerializeField] private QuestDefinition questDefinition;

    [Tooltip("Optional override. If set, takes precedence over QuestDefinition + context quest.")]
    [SerializeField] private string questIdOverride;

    [Header("Status")]
    [Tooltip("Required quest status")]
    [SerializeField] private QuestStatus requiredStatus = QuestStatus.Active;

    public override bool Evaluate(QuestContext context)
    {
        string questId = ResolveQuestId(context);

        // Prefer QuestLog state (single source of truth)
        if (!string.IsNullOrWhiteSpace(questId) && context.QuestLog != null)
        {
            QuestState state = context.QuestLog.GetStateOrDefault(questId);
            return state.Status == requiredStatus;
        }

        // Fallback to context state
        return context.QuestState.Status == requiredStatus;
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
