using UnityEngine;

[CreateAssetMenu(fileName = "LoseMoneyOutcome", menuName = "Dialogue/Outcomes/LoseMoneyOutcome")]
public class LoseMoneyOutcome : DialogueOutcomeAction
{
    [Header("Money")]
    [Min(1)]
    [SerializeField] private int amount = 1;

    [Tooltip("If true, the outcome fails if the player does not have enough gold.")]
    [SerializeField] private bool requireFullAmount = true;

    public override void Execute(DialogueOutcomeContext ctx)
    {
        if (amount <= 0)
        {
            Debug.LogError($"[{nameof(LoseMoneyOutcome)}] Invalid amount '{amount}'. Amount must be >= 1.", this);
            return;
        }

        PlayerStats stats = PlayerStats.Instance;
        if (stats == null)
            stats = FindFirstObjectByType<PlayerStats>();

        if (stats == null)
        {
            Debug.LogError($"[{nameof(LoseMoneyOutcome)}] Could not resolve a {nameof(PlayerStats)} in the scene. Add a PlayerStats component.", this);
            return;
        }

        if (requireFullAmount)
        {
            bool ok = stats.TrySpendGold(amount);
            if (!ok)
            {
                Debug.LogWarning($"[{nameof(LoseMoneyOutcome)}] Not enough gold. Requested {amount}, current {stats.Gold}.", this);
                return;
            }
        }
        else
        {
            int removable = Mathf.Min(stats.Gold, amount);
            if (removable > 0)
                stats.SetGold(stats.Gold - removable);
        }
    }
}
