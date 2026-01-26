using UnityEngine;

public class SwordContainer : InteractableBase, IDialogueTrigger, IInteractionConstraint, IConsumable
{

    [SerializeField] private DialogueController dialogueController;

    [SerializeField] private DialogueText[] dialogueSequence;

    [SerializeField] private PlayerArea[] allowedAreas;

    [SerializeField, Min(0)] private int dialogueIndex;
    private bool isConsumed;

    public void MarkConsumed()
    {
        isConsumed = true;
    }

    public DialogueController DialogueController => dialogueController;

    public DialogueText DialogueText => GetCurrentDialogue();

    public override void Interact()
    {
        if (isConsumed)
            return;

        DialogueText next = GetCurrentDialogue();
        if (next == null)
        {
            Debug.LogWarning($"[{nameof(SwordContainer)}] No dialogue assigned on '{name}'.", this);
            return;
        }

        TriggerDialogue(next);
        AdvanceDialogueIndex();
    }

    public void TriggerDialogue(DialogueText dialogueText)
    {
        dialogueController.StartDialogue(dialogueText, this);
    }

    public bool CanInteract(in InteractionContext ctx)
    {
        if (isConsumed)
            return false;

        return InteractionConstraintUtil.IsAreaAllowed(allowedAreas, ctx.PlayerArea);
    }
    private DialogueText GetCurrentDialogue()
    {
        if (isConsumed)
            return null;

        if (dialogueSequence == null || dialogueSequence.Length == 0)
            return null;

        int idx = Mathf.Clamp(dialogueIndex, 0, dialogueSequence.Length - 1);
        return dialogueSequence[idx];
    }

    private void AdvanceDialogueIndex()
    {
        if (dialogueSequence == null || dialogueSequence.Length == 0)
            return;

        dialogueIndex = Mathf.Min(dialogueIndex + 1, dialogueSequence.Length - 1);
    }
}
