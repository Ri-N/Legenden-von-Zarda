using UnityEngine;

public enum InteractionSide { Both, FrontOnly, BackOnly }

public interface IInteractionConstraint
{
    bool CanInteract(Vector3 playerWorldPosition);
}
