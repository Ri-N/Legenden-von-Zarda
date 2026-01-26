using System;
using System.Collections;

using UnityEngine;
using UnityEngine.UI;

public class PlayerInventoryController : MonoBehaviour, IUIElementController, IBlockable
{
    public static PlayerInventoryController Instance { get; private set; }

    [Header("Grid Layout")]
    [Min(1)]
    [SerializeField] private int columns = 6;

    [Min(1)]
    [SerializeField] private int rows = 4;

    [Header("UI References")]
    [Tooltip("Parent transform that has a GridLayoutGroup where slots will be spawned.")]
    [SerializeField] private Transform gridParent;

    [SerializeField] private Slot slotPrefab;

    [Tooltip("Optional: CanvasGroup on the inventory root for show/hide without disabling the GameObject.")]
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Startup")]
    [SerializeField] private bool startHidden = true;

    [Header("Inventory Data (runtime)")]
    [Tooltip("One entry per slot index (runtime state).")]
    [SerializeField] private InventoryEntry[] slots;

    [Header("Selection")]
    [SerializeField] private int selectedIndex = -1;

    /// <summary>Raised when a slot is clicked/selected.</summary>
    public event Action<int, InventoryEntry> SelectedChanged;

    private Slot[] spawnedSlots;

    public int SlotCount => columns * rows;
    public int SelectedIndex => selectedIndex;
    public InventoryEntry SelectedEntry =>
        (selectedIndex >= 0 && slots != null && selectedIndex < slots.Length) ? slots[selectedIndex] : null;

    // IUIElementController
    public UIElement Element => UIElement.Inventory;

    private bool isRegistered;
    private bool isBlocked;

    private void TryRegister()
    {
        if (isRegistered) return;
        if (UIController.Instance == null) return;

        UIController.Instance.Register(this);
        isRegistered = true;
    }

    private void TryUnregister()
    {
        if (!isRegistered) return;
        if (UIController.Instance == null) { isRegistered = false; return; }

        UIController.Instance.Unregister(this);
        isRegistered = false;
    }

    private IEnumerator WaitForUIControllerThenRegister()
    {
        // Wait until UIController exists (e.g., if it is spawned/loaded after this UI).
        while (UIController.Instance == null)
            yield return null;

        TryRegister();
    }

    private void OnValidate()
    {
        EnsureSlotArraySize();
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (gridParent == null)
        {
            Debug.LogError("[PlayerInventoryController] gridParent not set.", this);
            enabled = false;
            return;
        }

        if (slotPrefab == null)
        {
            Debug.LogError("[PlayerInventoryController] slotPrefab not set.", this);
            enabled = false;
            return;
        }

        EnsureSlotArraySize();
        BuildGrid();
        RefreshAll();
    }

    private void Reset()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void OnEnable()
    {
        TryRegister();
        if (!isRegistered)
            StartCoroutine(WaitForUIControllerThenRegister());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        TryUnregister();
    }

    private void Start()
    {
        if (startHidden)
            SetHidden(true);
    }

    /// <summary>
    /// Ensures the backing array has exactly SlotCount elements.
    /// </summary>
    private void EnsureSlotArraySize()
    {
        int target = Mathf.Max(1, SlotCount);

        if (slots == null || slots.Length != target)
        {
            Array.Resize(ref slots, target);

#if UNITY_EDITOR
            // Mark dirty so inspector updates when editing in the editor
            if (!Application.isPlaying)
                UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
    }

    private void BuildGrid()
    {
        // Clear existing children (safe when entering Play Mode multiple times)
        for (int i = gridParent.childCount - 1; i >= 0; i--)
        {
            Destroy(gridParent.GetChild(i).gameObject);
        }

        spawnedSlots = new Slot[SlotCount];

        for (int i = 0; i < SlotCount; i++)
        {
            Slot slot = Instantiate(slotPrefab, gridParent);
            slot.Init(i);
            slot.Clicked += OnSlotClicked;

            spawnedSlots[i] = slot;
        }
    }

    private void OnApplicationQuit()
    {
        Instance = null;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;

        if (spawnedSlots == null) return;

        foreach (var slot in spawnedSlots)
        {
            if (slot != null)
                slot.Clicked -= OnSlotClicked;
        }
    }

    private void OnSlotClicked(int index)
    {
        selectedIndex = index;

        InventoryEntry entry = (slots != null && index >= 0 && index < slots.Length) ? slots[index] : null;
        SelectedChanged?.Invoke(index, entry);
    }

    /// <summary>
    /// Updates all slot icons from the current data array.
    /// </summary>
    public void RefreshAll()
    {
        if (spawnedSlots == null || slots == null) return;

        int count = Mathf.Min(spawnedSlots.Length, slots.Length);

        for (int i = 0; i < count; i++)
            RefreshSlot(i);

        // Clear any extra spawned slots if mismatch (shouldn't happen)
        for (int i = count; i < spawnedSlots.Length; i++)
            spawnedSlots[i]?.Clear();
    }

    /// <summary>
    /// Updates a single slot's icon from the current data array.
    /// </summary>
    public void RefreshSlot(int index)
    {
        if (spawnedSlots == null || slots == null) return;
        if (index < 0 || index >= spawnedSlots.Length) return;

        Slot uiSlot = spawnedSlots[index];
        if (uiSlot == null) return;

        InventoryEntry entry = (index >= 0 && index < slots.Length) ? slots[index] : null;

        if (entry == null || entry.IsEmpty || entry.Item == null || entry.Item.Icon == null)
        {
            uiSlot.Clear();
            return;
        }

        uiSlot.SetIcon(entry.Item.Icon);
    }

    /// <summary>
    /// Helper to set/replace an entry at runtime (useful for debugging/tests).
    /// </summary>
    public void SetEntry(int index, InventoryEntry entry)
    {
        EnsureSlotArraySize();

        if (index < 0 || index >= slots.Length) return;

        slots[index] = entry;
        RefreshSlot(index);
    }

    /// <summary>
    /// Adds an item with an explicit amount.
    /// 1) Fills existing stacks of the same item
    /// 2) Places remaining amount into empty slots
    /// Returns false if not all items fit.
    /// </summary>
    public bool TryAddToFirstEmpty(ItemDefinition item, int amount)
    {
        if (item == null) return false;
        if (amount <= 0) return false;

        EnsureSlotArraySize();

        int remaining = amount;

        // 1) Fill existing stacks first
        for (int i = 0; i < slots.Length && remaining > 0; i++)
        {
            InventoryEntry e = slots[i];
            if (e == null || e.IsEmpty) continue;
            if (e.Item != item) continue;

            remaining = e.Add(remaining);
            RefreshSlot(i);
        }

        // 2) Use empty slots
        for (int i = 0; i < slots.Length && remaining > 0; i++)
        {
            InventoryEntry e = slots[i];
            if (e != null && !e.IsEmpty) continue;

            InventoryEntry newEntry = new InventoryEntry();
            newEntry.Set(item, remaining);

            // newEntry.Amount is clamped by max stack (inside InventoryEntry.Validate)
            int placed = newEntry.Amount;
            remaining -= placed;

            slots[i] = newEntry;
            RefreshSlot(i);
        }

        return remaining <= 0;
    }

    /// <summary>
    /// Called by UIController to show/hide this UI element.
    /// IMPORTANT: Do not disable the GameObject here; otherwise it can't receive show requests.
    /// </summary>
    public void SetHidden(bool hidden)
    {
        // If this UI is blocked (e.g., dialogue open), force it to stay hidden.
        if (!hidden && isBlocked)
            hidden = true;

        // Prefer CanvasGroup toggling (recommended)
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup != null)
        {
            canvasGroup.alpha = hidden ? 0f : 1f;
            canvasGroup.interactable = !hidden;
            canvasGroup.blocksRaycasts = !hidden;
        }
        else
        {
            // Fallback: keep this object active, hide visuals by toggling children
            for (int i = 0; i < transform.childCount; i++)
                transform.GetChild(i).gameObject.SetActive(!hidden);
        }

        if (!hidden)
        {
            RefreshAll();
            selectedIndex = -1;
        }
    }

    public void RequestClose()
    {
        UIController.RequestHide(UIElement.Inventory);
    }

    public void SetBlocked(bool blocked)
    {
        isBlocked = blocked;

        if (isBlocked)
        {
            UIController.RequestHide(UIElement.Inventory);
            SetHidden(true);
        }
    }
}
