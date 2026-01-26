using UnityEngine;

/// <summary>
/// Base class for NPC interaction that supports quest-driven dialogue routing via ConditionalDialogueSet.
/// 
/// This class starts dialogues directly via DialogueController.
/// </summary>
public class NPCBase : MonoBehaviour, IDialogueTrigger, IInteractable
{
    [Header("Dialogue")]
    [Tooltip("Dialogue controller used to start dialogues.")]
    [SerializeField] protected DialogueController dialogueController;

    [Tooltip("Optional fallback dialogue if no rule matches.")]
    [SerializeField] private DialogueText fallbackDialogue;

    [Tooltip("Optional quest-driven dialogue routing set.")]
    [SerializeField] private ConditionalDialogueSet dialogueSet;

    [Tooltip("Optional quest definition used as context for routing conditions.")]
    [SerializeField] private QuestDefinition questDefinition;

    [Header("Interaction")]
    [Tooltip("If true, interaction will do nothing when no dialogue could be resolved.")]
    [SerializeField] private bool blockIfNoDialogue = true;

    [Tooltip("If true, CanInteract() must return true for Interact() to request a dialogue.")]
    [SerializeField] private bool enforceCanInteract = true;

    protected QuestLog questLog;

    protected virtual void Awake()
    {
        // Cache QuestLog if present in the scene.
        questLog = FindFirstObjectByType<QuestLog>();

        if (dialogueController == null)
            dialogueController = FindFirstObjectByType<DialogueController>();
    }

    /// <summary>
    /// Whether this NPC can be interacted with right now.
    /// Override for special cases (e.g., Gregory locked before quest step).
    /// Default: true.
    /// </summary>
    public virtual bool CanInteract()
        => true;

    /// <summary>
    /// Optional overload if your interaction system passes the player.
    /// Default delegates to parameterless CanInteract().
    /// </summary>
    public virtual bool CanInteract(Player player)
        => CanInteract();

    /// <summary>
    /// Resolves the dialogue for this NPC based on the configured ConditionalDialogueSet.
    /// If no set is configured, returns the fallback dialogue.
    /// </summary>
    public virtual DialogueText ResolveDialogue()
    {
        // If no routing set is configured, just use fallback.
        if (dialogueSet == null)
            return fallbackDialogue;

        // Build context using QuestLog and questDefinition.
        Player player = Player.Instance;

        return DialogueSelector.Resolve(
            dialogueSet,
            questLog,
            questDefinition,
            player,
            this,
            fallbackDialogue);
    }

    /// <summary>
    /// Called by the interaction system.
    /// This overload supports setups where Interact() has no parameters.
    /// </summary>
    public virtual void Interact()
    {
        if (enforceCanInteract && !CanInteract())
            return;

        DialogueText dialogue = ResolveDialogue();
        if (dialogue == null && blockIfNoDialogue)
            return;

        TriggerDialogue(dialogue);
        OnDialogueRequested(dialogue);
    }

    /// <summary>
    /// Optional overload if your interaction system passes the player.
    /// </summary>
    public virtual void Interact(Player player)
    {
        if (enforceCanInteract && !CanInteract(player))
            return;

        DialogueText dialogue = ResolveDialogue();
        if (dialogue == null && blockIfNoDialogue)
            return;

        TriggerDialogue(dialogue);
        OnDialogueRequested(dialogue);
    }

    /// <summary>
    /// Hook for subclasses. Called after ResolveDialogue() succeeded (or returned null if not blocked).
    /// </summary>
    protected virtual void OnDialogueRequested(DialogueText dialogue)
    {
        // Intentionally empty.
    }

    public DialogueController DialogueController => dialogueController;

    public DialogueText DialogueText => ResolveDialogue();

    public virtual void TriggerDialogue(DialogueText dialogueText)
    {
        if (dialogueController == null)
        {
            Debug.LogError($"[{nameof(NPCBase)}] DialogueController is not assigned.", this);
            return;
        }

        if (dialogueText == null)
        {
            Debug.LogWarning($"[{nameof(NPCBase)}] No dialogue resolved (null).", this);
            return;
        }

        dialogueController.StartDialogue(dialogueText, this);
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public virtual string GetInteractText()
    {
        return "Sprechen";
    }
}
