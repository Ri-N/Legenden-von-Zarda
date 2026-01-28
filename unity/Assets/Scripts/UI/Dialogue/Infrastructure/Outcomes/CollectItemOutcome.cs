using UnityEngine;

[CreateAssetMenu(fileName = "CollectItemOutcome", menuName = "Dialogue/Outcomes/CollectItemOutcome")]
public class CollectItemOutcome : DialogueOutcomeAction
{
    [Header("Item to collect")]
    [SerializeField] private ItemDefinition itemDefinition;

    [Min(1)]
    [SerializeField] private int amount = 1;

    public override void Execute(DialogueOutcomeContext ctx)
    {
        PlayerInventoryController inventory = PlayerInventoryController.Instance;
        if (inventory == null)
        {
            Debug.LogError($"[{nameof(CollectItemOutcome)}] Could not resolve a {nameof(PlayerInventoryController)} in the scene. Assign it in the outcome or ensure one exists.", this);
            return;
        }

        if (itemDefinition == null)
        {
            Debug.LogError($"[{nameof(CollectItemOutcome)}] No {nameof(ItemDefinition)} assigned on this outcome asset.", this);
            return;
        }

        if (amount <= 0)
        {
            Debug.LogError($"[{nameof(CollectItemOutcome)}] Invalid amount '{amount}'. Amount must be >= 1.", this);
            return;
        }

        bool added = inventory.TryAddToFirstEmpty(itemDefinition, amount);
        if (!added)
        {
            Debug.LogWarning($"[{nameof(CollectItemOutcome)}] Inventory is full. Could not add '{itemDefinition.name}' x{amount}.", this);
            return;
        }

        if (ctx.Source is IConsumable consumable)
            consumable.MarkConsumed();

        inventory.RefreshAll();
    }
}
