using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Outcomes/Teleport Player Inside")]
public class TeleportPlayerInsideOutcome : DialogueOutcomeAction
{
    public override void Execute(DialogueOutcomeContext ctx)
    {
        if (PlayerWorldState.Instance == null)
        {
            Debug.LogError("TeleportPlayerInsideOutcome: PlayerWorldState.Instance is null. Ensure a PlayerWorldState exists in the scene.");
            return;
        }

        PlayerWorldState.Instance.RequestArea(PlayerArea.Room, ctx.Interactor);
    }
}
