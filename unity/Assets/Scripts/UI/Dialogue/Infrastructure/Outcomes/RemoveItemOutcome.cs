using UnityEngine;

[CreateAssetMenu(fileName = "RemoveItemOutcome", menuName = "Dialogue/Outcomes/RemoveItemOutcome")]
public class RemoveItemOutcome : DialogueOutcomeAction
{
    [Header("Item to remove")]
    [SerializeField] private ItemDefinition itemDefinition;

    [Min(1)]
    [SerializeField] private int amount = 1;

    [Tooltip("If true, the outcome fails if not enough items are present.")]
    [SerializeField] private bool requireFullAmount = true;

    public override void Execute(DialogueOutcomeContext ctx)
    {
        PlayerInventoryController inventory = PlayerInventoryController.Instance;
        if (inventory == null)
        {
            Debug.LogError($"[{nameof(RemoveItemOutcome)}] Could not resolve a {nameof(PlayerInventoryController)} in the scene. Ensure one exists.", this);
            return;
        }

        if (itemDefinition == null)
        {
            Debug.LogError($"[{nameof(RemoveItemOutcome)}] No {nameof(ItemDefinition)} assigned on this outcome asset.", this);
            return;
        }

        if (amount <= 0)
        {
            Debug.LogError($"[{nameof(RemoveItemOutcome)}] Invalid amount '{amount}'. Amount must be >= 1.", this);
            return;
        }

        bool ok = inventory.TryRemove(itemDefinition, amount, requireFullAmount);
        if (!ok && requireFullAmount)
        {
            Debug.LogWarning($"[{nameof(RemoveItemOutcome)}] Not enough '{itemDefinition.name}' in inventory. Requested {amount}.", this);
            return;
        }

        inventory.RefreshAll();
    }
}
