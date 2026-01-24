using UnityEngine;

public class Window : InteractableBase, IDialogueTrigger, IInteractionConstraint
{
    [SerializeField] private DialogueController dialogueController;
    [SerializeField] private DialogueText dialogueText;

    [Header("One-sided interaction")]
    [SerializeField] private InteractionSide allowedSide = InteractionSide.BackOnly;

    [Tooltip("Reference transform whose forward defines the 'front' side. If not set, this Window's transform is used.")]
    [SerializeField] private Transform sideReference;

    [Header("Area constraint")]
    [Tooltip("If set, the window can only be interacted with in these player areas. Leave empty to allow in all areas.")]
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
        if (!InteractionConstraintUtil.IsAreaAllowed(allowedAreas, ctx.PlayerArea))
        {
            return false;
        }

        Transform t = sideReference != null ? sideReference : transform;
        return InteractionConstraintUtil.IsSideAllowed(allowedSide, t, ctx.PlayerWorldPosition);
    }
}
