using UnityEngine;

[CreateAssetMenu(
    fileName = "StartQuestAction",
    menuName = "Scriptable Objects/Quests/Actions/StartQuestAction")]
public class StartQuestAction : QuestAction
{
    [Header("Quest")]
    [Tooltip("Optional. If set, this action starts this specific quest.")]
    [SerializeField] private QuestDefinition questDefinition;

    [Tooltip("Optional override. If set, takes precedence over QuestDefinition + context quest.")]
    [SerializeField] private string questIdOverride;

    [Tooltip("If true, starting an already started quest is considered OK (no-op).")]
    [SerializeField] private bool allowIfAlreadyStarted = true;

    public override void Execute(QuestContext context)
    {
        if (context.QuestLog == null)
        {
            Debug.LogError($"[{nameof(StartQuestAction)}] QuestLog is null in context.", this);
            return;
        }

        string questId = ResolveQuestId(context);
        if (string.IsNullOrWhiteSpace(questId))
        {
            Debug.LogError($"[{nameof(StartQuestAction)}] Could not resolve questId.", this);
            return;
        }

        QuestState state = context.QuestLog.GetStateOrDefault(questId);
        if (!allowIfAlreadyStarted && state.Status != QuestStatus.NotStarted)
        {
            Debug.LogWarning($"[{nameof(StartQuestAction)}] Quest '{questId}' already started (status: {state.Status}).", this);
            return;
        }

        // StartQuest returns false if already started/completed.
        context.QuestLog.StartQuest(questId, context.Source);
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
