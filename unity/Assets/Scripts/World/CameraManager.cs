using Unity.Cinemachine;

using UnityEngine;

public class CameraManager : MonoBehaviour, IBlockable
{
    [Header("Cameras")]
    [SerializeField] private CinemachineCamera topDownCam;
    [SerializeField] private CinemachineCamera thirdPersonCam;

    [Header("Priorities")]
    [SerializeField] private int activePriority = 20;
    [SerializeField] private int inactivePriority = 10;

    private bool isBlocked;

    // Cinemachine input components that drive camera rotation from mouse/stick.
    // Depending on the Cinemachine version/setup, one of these may be present.
    private CinemachineInputAxisController topDownAxis;
    private CinemachineInputAxisController thirdPersonAxis;

    private void OnEnable()
    {
        if (PlayerWorldState.Instance != null)
        {
            PlayerWorldState.Instance.AreaChanged += HandleAreaChanged;
        }
    }

    private void OnDisable()
    {
        if (PlayerWorldState.Instance != null)
        {
            PlayerWorldState.Instance.AreaChanged -= HandleAreaChanged;
        }
    }

    private void Start()
    {
        // Cache input components once. Disabling these will block camera rotation.
        topDownAxis = topDownCam != null ? topDownCam.GetComponent<CinemachineInputAxisController>() : null;
        thirdPersonAxis = thirdPersonCam != null ? thirdPersonCam.GetComponent<CinemachineInputAxisController>() : null;

        ApplyBlockedState();

        // Ensure we're subscribed even if the singleton becomes available after OnEnable
        if (PlayerWorldState.Instance != null)
        {
            // Avoid double-subscribe if OnEnable already ran with Instance available
            PlayerWorldState.Instance.AreaChanged -= HandleAreaChanged;
            PlayerWorldState.Instance.AreaChanged += HandleAreaChanged;

            ApplyForArea(PlayerWorldState.Instance.CurrentArea);
        }
        else
        {
            // Safe fallback
            SetMode(CameraMode.TopDown);
        }
    }

    private void HandleAreaChanged(PlayerArea area, PlayerAreaContext ctx)
    {
        ApplyForArea(area);
    }

    private void ApplyForArea(PlayerArea area)
    {
        // Room => TopDown, Village => ThirdPerson
        switch (area)
        {
            case PlayerArea.Room:
                SetMode(CameraMode.TopDown);
                break;
            case PlayerArea.Village:
                SetMode(CameraMode.ThirdPerson);
                break;
            default:
                SetMode(CameraMode.TopDown);
                break;
        }
    }

    private void SetMode(CameraMode mode)
    {
        bool third = mode == CameraMode.ThirdPerson;

        thirdPersonCam.Priority = third ? activePriority : inactivePriority;
        topDownCam.Priority = third ? inactivePriority : activePriority;
    }

    private void ApplyBlockedState()
    {
        if (topDownAxis != null) topDownAxis.enabled = !isBlocked;
        if (thirdPersonAxis != null) thirdPersonAxis.enabled = !isBlocked;
    }

    public void SetBlocked(bool blocked)
    {
        isBlocked = blocked;
        ApplyBlockedState();
    }
}
