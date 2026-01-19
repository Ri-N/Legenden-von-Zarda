using UnityEngine;

public class SwordContainer : InteractableBase, IMonologueTrigger
{
    [SerializeField] private DialogueController dialogueController;
    [SerializeField] private DialogueText dialogueText;

    public DialogueController DialogueController => dialogueController;

    public DialogueText DialogueText => dialogueText;

    public override void Interact()
    {
        TriggerMonologue(dialogueText);
    }

    public void TriggerMonologue(DialogueText dialogueText)
    {
        dialogueController.DisplayNextParagraph(dialogueText);
    }
}