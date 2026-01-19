using System;

using UnityEngine;

[RequireComponent(typeof(CharacterController))]

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }
    private CharacterController characterController;


    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float gravity = -25f;
    [SerializeField] private float interactionRange = 5f;
    [SerializeField] private GameInput gameInput;

    private bool canMove = true;


    public event Action<IInteractable, IInteractable> OnInteractableChanged;

    private float verticalVelocity;
    private IInteractable current;

    public IInteractable CurrentInteractable => current;


    private void Awake()
    {
        Instance = this;
        characterController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        gameInput.OnInteractAction += GameInput_OnInteractAction;
    }

    private void Update()
    {
        if (canMove)
        {
            HandleMovement();
            UpdateCurrentInteractable();
        }
    }

    private void HandleMovement()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        if (characterController.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }

        verticalVelocity += gravity * Time.deltaTime;

        Vector3 velocity = moveDir * moveSpeed;
        velocity.y = verticalVelocity;

        characterController.Move(velocity * Time.deltaTime);

        // Rotate towards horizontal movement direction
        if (moveDir.sqrMagnitude > 0.001f)
        {
            transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotationSpeed);
        }
    }

    private void UpdateCurrentInteractable()
    {
        IInteractable next = GetInteractableObject();
        SetCurrentInteractable(next);
    }

    public IInteractable GetInteractableObject()
    {
        IInteractable closestInteractable = null;
        float closestDistSq = float.PositiveInfinity;

        Collider[] colliders = Physics.OverlapSphere(transform.position, interactionRange);
        foreach (Collider collider in colliders)
        {
            if (!collider.TryGetComponent(out IInteractable interactable))
            { continue; }

            Vector3 delta = interactable.GetTransform().position - transform.position;
            float distSq = delta.sqrMagnitude;

            if (distSq < closestDistSq)
            {
                closestDistSq = distSq;
                closestInteractable = interactable;
            }
        }

        return closestInteractable;
    }

    private void SetCurrentInteractable(IInteractable next)
    {
        if (ReferenceEquals(current, next))
        { return; }

        IInteractable prev = current;
        current = next;
        OnInteractableChanged?.Invoke(prev, current);
    }

    private void GameInput_OnInteractAction(object sender, EventArgs e)
    {
        IInteractable interactable = GetInteractableObject();
        interactable?.Interact();
    }

    public void SetCanMove(bool canMove)
    {
        this.canMove = canMove;
    }
}
