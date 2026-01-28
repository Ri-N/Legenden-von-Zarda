using UnityEngine;

[CreateAssetMenu(
    fileName = "SetQuestFlagAction",
    menuName = "Scriptable Objects/Quests/Actions/SetQuestFlagAction")]
public class SetQuestFlagAction : QuestAction
{
    [Header("Quest")]
    [Tooltip("Optional. If set, this action affects this specific quest.")]
    [SerializeField] private QuestDefinition questDefinition;

    [Tooltip("Optional override. If set, takes precedence over QuestDefinition + context quest.")]
    [SerializeField] private string questIdOverride;

    [Header("Flag")]
    [SerializeField] private QuestFlag flag;

    [Tooltip("Value to set for the flag.")]
    [SerializeField] private bool value = true;

    public override void Execute(QuestContext context)
    {
        if (context.QuestLog == null)
        {
            Debug.LogError($"[{nameof(SetQuestFlagAction)}] QuestLog is null in context.", this);
            return;
        }

        string questId = ResolveQuestId(context);
        if (string.IsNullOrWhiteSpace(questId))
        {
            Debug.LogError($"[{nameof(SetQuestFlagAction)}] Could not resolve questId.", this);
            return;
        }

        context.QuestLog.SetFlag(questId, flag, value, context.Source);
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
