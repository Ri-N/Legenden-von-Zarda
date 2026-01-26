using UnityEngine;

/// <summary>
/// Base class for quest conditions. Conditions are data-driven checks that can be
/// referenced by QuestStepDefinition completion conditions and dialogue gating.
/// </summary>
public abstract class QuestCondition : ScriptableObject
{
    /// <summary>
    /// Returns true if this condition is fulfilled in the given context.
    /// </summary>
    public abstract bool Evaluate(QuestContext context);
}
