using UnityEngine;

public class Bed : NPCBase, IInteractionConstraint
{
    [Header("Interaction Constraint")]
    [SerializeField] private PlayerArea[] allowedAreas;

    // NPCBase already implements InteractableBase + IDialogueTrigger and resolves dialogues via
    // ConditionalDialogueSet / QuestDefinition / fallbackDialogue.
    // Configure those fields on the Bed in the inspector.

    public bool CanInteract(in InteractionContext ctx)
    {
        return InteractionConstraintUtil.IsAreaAllowed(allowedAreas, ctx.PlayerArea);
    }
}
