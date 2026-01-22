using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Outcomes/Teleport Player Outside")]
public class TeleportPlayerOutsideOutcome : DialogueOutcomeAction
{
    public override void Execute(DialogueOutcomeContext ctx)
    {
        if (Player.Instance == null) return;

        if (ctx.Interactor is not IHasTeleportPoints hasExit || hasExit.ExitPoint == null)
            return;

        Player.Instance.MoveTo(hasExit.ExitPoint.position);
    }
}

[CreateAssetMenu(menuName = "Dialogue/Outcomes/Teleport Player Inside")]
public class TeleportPlayerInsideOutcome : DialogueOutcomeAction
{
    public override void Execute(DialogueOutcomeContext ctx)
    {
        if (Player.Instance == null) return;

        if (ctx.Interactor is not IHasTeleportPoints hasExit || hasExit.EntryPoint == null)
            return;

        Player.Instance.MoveTo(hasExit.EntryPoint.position);
    }
}
