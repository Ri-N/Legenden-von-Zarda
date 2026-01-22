using UnityEngine;

public class Door : InteractableBase, IDialogueTrigger, IHasTeleportPoints
{
    [SerializeField] private DialogueController dialogueController;

    [Header("Dialogue")]
    [Tooltip("Dialogue shown when the player is on the outside side of the door.")]
    [SerializeField] private DialogueText outsideDialogueText;
    public DialogueText DialogueText => outsideDialogueText;

    [Tooltip("Dialogue shown when the player is on the inside side of the door.")]
    [SerializeField] private DialogueText insideDialogueText;

    [Header("Side detection")]
    [Tooltip("Invert outside/inside if your model/axis is flipped.")]
    [SerializeField] private bool outsideIsBackSide = false;

    [Header("Teleport points")]
    [SerializeField] private Transform entryPoint;
    [SerializeField] private Transform exitPoint;

    public DialogueController DialogueController => dialogueController;

    // For your teleport context interface
    public Transform EntryPoint => entryPoint;
    public Transform ExitPoint => exitPoint;

    public override void Interact()
    {
        DialogueText chosen = IsPlayerOutsideSide() ? outsideDialogueText : insideDialogueText;

        if (chosen == null) chosen = outsideDialogueText != null ? outsideDialogueText : insideDialogueText;

        if (chosen != null)
        {
            TriggerDialogue(chosen);
        }
    }

    public void TriggerDialogue(DialogueText dialogueText)
    {
        // Start dialogue with context (this Door as interactor)
        dialogueController.StartDialogue(dialogueText, this);
    }

    private bool IsPlayerOutsideSide()
    {
        Vector3 playerPos = Player.Instance != null
            ? Player.Instance.transform.position
            : transform.position;

        if (entryPoint == null || exitPoint == null)
        {
            Debug.LogError($"Door '{name}': EntryPoint/ExitPoint not set. Cannot auto-detect side. Defaulting to outside.", this);
            return true;
        }

        // Define the separating plane between inside/outside as the midpoint between entry/exit.
        Vector3 planePoint = (entryPoint.position + exitPoint.position) * 0.5f;

        // Define outside normal from entry -> exit.
        Vector3 outsideNormal = (exitPoint.position - entryPoint.position).normalized;
        if (outsideNormal.sqrMagnitude < 0.0001f)
        {
            Debug.LogError($"Door '{name}': EntryPoint and ExitPoint are at the same position. Cannot auto-detect side. Defaulting to outside.", this);
            return true;
        }

        float dot = Vector3.Dot(outsideNormal, playerPos - planePoint);
        bool isOutside = dot >= 0f;

        return outsideIsBackSide ? !isOutside : isOutside;
    }
}
