using UnityEngine;

public class Desk : InteractableBase, IDialogueTrigger, IInteractionConstraint
{
    [SerializeField] private DialogueController dialogueController;
    [SerializeField] private DialogueText dialogueText;

    [SerializeField] private PlayerArea[] allowedAreas;

    public DialogueController DialogueController => dialogueController;
    public DialogueText DialogueText => dialogueText;

    public override void Interact()
    {
        TriggerDialogue(DialogueText);
    }

    public void TriggerDialogue(DialogueText dialogueText)
    {
        DialogueController.StartDialogue(dialogueText, this);
    }

    public bool CanInteract(in InteractionContext ctx)
    {
        return InteractionConstraintUtil.IsAreaAllowed(allowedAreas, ctx.PlayerArea);
    }
}
