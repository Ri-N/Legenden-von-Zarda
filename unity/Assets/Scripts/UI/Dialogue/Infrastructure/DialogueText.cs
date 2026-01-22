using UnityEngine;

[CreateAssetMenu(fileName = "DialogueText", menuName = "Dialogue/DialogueText")]
public class DialogueText : ScriptableObject
{
    public string speakerName;

    [TextArea(5, 10)]
    public string[] paragraphs;
    public ChoiceOption[] choices;
}
