using UnityEngine;

public interface IInteractionConstraint
{
    bool CanInteract(Vector3 playerWorldPosition);
}
