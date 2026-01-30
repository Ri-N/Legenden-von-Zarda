using System.Collections.Generic;

using UnityEngine;

/// <summary>
/// Data-only definition of a quest.
/// Describes steps, identifiers and optional meta information.
/// Runtime progress is stored in QuestState (via QuestLog).
/// </summary>
[CreateAssetMenu(fileName = "QuestDefinition", menuName = "Scriptable Objects/Quests/QuestDefinition")]
public class QuestDefinition : ScriptableObject
{
    [Header("Identification")]
    [Tooltip("Unique quest id (e.g. bertram_sword_repair)")]
    public string questId;

    [Header("Presentation")]
    public string title;
    [TextArea]
    public string description;

    [Header("Steps")]
    [Tooltip("Ordered list of quest steps. Step ids must be unique within the quest.")]
    public List<QuestStepDefinition> steps = new();

    /// <summary>
    /// Returns the step definition with the given id or null if not found.
    /// </summary>
    public QuestStepDefinition? GetStep(string stepId)
    {
        if (string.IsNullOrEmpty(stepId))
            return default;

        for (int i = 0; i < steps.Count; i++)
        {
            if (steps[i].stepId == stepId)
                return steps[i];
        }

        return default;
    }

    /// <summary>
    /// Returns the first step of the quest (used on quest start).
    /// </summary>
    public QuestStepDefinition? GetFirstStep()
    {
        if (steps == null || steps.Count == 0)
            return default;

        return steps[0];
    }

    /// <summary>
    /// Validation helper to catch common setup errors early.
    /// </summary>
    public bool IsValid(out string error)
    {
        if (string.IsNullOrWhiteSpace(questId))
        {
            error = "QuestDefinition has no questId.";
            return false;
        }

        if (steps == null || steps.Count == 0)
        {
            error = $"Quest '{questId}' has no steps.";
            return false;
        }

        HashSet<string> ids = new();
        for (int i = 0; i < steps.Count; i++)
        {
            QuestStepDefinition step = steps[i];
            if (string.IsNullOrWhiteSpace(step.stepId))
            {
                error = $"Quest '{questId}' has a step without stepId.";
                return false;
            }

            if (!ids.Add(step.stepId))
            {
                error = $"Quest '{questId}' has duplicate stepId '{step.stepId}'.";
                return false;
            }
        }

        error = null;
        return true;
    }
}
