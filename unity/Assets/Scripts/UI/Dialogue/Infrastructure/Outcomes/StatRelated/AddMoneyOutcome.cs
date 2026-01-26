using UnityEngine;

[CreateAssetMenu(fileName = "AddMoneyOutcome", menuName = "Dialogue/Outcomes/AddMoneyOutcome")]
public class AddMoneyOutcome : DialogueOutcomeAction
{
    [Header("Money")]
    [Min(1)]
    [SerializeField] private int amount = 1;

    public override void Execute(DialogueOutcomeContext ctx)
    {
        if (amount <= 0)
        {
            Debug.LogError($"[{nameof(AddMoneyOutcome)}] Invalid amount '{amount}'. Amount must be >= 1.", this);
            return;
        }

        PlayerStats stats = PlayerStats.Instance;
        if (stats == null)
            stats = FindFirstObjectByType<PlayerStats>();

        if (stats == null)
        {
            Debug.LogError($"[{nameof(AddMoneyOutcome)}] Could not resolve a {nameof(PlayerStats)} in the scene. Add a PlayerStats component to the player (or a singleton instance).", this);
            return;
        }

        stats.AddGold(amount);
    }
}
