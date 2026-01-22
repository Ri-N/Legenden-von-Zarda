using UnityEngine;

public class SwordContainer : InteractableBase, IDialogueTrigger
{
    [SerializeField] private DialogueController dialogueController;
    [SerializeField] private DialogueText dialogueText;

    public DialogueController DialogueController => dialogueController;

    public DialogueText DialogueText => dialogueText;

    public override void Interact()
    {
        TriggerDialogue(dialogueText);
    }

    public void TriggerDialogue(DialogueText dialogueText)
    {
        dialogueController.StartDialogue(dialogueText, this);
    }
}
