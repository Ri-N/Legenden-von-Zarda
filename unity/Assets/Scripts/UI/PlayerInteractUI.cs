using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class PlayerInteractUI : MonoBehaviour, IUIElementController, IBlockable
{
    private bool isHidden;
    private bool isBlocked;

    private bool SuppressPrompt => isHidden || isBlocked;

    private void OnEnable()
    {
        if (UIController.Instance != null)
        {
            UIController.Instance.Register(this);
        }
    }

    [SerializeField] private GameObject containerGameObject;
    [SerializeField] private TextMeshProUGUI interactText;

    [SerializeField] private Vector3 worldOffset = new(0f, 1.6f, 0f);

    private Player player;
    private IInteractable currentInteractable;

    private RectTransform containerRect;
    private Canvas parentCanvas;
    private Camera uiCamera;

    public UIElement Element => UIElement.PlayerInteract;

    /// <summary>
    /// Called by UIController to show/hide this UI element.
    /// IMPORTANT: Do not disable this GameObject, otherwise it can't receive further UI requests.
    /// </summary>
    public void SetHidden(bool hidden)
    {
        if (hidden)
            Hide();
        else
            Show();
    }

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

        if (containerGameObject == null)
            return;

        // When suppressed (e.g., during sleep or blocked), never show the prompt.
        containerGameObject.SetActive(!SuppressPrompt && currentInteractable != null);
        if (!SuppressPrompt && currentInteractable != null)
            interactText.SetText(currentInteractable.GetInteractText());
    }

    private void LateUpdate()
    {
        if (currentInteractable == null || containerRect == null || containerGameObject == null)
            return;

        if (SuppressPrompt)
        {
            if (containerGameObject != null && containerGameObject.activeSelf)
                containerGameObject.SetActive(false);
            return;
        }

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
            var canvasRect = parentCanvas.transform as RectTransform;
            if (canvasRect != null)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, uiCamera, out Vector2 localPoint);
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
        if (UIController.Instance != null)
        {
            UIController.Instance.Unregister(this);
        }

        if (player != null)
            player.OnInteractableChanged -= HandleChanged;

        currentInteractable = null;
        if (containerGameObject != null)
            containerGameObject.SetActive(false);

        isHidden = false;
        isBlocked = false;
    }

    private void Hide()
    {
        isHidden = true;
        if (containerGameObject != null)
            containerGameObject.SetActive(false);
    }

    private void Show()
    {
        isHidden = false;

        if (containerGameObject != null)
            containerGameObject.SetActive(currentInteractable != null && !SuppressPrompt);

        if (currentInteractable != null && !SuppressPrompt)
            interactText.SetText(currentInteractable.GetInteractText());
    }

    public void SetBlocked(bool blocked)
    {
        isBlocked = blocked;

        if (SuppressPrompt)
        {
            if (containerGameObject != null)
                containerGameObject.SetActive(false);
            return;
        }

        if (containerGameObject != null)
            containerGameObject.SetActive(currentInteractable != null);

        if (currentInteractable != null)
            interactText.SetText(currentInteractable.GetInteractText());
    }
}
