using System;
using System.Collections.Generic;

using UnityEngine;

public class UIController : MonoBehaviour
{
    [Header("Game Input")]
    [SerializeField] private GameInput gameInput;

    public static UIController Instance { get; private set; }

    private readonly List<IUIElementController> registeredElements = new();

    private readonly Dictionary<UIElement, bool> hiddenState = new();

    private bool IsHidden(UIElement element)
    {
        return !hiddenState.TryGetValue(element, out bool hidden) || hidden;
    }

    public void Register(IUIElementController element)
    {
        if (element == null) return;
        if (!registeredElements.Contains(element))
            registeredElements.Add(element);
    }

    public void Unregister(IUIElementController element)
    {
        if (element == null) return;
        registeredElements.Remove(element);
    }

    private void ApplyHidden(UIElement element, bool hide)
    {
        // Apply to UI elements via interface
        for (int i = 0; i < registeredElements.Count; i++)
        {
            IUIElementController ui = registeredElements[i];
            if (ui == null) continue;

            if (element == UIElement.All || ui.Element == element)
            {
                ui.SetHidden(hide);

                // Persist state for each affected element
                hiddenState[ui.Element] = hide;
            }
        }

        // Persist state for the specific element (unless it's a broadcast)
        if (element != UIElement.All)
            hiddenState[element] = hide;

        // Optional/legacy event for non-UI listeners
        HideUIRequested?.Invoke(element, hide);

        // Inventory-specific high-level events
        if (element == UIElement.Inventory)
        {
            if (hide) InventoryClosed?.Invoke();
            else InventoryOpened?.Invoke();
        }

        // Menu-specific high-level events
        if (element == UIElement.Menu)
        {
            if (hide) MenuClosed?.Invoke();
            else MenuOpened?.Invoke();
        }
    }

    public event Action<UIElement, bool> HideUIRequested;
    public event Action InventoryOpened;
    public event Action InventoryClosed;
    public event Action MenuOpened;
    public event Action MenuClosed;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        if (gameInput == null)
        {
            Debug.LogError("[UIController] GameInput not set.", this);
            enabled = false;
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        gameInput.OnOpenInventory += GameInput_OnOpenInventory;
        gameInput.OnOpenMenu += GameInput_OnOpenMenu;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;

        if (gameInput != null)
        {
            gameInput.OnOpenInventory -= GameInput_OnOpenInventory;
            gameInput.OnOpenMenu -= GameInput_OnOpenMenu;
        }
    }

    public static void RequestHide(UIElement element)
    {
        if (Instance == null)
        {
            Debug.LogWarning($"UIController.RequestHide({element}): No UIController Instance in the scene.");
            return;
        }

        Instance.ApplyHidden(element, true);
    }

    public static void RequestShow(UIElement element)
    {
        if (Instance == null)
        {
            Debug.LogWarning($"UIController.RequestShow({element}): No UIController Instance in the scene.");
            return;
        }

        Instance.ApplyHidden(element, false);
    }

    public static void RequestSetHidden(UIElement element, bool hide)
    {
        if (hide)
            RequestHide(element);
        else
            RequestShow(element);
    }

    private void GameInput_OnOpenInventory(object sender, EventArgs e)
    {
        bool currentlyHidden = IsHidden(UIElement.Inventory);
        RequestSetHidden(UIElement.Inventory, !currentlyHidden);
    }

    private void GameInput_OnOpenMenu(object sender, EventArgs e)
    {
        bool currentlyHidden = IsHidden(UIElement.Menu);
        RequestSetHidden(UIElement.Menu, !currentlyHidden);
    }

    // UnityEvent-friendly wrappers (static methods do not show up in Button OnClick)
    public void HideMenu() => RequestHide(UIElement.Menu);

}
