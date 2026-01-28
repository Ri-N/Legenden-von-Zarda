using UnityEngine;

/// <summary>
/// Dialogue outcome that starts a quest via QuestLog.
/// This is the bridge from the dialogue system (DialogueOutcomeAction) into the quest system.
/// </summary>
[CreateAssetMenu(fileName = "StartQuestOutcome", menuName = "Dialogue/Outcomes/StartQuestOutcome")]
public class StartQuestOutcome : DialogueOutcomeAction
{
    [Header("Quest")]
    [Tooltip("Optional. If set, this outcome starts this specific quest.")]
    [SerializeField] private QuestDefinition questDefinition;

    [Tooltip("Optional override. If set, takes precedence over QuestDefinition.")]
    [SerializeField] private string questIdOverride;

    [Tooltip("If true, starting an already started quest is considered OK (no-op).")]
    [SerializeField] private bool allowIfAlreadyStarted = true;

    public override void Execute(DialogueOutcomeContext ctx)
    {
        QuestLog questLog = FindFirstObjectByType<QuestLog>();
        if (questLog == null)
        {
            Debug.LogError($"[{nameof(StartQuestOutcome)}] No {nameof(QuestLog)} found in the scene.", this);
            return;
        }

        string questId = ResolveQuestId();
        if (string.IsNullOrWhiteSpace(questId))
        {
            Debug.LogError($"[{nameof(StartQuestOutcome)}] Could not resolve questId. Assign a QuestDefinition or questIdOverride.", this);
            return;
        }

        QuestState state = questLog.GetStateOrDefault(questId);
        if (!allowIfAlreadyStarted && state.Status != QuestStatus.NotStarted)
        {
            Debug.LogWarning($"[{nameof(StartQuestOutcome)}] Quest '{questId}' already started (status: {state.Status}).", this);
            return;
        }

        // StartQuest returns false if already started/completed.
        questLog.StartQuest(questId, ctx.Source as Object);
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
