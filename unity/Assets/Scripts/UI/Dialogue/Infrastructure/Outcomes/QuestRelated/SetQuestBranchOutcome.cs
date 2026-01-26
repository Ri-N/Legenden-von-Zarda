using UnityEngine;

/// <summary>
/// Dialogue outcome that sets a quest branch via QuestLog.
/// Used for branching decisions (e.g. bring money, hide money, donate, spend).
/// </summary>
[CreateAssetMenu(
    fileName = "SetQuestBranchOutcome",
    menuName = "Scriptable Objects/Dialogue/Outcomes/SetQuestBranchOutcome")]
public class SetQuestBranchOutcome : DialogueOutcomeAction
{
    [Header("Quest")]
    [Tooltip("Optional. If set, this outcome affects this specific quest.")]
    [SerializeField] private QuestDefinition questDefinition;

    [Tooltip("Optional override. If set, takes precedence over QuestDefinition.")]
    [SerializeField] private string questIdOverride;

    [Header("Branch")]
    [Tooltip("Branch value to set on the quest state.")]
    [SerializeField] private QuestBranch branch;

    public override void Execute(DialogueOutcomeContext ctx)
    {
        QuestLog questLog = FindFirstObjectByType<QuestLog>();
        if (questLog == null)
        {
            Debug.LogError($"[{nameof(SetQuestBranchOutcome)}] No {nameof(QuestLog)} found in the scene.", this);
            return;
        }

        string questId = ResolveQuestId();
        if (string.IsNullOrWhiteSpace(questId))
        {
            Debug.LogError($"[{nameof(SetQuestBranchOutcome)}] Could not resolve questId. Assign a QuestDefinition or questIdOverride.", this);
            return;
        }

        questLog.SetBranch(questId, branch);
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
