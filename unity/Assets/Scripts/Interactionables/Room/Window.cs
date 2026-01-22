using UnityEngine;

public class Window : InteractableBase, IDialogueTrigger, IInteractionConstraint
{
    [SerializeField] private DialogueController dialogueController;
    [SerializeField] private DialogueText dialogueText;

    [Header("One-sided interaction")]
    [SerializeField] private InteractionSide allowedSide = InteractionSide.BackOnly;

    [Tooltip("Reference transform whose forward defines the 'front' side. If not set, this Window's transform is used.")]
    [SerializeField] private Transform sideReference;

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

    public bool CanInteract(Vector3 playerWorldPosition)
    {
        if (allowedSide == InteractionSide.Both)
        {
            return true;
        }

        Transform t = sideReference != null ? sideReference : transform;

        float dot = Vector3.Dot(t.forward, playerWorldPosition - t.position);
        bool isFront = dot >= 0f;

        return allowedSide == InteractionSide.FrontOnly ? isFront : !isFront;
    }
}
