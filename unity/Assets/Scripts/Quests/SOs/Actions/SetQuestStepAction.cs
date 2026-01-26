using UnityEngine;

[CreateAssetMenu(
    fileName = "SetQuestStepAction",
    menuName = "Scriptable Objects/Quests/Actions/SetQuestStepAction")]
public class SetQuestStepAction : QuestAction
{
    [Header("Quest")]
    [Tooltip("Optional. If set, this action affects this specific quest.")]
    [SerializeField] private QuestDefinition questDefinition;

    [Tooltip("Optional override. If set, takes precedence over QuestDefinition + context quest.")]
    [SerializeField] private string questIdOverride;

    [Header("Step")]
    [Tooltip("Step id to set on the quest.")]
    [SerializeField] private string stepId;

    [Header("Behavior")]
    [Tooltip("If true, executes the new step's onEnterActions.")]
    [SerializeField] private bool runEnterActions = true;

    [Tooltip("If true, QuestLog will immediately evaluate if the new step can auto-complete.")]
    [SerializeField] private bool evaluateAfter = true;

    public override void Execute(QuestContext context)
    {
        if (context.QuestLog == null)
        {
            Debug.LogError($"[{nameof(SetQuestStepAction)}] QuestLog is null in context.", this);
            return;
        }

        string questId = ResolveQuestId(context);
        if (string.IsNullOrWhiteSpace(questId))
        {
            Debug.LogError($"[{nameof(SetQuestStepAction)}] Could not resolve questId.", this);
            return;
        }

        if (string.IsNullOrWhiteSpace(stepId))
        {
            Debug.LogError($"[{nameof(SetQuestStepAction)}] stepId is empty.", this);
            return;
        }

        context.QuestLog.SetStep(questId, stepId, context.Source, runEnterActions, evaluateAfter);
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
