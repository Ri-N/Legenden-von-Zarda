using UnityEngine;

public class Bed : InteractableBase
{

    public override void Interact()
    {
        Sleep();
    }

    private void Sleep()
    {
        Debug.Log("Player sleeps in the bed.");
    }
}
