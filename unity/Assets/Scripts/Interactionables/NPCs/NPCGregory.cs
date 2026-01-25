using UnityEngine;

public class NPCGregory : InteractableBase, IDialogueTrigger
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
        DialogueController.StartDialogue(dialogueText, this);
    }
}
