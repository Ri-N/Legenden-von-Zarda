using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

/// <summary>
/// Central place to block/unblock gameplay systems (movement, interaction, camera, etc.)
/// based on high-level events (e.g., Inventory opened/closed).
///
/// This controller keeps track of multiple active block reasons. A blockable is considered
/// blocked if at least one reason is active.
/// </summary>
public class GameBlockController : MonoBehaviour
{
    public static GameBlockController Instance { get; private set; }
    [Header("Targets")]
    [Tooltip("Gameplay systems to block (e.g., Player movement, GameInput gameplay, camera look).")]
    [SerializeField] private MonoBehaviour[] gameplayTargets;

    [Tooltip("UI systems to block for heavy blocks (e.g., Inventory UI, PlayerInteractUI prompt). NOTE: Inventory-open should NOT block this group.")]
    [SerializeField] private MonoBehaviour[] uiTargets;

    [Header("Debug")]
    [SerializeField] private bool logStateChanges;

    private readonly List<IBlockable> gameplayBlockables = new();
    private readonly List<IBlockable> uiBlockables = new();
    private readonly HashSet<BlockReason> activeReasons = new();

    private bool isSubscribedToUI;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        RebuildBlockables();
    }

    private void OnEnable()
    {
        TrySubscribeUI();
        if (!isSubscribedToUI)
            StartCoroutine(WaitForUIControllerThenSubscribe());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        UnsubscribeUI();

        if (Instance == this)
            Instance = null;
    }

    public void RebuildBlockables()
    {
        gameplayBlockables.Clear();
        uiBlockables.Clear();

        Collect(gameplayTargets, gameplayBlockables, "gameplayTargets");
        Collect(uiTargets, uiBlockables, "uiTargets");

        ApplyBlockedState();
    }

    private void Collect(MonoBehaviour[] sources, List<IBlockable> targetList, string label)
    {
        if (sources == null) return;

        for (int i = 0; i < sources.Length; i++)
        {
            var mb = sources[i];
            if (mb == null) continue;

            if (mb is IBlockable blockable)
            {
                if (!targetList.Contains(blockable))
                    targetList.Add(blockable);
            }
            else
            {
                Debug.LogWarning($"[GameBlockController] Target '{mb.name}' in {label} does not implement IBlockable.", mb);
            }
        }
    }

    public bool IsBlocked => HasAnyActiveReason();

    public int ActiveReasonCount => activeReasons.Count;

    public bool IsReasonActive(BlockReason reason)
    {
        return activeReasons.Contains(reason);
    }

    public void Block(BlockReason reason)
    {
        // Idempotent: adding an already-active reason does nothing.
        bool added = activeReasons.Add(reason);
        if (!added)
        {
            if (logStateChanges)
                Debug.Log($"[GameBlockController] Block '{reason}' ignored (already active) -> IsBlocked={IsBlocked}.", this);
            return;
        }

        ApplyBlockedState();

        if (logStateChanges)
            Debug.Log($"[GameBlockController] Block '{reason}' -> IsBlocked={IsBlocked}.", this);
    }

    public void Unblock(BlockReason reason)
    {
        // Idempotent: removing a non-active reason does nothing.
        bool removed = activeReasons.Remove(reason);
        if (!removed)
        {
            if (logStateChanges)
                Debug.Log($"[GameBlockController] Unblock '{reason}' ignored (not active) -> IsBlocked={IsBlocked}.", this);
            return;
        }

        ApplyBlockedState();

        if (logStateChanges)
            Debug.Log($"[GameBlockController] Unblock '{reason}' -> IsBlocked={IsBlocked}.", this);
    }

    public void SetBlocked(BlockReason reason, bool blocked)
    {
        HandleBlockRequest(reason, blocked);
    }

    /// <summary>
    /// Single decision point: receives the request + reason and decides what gets blocked.
    /// </summary>
    private void HandleBlockRequest(BlockReason reason, bool blocked)
    {
        if (blocked) Block(reason);
        else Unblock(reason);

        // Decisions are applied in ApplyBlockedState() based on active reasons.
    }

    private bool HasAnyActiveReason()
    {
        return activeReasons.Count > 0;
    }

    private void ApplyBlockedState()
    {
        // Gameplay is blocked if ANY reason is active (inventory open, dialogue, sleep, etc.)
        bool gameplayBlocked = HasAnyActiveReason();

        // UI is blocked only for strong blockers (NOT for inventory-open).
        // This prevents the inventory from "blocking itself".
        bool uiBlocked =
            IsReasonActive(BlockReason.Dialogue) ||
            IsReasonActive(BlockReason.Sleep);

        for (int i = 0; i < gameplayBlockables.Count; i++)
        {
            var b = gameplayBlockables[i];
            if (b == null) continue;
            b.SetBlocked(gameplayBlocked);
        }

        for (int i = 0; i < uiBlockables.Count; i++)
        {
            var b = uiBlockables[i];
            if (b == null) continue;
            b.SetBlocked(uiBlocked);
        }
    }

    private void TrySubscribeUI()
    {
        if (isSubscribedToUI) return;
        if (UIController.Instance == null) return;

        UIController.Instance.InventoryOpened += OnInventoryOpened;
        UIController.Instance.InventoryClosed += OnInventoryClosed;
        isSubscribedToUI = true;
    }

    private void UnsubscribeUI()
    {
        if (!isSubscribedToUI) return;
        if (UIController.Instance != null)
        {
            UIController.Instance.InventoryOpened -= OnInventoryOpened;
            UIController.Instance.InventoryClosed -= OnInventoryClosed;
        }

        isSubscribedToUI = false;
    }

    private IEnumerator WaitForUIControllerThenSubscribe()
    {
        while (UIController.Instance == null)
            yield return null;

        TrySubscribeUI();
    }

    private void OnInventoryOpened()
    {
        SetBlocked(BlockReason.Inventory, true);
    }

    private void OnInventoryClosed()
    {
        SetBlocked(BlockReason.Inventory, false);
    }
}
