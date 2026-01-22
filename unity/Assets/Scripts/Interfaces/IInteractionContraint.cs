using UnityEngine;

/// <summary>
/// Context available to interaction constraints.
/// Add more fields here over time (e.g., camera forward, time of day, inventory flags)
/// without changing every constraint signature again.
/// </summary>
public readonly struct InteractionContext
{
    public readonly Vector3 PlayerWorldPosition;
    public readonly PlayerArea PlayerArea;

    public InteractionContext(Vector3 playerWorldPosition, PlayerArea playerArea)
    {
        PlayerWorldPosition = playerWorldPosition;
        PlayerArea = playerArea;
    }
}

public interface IInteractionConstraint
{
    bool CanInteract(in InteractionContext ctx);
}
