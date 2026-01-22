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
    [Header("Interaction ranges by area")]
    [SerializeField] private float interactionRangeRoom = 5f;
    [SerializeField] private float interactionRangeVillage = 7.5f;
    [SerializeField] private float interactionRange = 5f;

    [Header("Interaction")]
    [Tooltip("Layers that contain interactable colliders.")]
    [SerializeField] private LayerMask interactableMask = ~0;

    [Tooltip("Layers that can block interaction line-of-sight (e.g., Walls, Default).")]
    [SerializeField] private LayerMask obstructionMask = ~0;

    [Tooltip("Ray origin height offset for line-of-sight checks.")]
    [SerializeField] private float interactionRayHeight = 1.2f;

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

        if (PlayerWorldState.Instance != null)
        {
            PlayerWorldState.Instance.AreaChanged += OnAreaChanged;
            ApplyArea(PlayerWorldState.Instance.CurrentArea);
        }
    }

    private void OnDestroy()
    {
        if (PlayerWorldState.Instance != null)
        {
            PlayerWorldState.Instance.AreaChanged -= OnAreaChanged;
        }

        if (gameInput != null)
        {
            gameInput.OnInteractAction -= GameInput_OnInteractAction;
        }
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
        var moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

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

        Collider[] colliders = Physics.OverlapSphere(transform.position, interactionRange, interactableMask, QueryTriggerInteraction.Ignore);
        foreach (Collider collider in colliders)
        {
            if (!collider.TryGetComponent(out IInteractable interactable))
            { continue; }

            IInteractionConstraint constraint = collider.GetComponent<IInteractionConstraint>();
            if (constraint != null && !constraint.CanInteract(transform.position))
            {
                continue;
            }

            Vector3 delta = interactable.GetTransform().position - transform.position;
            float distSq = delta.sqrMagnitude;

            // Block interaction through obstacles.
            if (!HasLineOfSight(interactable, collider))
            {
                continue;
            }

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

    private void OnAreaChanged(PlayerArea newArea, PlayerAreaContext ctx)
    {
        ApplyArea(newArea);
    }

    public void ApplyArea(PlayerArea area)
    {
        interactionRange = area switch
        {
            PlayerArea.Room => interactionRangeRoom,
            PlayerArea.Village => interactionRangeVillage,
            _ => interactionRange,
        };
    }

    public void MoveTo(Vector3 worldPosition)
    {
        if (characterController != null)
        {
            characterController.enabled = false;
        }

        transform.position = worldPosition;
        verticalVelocity = 0f;

        if (characterController != null)
        {
            characterController.enabled = true;
        }
    }
    private bool HasLineOfSight(IInteractable interactable, Collider interactableCollider)
    {
        Vector3 origin = transform.position + Vector3.up * interactionRayHeight;

        Vector3 target = interactableCollider != null
            ? interactableCollider.ClosestPoint(origin)
            : interactable.GetTransform().position;

        // If the target is basically at origin, consider it visible.
        Vector3 dir = target - origin;
        float dist = dir.magnitude;
        if (dist < 0.05f) return true;

        dir /= dist;

        // If anything on obstructionMask is between origin and target, block interaction.
        if (Physics.Raycast(origin, dir, out RaycastHit hit, dist, obstructionMask, QueryTriggerInteraction.Ignore))
        {
            // Allow if the first hit is the interactable itself.
            if (interactableCollider != null && hit.collider == interactableCollider)
            {
                return true;
            }

            return false;
        }

        return true;
    }
}
