using UnityEngine;

[CreateAssetMenu(fileName = "ItemDefinition", menuName = "Inventory/Item Definition")]
public class ItemDefinition : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private string id;
    [SerializeField] private string displayName;

    [Header("Presentation")]
    [SerializeField] private Sprite icon;

    [Header("Gameplay")]
    [SerializeField] private ItemType itemType = ItemType.Misc;

    [Tooltip("Maximum amount that can be stacked in one slot. Use 1 for non-stackable items (e.g., weapons).")]
    [Min(1)]
    [SerializeField] private int maxStack = 1;

    public string Id => id;
    public string DisplayName => displayName;
    public Sprite Icon => icon;
    public ItemType Type => itemType;
    public int MaxStack => maxStack;

    private void OnValidate()
    {
        // Keep data sane in the inspector
        if (string.IsNullOrWhiteSpace(id))
        {
            // Optional: auto-fill id from asset name (you can delete this if you prefer manual ids)
            id = name.Replace(" ", "_").ToLowerInvariant();
        }

        if (maxStack < 1) maxStack = 1;
    }
}
