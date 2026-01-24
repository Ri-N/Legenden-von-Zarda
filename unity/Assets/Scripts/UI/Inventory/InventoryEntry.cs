using UnityEngine;

/// <summary>
/// Runtime inventory slot state: item + amount.
/// Serializable so it can live directly inside a PlayerInventoryController slots array.
/// </summary>
[System.Serializable]
public class InventoryEntry : ISerializationCallbackReceiver
{
    [SerializeField] private ItemDefinition item;

    [Min(0)]
    [SerializeField] private int amount;

    public ItemDefinition Item => item;
    public int Amount => amount;

    public bool IsEmpty => item == null || amount <= 0;

    public void Set(ItemDefinition newItem, int newAmount)
    {
        item = newItem;
        amount = Mathf.Max(0, newAmount);
        Validate();
    }

    /// <summary>
    /// Adds up to the max stack size. Returns overflow that didn’t fit.
    /// </summary>
    public int Add(int toAdd)
    {
        if (item == null || toAdd <= 0) return toAdd;

        amount += toAdd;

        int max = Mathf.Max(1, item.MaxStack);
        int clamped = Mathf.Min(amount, max);
        int overflow = amount - clamped;

        amount = clamped;
        return overflow;
    }

    /// <summary>
    /// Removes up to the requested amount. Returns the actually removed count.
    /// Clears item if empty afterwards.
    /// </summary>
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

    private void Validate()
    {
        if (amount < 0) amount = 0;

        if (item == null)
        {
            if (amount != 0) amount = 0;
            return;
        }

        int max = Mathf.Max(1, item.MaxStack);
        if (amount > max) amount = max;
    }

    public void OnBeforeSerialize() => Validate();
    public void OnAfterDeserialize() => Validate();
}
