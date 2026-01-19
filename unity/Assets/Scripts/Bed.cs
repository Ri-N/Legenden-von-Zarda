

using UnityEngine;

public class Bed : MonoBehaviour, IInteractable
{
    public Transform GetTransform()
    {
        return transform;
    }

    public void Interact()
    {

    }

    public string GetInteractText()
    {
        return "Sleep";
    }
}
