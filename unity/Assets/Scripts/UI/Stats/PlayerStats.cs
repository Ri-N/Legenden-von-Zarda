using System;

using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance { get; private set; }

    [SerializeField] private int gold = 0;

    public int Gold => gold;

    public event Action<StatType, int> StatChanged; // (type, newValue)

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void AddGold(int amount)
    {
        if (amount <= 0) return;
        SetGold(gold + amount);
    }

    public bool TrySpendGold(int amount)
    {
        if (amount <= 0) return true;
        if (gold < amount) return false;

        SetGold(gold - amount);
        return true;
    }

    public void SetGold(int value)
    {
        int clamped = Mathf.Max(0, value);
        if (clamped == gold) return;

        gold = clamped;
        StatChanged?.Invoke(StatType.Gold, gold);
    }
}
