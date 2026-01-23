using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Outcomes/Teleport Player Outside")]
public class TeleportPlayerOutsideOutcome : DialogueOutcomeAction
{
    public override void Execute(DialogueOutcomeContext ctx)
    {
        if (PlayerWorldState.Instance == null)
        {
            Debug.LogError("TeleportPlayerOutsideOutcome: PlayerWorldState.Instance is null. Ensure a PlayerWorldState exists in the scene.");
            return;
        }

        PlayerWorldState.Instance.RequestArea(PlayerArea.Village, ctx.Interactor);
    }
}