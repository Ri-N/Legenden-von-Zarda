
using UnityEngine;

/// <summary>
/// Dialogue outcome that sets or clears a quest flag via QuestLog.
/// Bridge from the dialogue system (DialogueOutcomeAction) into the quest system.
/// </summary>
[CreateAssetMenu(fileName = "SetQuestFlagOutcome", menuName = "Dialogue/Outcomes/SetQuestFlagOutcome")]
public class SetQuestFlagOutcome : DialogueOutcomeAction
{
    [Header("Quest")]
    [Tooltip("Optional. If set, this outcome affects this specific quest.")]
    [SerializeField] private QuestDefinition questDefinition;

    [Tooltip("Optional override. If set, takes precedence over QuestDefinition.")]
    [SerializeField] private string questIdOverride;

    [Header("Flag")]
    [SerializeField] private QuestFlag flag;

    [Tooltip("Value to set for the flag.")]
    [SerializeField] private bool value = true;

    public override void Execute(DialogueOutcomeContext ctx)
    {
        QuestLog questLog = FindFirstObjectByType<QuestLog>();
        if (questLog == null)
        {
            Debug.LogError($"[{nameof(SetQuestFlagOutcome)}] No {nameof(QuestLog)} found in the scene.", this);
            return;
        }

        string questId = ResolveQuestId();
        if (string.IsNullOrWhiteSpace(questId))
        {
            Debug.LogError($"[{nameof(SetQuestFlagOutcome)}] Could not resolve questId. Assign a QuestDefinition or questIdOverride.", this);
            return;
        }

        questLog.SetFlag(questId, flag, value, ctx.Source as Object);
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
