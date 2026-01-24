using UnityEngine;

[CreateAssetMenu(fileName = "CollectItemOutcome", menuName = "Dialogue/Outcomes/CollectItemOutcome")]
public class CollectItemOutcome : DialogueOutcomeAction
{

    public override void Execute(DialogueOutcomeContext ctx)
    {
        PlayerInventoryController inventory = PlayerInventoryController.Instance;
        if (inventory == null)
        {
            Debug.LogError($"[{nameof(CollectItemOutcome)}] Could not resolve a {nameof(PlayerInventoryController)} in the scene. Assign it in the outcome or ensure one exists.", this);
            return;
        }

        if (ctx.Source is not IHasInventoryItem hasItem)
        {
            Debug.LogError($"[{nameof(CollectItemOutcome)}] Outcome requires ctx.Source to implement {nameof(IHasInventoryItem)}.", this);
            return;
        }

        if (hasItem.ItemDefinition == null)
        {
            Debug.LogWarning($"[{nameof(CollectItemOutcome)}] Source provided a null {nameof(ItemDefinition)}.", this);
            return;
        }

        if (hasItem.ItemAmount <= 0)
        {
            Debug.LogError($"[{nameof(CollectItemOutcome)}] Source provided an invalid Amount ({hasItem.ItemAmount}).", this);
            return;
        }

        bool added = inventory.TryAddToFirstEmpty(hasItem.ItemDefinition, hasItem.ItemAmount);
        if (!added)
        {
            Debug.LogWarning($"[{nameof(CollectItemOutcome)}] Inventory is full. Could not add '{hasItem.ItemDefinition.name}' x{hasItem.ItemAmount}.", this);
            return;
        }

        inventory.RefreshAll();
    }
}
