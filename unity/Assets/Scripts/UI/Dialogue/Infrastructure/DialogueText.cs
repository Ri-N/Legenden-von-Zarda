using UnityEngine;

[CreateAssetMenu(fileName = "DialogueText", menuName = "Dialogue/DialogueText")]
public class DialogueText : ScriptableObject
{
    [Header("Paragraphs / Lines")]
    public DialogueLine[] lines;

    [Header("Choices (optional)")]
    public ChoiceOption[] choices;

    [System.Serializable]
    public struct DialogueLine
    {
        public string speakerName;

        [TextArea(5, 10)]
        public string text;
    }
}
