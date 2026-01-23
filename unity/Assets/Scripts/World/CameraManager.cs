using Unity.Cinemachine;

using UnityEngine;


public class CameraManager : MonoBehaviour
{
    [Header("Cameras")]
    [SerializeField] private CinemachineCamera topDownCam;
    [SerializeField] private CinemachineCamera thirdPersonCam;

    [Header("Priorities")]
    [SerializeField] private int activePriority = 20;
    [SerializeField] private int inactivePriority = 10;

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

    private enum CameraMode { TopDown, ThirdPerson }
}
