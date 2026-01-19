using UnityEngine;

public class Desk : InteractableBase, IMonologueTrigger
{
    [SerializeField] private DialogueController dialogueController;
    [SerializeField] private DialogueText dialogueText;

    public DialogueController DialogueController => dialogueController;
    public DialogueText DialogueText => dialogueText;

    public override void Interact()
    {
        TriggerMonologue(DialogueText);
    }

    public void TriggerMonologue(DialogueText dialogueText)
    {
        DialogueController.DisplayNextParagraph(dialogueText);
    }
}