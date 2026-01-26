using System;
using System.Collections.Generic;

using UnityEngine;

/// <summary>
/// Data-only definition of a single quest step.
/// Describes what the step is, how it can be completed,
/// and what happens when it is entered or completed.
/// </summary>
[Serializable]
public struct QuestStepDefinition
{
    [Header("Identification")]
    [Tooltip("Unique step id within the quest (e.g. S8_DeliverHeavyToGregory)")]
    public string stepId;

    [Header("Presentation")]
    public string title;
    [TextArea]
    public string description;

    [Header("Completion Conditions")]
    [Tooltip("All conditions must be fulfilled to complete this step")]
    public List<QuestCondition> completionConditions;

    [Header("Actions")]
    [Tooltip("Executed once when the step becomes active")]
    public List<QuestAction> onEnterActions;

    [Tooltip("Executed once when the step is completed")]
    public List<QuestAction> onCompleteActions;

    [Header("Flow")]
    [Tooltip("Id of the next step. Leave empty if this is the final step or branching is handled by actions.")]
    public string nextStepId;

    /// <summary>
    /// Returns true if all completion conditions are fulfilled.
    /// If no conditions are defined, the step is considered immediately completable.
    /// </summary>
    public bool AreCompletionConditionsMet(QuestContext context)
    {
        if (completionConditions == null || completionConditions.Count == 0)
            return true;

        for (int i = 0; i < completionConditions.Count; i++)
        {
            QuestCondition condition = completionConditions[i];
            if (condition == null)
                continue;

            if (!condition.Evaluate(context))
                return false;
        }

        return true;
    }
}
