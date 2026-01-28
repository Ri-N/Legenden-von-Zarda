using UnityEngine;

/// <summary>
/// Dialogue outcome that completes a quest via QuestLog.
/// Bridge from the dialogue system (DialogueOutcomeAction) into the quest system.
/// </summary>
[CreateAssetMenu(fileName = "CompleteQuestOutcome", menuName = "Dialogue/Outcomes/CompleteQuestOutcome")]
public class CompleteQuestOutcome : DialogueOutcomeAction
{
    [Header("Quest")]
    [Tooltip("Optional. If set, this outcome completes this specific quest.")]
    [SerializeField] private QuestDefinition questDefinition;

    [Tooltip("Optional override. If set, takes precedence over QuestDefinition.")]
    [SerializeField] private string questIdOverride;

    public override void Execute(DialogueOutcomeContext ctx)
    {
        QuestLog questLog = FindFirstObjectByType<QuestLog>();
        if (questLog == null)
        {
            Debug.LogError($"[{nameof(CompleteQuestOutcome)}] No {nameof(QuestLog)} found in the scene.", this);
            return;
        }

        string questId = ResolveQuestId();
        if (string.IsNullOrWhiteSpace(questId))
        {
            Debug.LogError($"[{nameof(CompleteQuestOutcome)}] Could not resolve questId. Assign a QuestDefinition or questIdOverride.", this);
            return;
        }

        questLog.CompleteQuest(questId, ctx.Source as Object);
    }

    private string ResolveQuestId()
    {
        if (!string.IsNullOrWhiteSpace(questIdOverride))
            return questIdOverride;

        if (questDefinition != null && !string.IsNullOrWhiteSpace(questDefinition.questId))
            return questDefinition.questId;

        return string.Empty;
    }
}
