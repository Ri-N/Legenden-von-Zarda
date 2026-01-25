using UnityEngine;

[System.Serializable]
public class ChoiceOption
{
    public string optionText;

    [Tooltip("Dialogue node to continue with after this choice is selected (can be null to end the conversation).")]
    public DialogueText next;

    [Tooltip("Optional side-effects that run immediately when this choice is selected (e.g., set flags, grant items, start quests).")]
    public DialogueOutcomeAction[] outcomes;
}
