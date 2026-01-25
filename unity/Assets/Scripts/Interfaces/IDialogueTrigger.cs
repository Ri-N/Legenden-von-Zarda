public interface IDialogueTrigger
{
    DialogueController DialogueController { get; }
    DialogueText DialogueText { get; }

    void TriggerDialogue(DialogueText dialogueText);
}
