using UnityEngine;

/// <summary>
/// Dialogue outcome that sets a quest step via QuestLog.
/// Bridge from the dialogue system (DialogueOutcomeAction) into the quest system.
/// </summary>
[CreateAssetMenu(fileName = "SetQuestStepOutcome", menuName = "Dialogue/Outcomes/SetQuestStepOutcome")]
public class SetQuestStepOutcome : DialogueOutcomeAction
{
    [Header("Quest")]
    [Tooltip("Optional. If set, this outcome affects this specific quest.")]
    [SerializeField] private QuestDefinition questDefinition;

    [Tooltip("Optional override. If set, takes precedence over QuestDefinition.")]
    [SerializeField] private string questIdOverride;

    [Header("Step")]
    [Tooltip("Step id to set on the quest.")]
    [SerializeField] private string stepId;

    [Header("Behavior")]
    [Tooltip("If true, executes the new step's onEnterActions.")]
    [SerializeField] private bool runEnterActions = true;

    [Tooltip("If true, QuestLog will immediately evaluate if the new step can auto-complete.")]
    [SerializeField] private bool evaluateAfter = true;

    public override void Execute(DialogueOutcomeContext ctx)
    {
        QuestLog questLog = FindFirstObjectByType<QuestLog>();
        if (questLog == null)
        {
            Debug.LogError($"[{nameof(SetQuestStepOutcome)}] No {nameof(QuestLog)} found in the scene.", this);
            return;
        }

        string questId = ResolveQuestId();
        if (string.IsNullOrWhiteSpace(questId))
        {
            Debug.LogError($"[{nameof(SetQuestStepOutcome)}] Could not resolve questId. Assign a QuestDefinition or questIdOverride.", this);
            return;
        }

        if (string.IsNullOrWhiteSpace(stepId))
        {
            Debug.LogError($"[{nameof(SetQuestStepOutcome)}] stepId is empty.", this);
            return;
        }

        questLog.SetStep(questId, stepId, ctx.Source as Object, runEnterActions, evaluateAfter);
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
