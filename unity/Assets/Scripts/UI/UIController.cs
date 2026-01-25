using System;

using UnityEngine;

public class UIController : MonoBehaviour
{
    public static UIController Instance { get; private set; }

    public enum UIElement
    {
        PlayerInteract,
        Dialogue,
        Inventory,
        All
    }

    public event Action<UIElement, bool> HideUIRequested;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public static void RequestHide(UIElement element)
    {
        if (Instance == null)
        {
            Debug.LogWarning($"UIController.RequestHide({element}): No UIController Instance in the scene.");
            return;
        }

        Instance.HideUIRequested?.Invoke(element, true);
    }

    public static void RequestShow(UIElement element)
    {
        if (Instance == null)
        {
            Debug.LogWarning($"UIController.RequestShow({element}): No UIController Instance in the scene.");
            return;
        }

        Instance.HideUIRequested?.Invoke(element, false);
    }

    public static void RequestSetHidden(UIElement element, bool hide)
    {
        if (hide)
            RequestHide(element);
        else
            RequestShow(element);
    }
}
