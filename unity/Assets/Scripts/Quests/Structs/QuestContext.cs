using UnityEngine;

/// <summary>
/// Context object passed to quest conditions and actions.
/// Provides controlled access to runtime systems without tight coupling.
/// </summary>
public struct QuestContext
{
    /// <summary>
    /// Quest runtime manager holding all quest states.
    /// </summary>
    public QuestLog QuestLog { get; }

    /// <summary>
    /// Definition of the quest currently being evaluated/executed.
    /// </summary>
    public QuestDefinition QuestDefinition { get; }

    /// <summary>
    /// Current mutable state of the quest.
    /// </summary>
    public QuestState QuestState { get; }

    /// <summary>
    /// Optional reference to the player GameObject.
    /// </summary>
    public GameObject Player { get; }

    /// <summary>
    /// Optional source object that triggered the evaluation
    /// (e.g. NPC, Interactable, Dialogue).
    /// </summary>
    public Object Source { get; }

    /// <summary>
    /// Creates a new quest context.
    /// </summary>
    public QuestContext(
        QuestLog questLog,
        QuestDefinition questDefinition,
        QuestState questState,
        Player player = null,
        Object source = null)
    {
        QuestLog = questLog;
        QuestDefinition = questDefinition;
        QuestState = questState;
        Player = player.gameObject;
        Source = source;
    }
}
