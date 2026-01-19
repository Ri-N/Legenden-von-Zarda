public interface IMonologueTrigger
{
    DialogueController DialogueController { get; }
    DialogueText DialogueText { get; }

    void TriggerMonologue(DialogueText dialogueText);
}
