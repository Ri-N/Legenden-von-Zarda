using System.Collections.Generic;

using UnityEngine;


[System.Serializable]
public class DialogueRule
{
    [Tooltip("All conditions must be fulfilled for this rule to match")]
    public List<QuestCondition> conditions = new();

    [Tooltip("Dialogue that will be used if all conditions match")]
    public DialogueText dialogue;

    public bool Matches(QuestContext context)
    {
        if (conditions == null || conditions.Count == 0)
            return true;

        for (int i = 0; i < conditions.Count; i++)
        {
            QuestCondition condition = conditions[i];
            if (condition == null)
                continue;

            if (!condition.Evaluate(context))
                return false;
        }

        return true;
    }
}
