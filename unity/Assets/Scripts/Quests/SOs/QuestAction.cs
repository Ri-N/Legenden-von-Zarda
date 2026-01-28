using UnityEngine;

/// <summary>
/// Base class for quest actions. Actions are data-driven side effects executed when
/// a quest step is entered/completed or when a dialogue outcome triggers quest progress.
/// </summary>
public abstract class QuestAction : ScriptableObject
{
    /// <summary>
    /// Executes this action in the given context.
    /// </summary>
    public abstract void Execute(QuestContext context);
}
