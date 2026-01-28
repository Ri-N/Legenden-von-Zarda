using UnityEngine;

[CreateAssetMenu(
    fileName = "CompleteQuestAction",
    menuName = "Scriptable Objects/Quests/Actions/CompleteQuestAction")]
public class CompleteQuestAction : QuestAction
{
    [Header("Quest")]
    [Tooltip("Optional. If set, this action completes this specific quest.")]
    [SerializeField] private QuestDefinition questDefinition;

    [Tooltip("Optional override. If set, takes precedence over QuestDefinition + context quest.")]
    [SerializeField] private string questIdOverride;

    public override void Execute(QuestContext context)
    {
        if (context.QuestLog == null)
        {
            Debug.LogError($"[{nameof(CompleteQuestAction)}] QuestLog is null in context.", this);
            return;
        }

        string questId = ResolveQuestId(context);
        if (string.IsNullOrWhiteSpace(questId))
        {
            Debug.LogError($"[{nameof(CompleteQuestAction)}] Could not resolve questId.", this);
            return;
        }

        context.QuestLog.CompleteQuest(questId, context.Source);
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
