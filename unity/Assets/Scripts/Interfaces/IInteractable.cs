using UnityEngine;

public interface IInteractable
{
    void Interact();
    Transform GetTransform();
    string GetInteractText();
}

public abstract class InteractableBase : MonoBehaviour, IInteractable
{
    public abstract void Interact();

    public virtual Transform GetTransform()
    {
        return transform;
    }

    public virtual string GetInteractText()
    {
        return "Interagieren";
    }
}
