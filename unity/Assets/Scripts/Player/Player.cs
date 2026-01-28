using System;

using UnityEngine;

[RequireComponent(typeof(CharacterController))]

public class Player : MonoBehaviour, IBlockable
{
    public static Player Instance { get; private set; }
    private CharacterController characterController;


    [Header("Movement")]
    [SerializeField] private float moveSpeed = 7f;

    [Tooltip("How fast the character turns to face movement direction")]
    [Range(0.0f, 0.3f)]
    [SerializeField] private float rotationSmoothTime = 0.12f;

    [Tooltip("Acceleration and deceleration")]
    [SerializeField] private float speedChangeRate = 10.0f;

    [SerializeField] private float gravity = -25f;

    private float speed;
    private float targetRotation;
    private float rotationVelocity;

    private Camera mainCamera;

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
    private PlayerWorldState playerWorldState;

    private bool canMove = true;


    public event Action<IInteractable, IInteractable> OnInteractableChanged;

    private float verticalVelocity;
    private IInteractable current;

    public IInteractable CurrentInteractable => current;


    private void Awake()
    {
        Instance = this;
        characterController = GetComponent<CharacterController>();

        mainCamera = Camera.main;
    }

    private void Start()
    {
        gameInput.OnInteractAction += GameInput_OnInteractAction;

        playerWorldState = PlayerWorldState.Instance;
        if (playerWorldState != null)
        {
            playerWorldState.AreaChanged += OnAreaChanged;
            ApplyArea(playerWorldState.CurrentArea);
        }
    }

    private void OnDestroy()
    {
        if (playerWorldState != null)
        {
            playerWorldState.AreaChanged -= OnAreaChanged;
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
        // Read input (your GameInput currently returns a normalized vector)
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();

        // Determine target speed (0 when no input)
        float targetSpeed = (inputVector == Vector2.zero) ? 0.0f : moveSpeed;

        // Current horizontal speed (ignore vertical velocity)
        float currentHorizontalSpeed = new Vector3(characterController.velocity.x, 0.0f, characterController.velocity.z).magnitude;

        float speedOffset = 0.1f;

        // Smooth speed changes
        if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed, Time.deltaTime * speedChangeRate);
            speed = Mathf.Round(speed * 1000f) / 1000f;
        }
        else
        {
            speed = targetSpeed;
        }

        // Ensure we always reference the actual render camera (driven by CinemachineBrain).
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        // Camera-relative movement (project camera axes onto the ground plane).
        Vector3 camForward = transform.forward;
        Vector3 camRight = transform.right;

        if (mainCamera != null)
        {
            camForward = mainCamera.transform.forward;
            camRight = mainCamera.transform.right;
        }

        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDir = (camRight * inputVector.x + camForward * inputVector.y);
        if (moveDir.sqrMagnitude > 1f)
        {
            moveDir.Normalize();
        }

        // Rotate towards movement direction.
        if (moveDir.sqrMagnitude > 0.0001f)
        {
            targetRotation = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity, rotationSmoothTime);
            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }

        // Horizontal movement direction.
        Vector3 targetDirection = moveDir;

        // Gravity / vertical
        if (characterController.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }

        verticalVelocity += gravity * Time.deltaTime;

        Vector3 motion = (targetDirection.sqrMagnitude > 0.0001f)
            ? targetDirection * (speed * Time.deltaTime)
            : Vector3.zero;
        motion.y = verticalVelocity * Time.deltaTime;

        characterController.Move(motion);
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

            if (collider.TryGetComponent(out IInteractionConstraint constraint))
            {
                PlayerArea area = playerWorldState != null
                    ? playerWorldState.CurrentArea
                    : PlayerArea.Room;

                var ctx = new InteractionContext(transform.position, area);

                if (!constraint.CanInteract(in ctx))
                {
                    continue;
                }
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
        // When blocked, ignore interaction input entirely.
        if (!canMove)
            return;

        IInteractable interactable = GetInteractableObject();
        interactable?.Interact();
    }

    private void SetCanMove(bool canMove)
    {
        this.canMove = canMove;
    }

    public void SetBlocked(bool blocked)
    {
        SetCanMove(!blocked);

        if (blocked)
        {
            SetCurrentInteractable(null);
        }
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
