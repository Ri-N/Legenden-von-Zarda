using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class PlayerInteractUI : MonoBehaviour
{
    [SerializeField] private GameObject containerGameObject;
    [SerializeField] private TextMeshProUGUI interactText;

    [SerializeField] private Vector3 worldOffset = new Vector3(0f, 1.6f, 0f);

    private Player player;
    private IInteractable currentInteractable;

    private RectTransform containerRect;
    private Canvas parentCanvas;
    private Camera uiCamera;

    private void Start()
    {
        player = Player.Instance;

        containerRect = containerGameObject != null ? containerGameObject.GetComponent<RectTransform>() : null;
        parentCanvas = containerGameObject != null ? containerGameObject.GetComponentInParent<Canvas>() : null;

        uiCamera = (parentCanvas != null && parentCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
            ? parentCanvas.worldCamera
            : Camera.main;

        if (player != null)
            player.OnInteractableChanged += HandleChanged;

        if (containerGameObject != null)
            containerGameObject.SetActive(false);
    }

    private void HandleChanged(IInteractable prev, IInteractable current)
    {
        currentInteractable = current;

        containerGameObject.SetActive(currentInteractable != null);
        if (currentInteractable != null)
            interactText.SetText(currentInteractable.GetInteractText());
    }

    private void LateUpdate()
    {
        if (currentInteractable == null || containerRect == null || containerGameObject == null)
            return;

        if (uiCamera == null)
            uiCamera = Camera.main;

        Vector3 worldPos = currentInteractable.GetTransform().position + worldOffset;
        Vector3 screenPos = uiCamera.WorldToScreenPoint(worldPos);

        // If behind the camera, hide the prompt
        if (screenPos.z <= 0f)
        {
            if (containerGameObject.activeSelf)
                containerGameObject.SetActive(false);
            return;
        }

        if (!containerGameObject.activeSelf)
            containerGameObject.SetActive(true);

        // Position depends on canvas render mode
        if (parentCanvas != null && parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            containerRect.position = screenPos;
        }
        else if (parentCanvas != null)
        {
            RectTransform canvasRect = parentCanvas.transform as RectTransform;
            if (canvasRect != null)
            {
                Vector2 localPoint;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, uiCamera, out localPoint);
                containerRect.anchoredPosition = localPoint;
            }
            else
            {
                containerRect.position = screenPos;
            }
        }
        else
        {
            containerRect.position = screenPos;
        }
    }

    private void OnDisable()
    {
        if (player != null)
            player.OnInteractableChanged -= HandleChanged;

        currentInteractable = null;
        if (containerGameObject != null)
            containerGameObject.SetActive(false);
    }
}
