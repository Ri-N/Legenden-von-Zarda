using UnityEngine;

[CreateAssetMenu(fileName = "InventoryEntry", menuName = "Inventory/Inventory Entry")]
public class InventoryEntry : ScriptableObject
{
    [SerializeField] private ItemDefinition item;
    [Min(0)]
    [SerializeField] private int amount;

    public ItemDefinition Item => item;
    public int Amount => amount;

    public void Set(ItemDefinition newItem, int newAmount)
    {
        item = newItem;
        amount = Mathf.Max(0, newAmount);
        ClampToStack();
    }

    public int Add(int toAdd)
    {
        if (item == null || toAdd <= 0) return toAdd;

        int before = amount;
        amount += toAdd;
        int clamped = Mathf.Min(amount, item.MaxStack);
        int overflow = amount - clamped;

        amount = clamped;
        return overflow;
    }

    public int Remove(int toRemove)
    {
        if (toRemove <= 0) return 0;

        int removed = Mathf.Min(amount, toRemove);
        amount -= removed;

        if (amount <= 0)
        {
            amount = 0;
            item = null;
        }

        return removed;
    }

    public bool IsEmpty => item == null || amount <= 0;

    private void OnValidate()
    {
        if (amount < 0) amount = 0;
        ClampToStack();

        // If there is no item, amount should be 0
        if (item == null && amount != 0) amount = 0;
    }

    private void ClampToStack()
    {
        if (item == null) return;
        if (amount > item.MaxStack) amount = item.MaxStack;
    }
}
