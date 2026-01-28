using System;

using UnityEngine;

public class GameInput : MonoBehaviour
{
    private PlayerInputActions playerInputActions;
    public event EventHandler OnInteractAction;
    public event EventHandler OnOpenInventory;

    public event EventHandler OnOpenMenu;

    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();

        playerInputActions.Player.Interact.performed += Interact_Performed;
        playerInputActions.Player.OpenInventory.performed += OpenInventory_Performed;
        playerInputActions.Player.OpenMenu.performed += OpenMenu_Performed;
    }

    public Vector2 GetMovementVectorNormalized()
    {
        return playerInputActions.Player.Move.ReadValue<Vector2>().normalized;
    }

    public void Interact_Performed(UnityEngine.InputSystem.InputAction.CallbackContext callbackContext)
    {
        OnInteractAction?.Invoke(this, EventArgs.Empty);
    }

    public void OpenInventory_Performed(UnityEngine.InputSystem.InputAction.CallbackContext callbackContext)
    {
        OnOpenInventory?.Invoke(this, EventArgs.Empty);
    }

    public void OpenMenu_Performed(UnityEngine.InputSystem.InputAction.CallbackContext callbackContext)
    {
        OnOpenMenu?.Invoke(this, EventArgs.Empty);
    }

    private void OnDestroy()
    {
        if (playerInputActions == null) return;

        playerInputActions.Player.Interact.performed -= Interact_Performed;
        playerInputActions.Player.OpenInventory.performed -= OpenInventory_Performed;
        playerInputActions.Player.OpenMenu.performed -= OpenMenu_Performed;
    }
}
