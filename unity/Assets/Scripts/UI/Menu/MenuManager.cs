using UnityEngine;

public sealed class MenuManager : MonoBehaviour, IUIElementController
{
    [Header("UI")]
    [SerializeField] private UIController uiController;
    [SerializeField] private GameInput gameInput;

    [SerializeField] private GameObject menuRoot;

    [Header("Menu Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject controlsPanel;
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private GameObject creditsPanel;

    [Header("Optional")]
    [SerializeField] private bool pauseTime = true;

    private bool isOpen;

    public UIElement Element => UIElement.Menu;

    private void OnEnable()
    {
        if (uiController == null)
            uiController = FindFirstObjectByType<UIController>();

        if (uiController != null)
            uiController.Register(this);

        if (gameInput == null)
            gameInput = FindFirstObjectByType<GameInput>();
    }

    private void OnDisable()
    {
        if (uiController != null)
            uiController.Unregister(this);
    }

    private void Awake()
    {
        if (menuRoot) menuRoot.SetActive(false);
        if (mainMenuPanel) mainMenuPanel.SetActive(true);
        if (controlsPanel) controlsPanel.SetActive(false);
        if (tutorialPanel) tutorialPanel.SetActive(false);
        if (creditsPanel) creditsPanel.SetActive(false);
        ApplyState(false);
    }

    public void ToggleMenu() => SetOpen(!isOpen);

    public void SetOpen(bool open)
    {
        isOpen = open;
        if (menuRoot) menuRoot.SetActive(open);
        if (open)
            ShowMainMenu();
        ApplyState(open);
    }

    public void ShowControls()
    {
        if (mainMenuPanel) mainMenuPanel.SetActive(false);
        if (tutorialPanel) tutorialPanel.SetActive(false);
        if (creditsPanel) creditsPanel.SetActive(false);
        if (controlsPanel) controlsPanel.SetActive(true);
    }

    public void ShowTutorial()
    {
        if (mainMenuPanel) mainMenuPanel.SetActive(false);
        if (controlsPanel) controlsPanel.SetActive(false);
        if (creditsPanel) creditsPanel.SetActive(false);
        if (tutorialPanel) tutorialPanel.SetActive(true);
    }

    public void ShowCredits()
    {
        if (mainMenuPanel) mainMenuPanel.SetActive(false);
        if (controlsPanel) controlsPanel.SetActive(false);
        if (tutorialPanel) tutorialPanel.SetActive(false);
        if (creditsPanel) creditsPanel.SetActive(true);
    }

    public void ShowMainMenu()
    {
        if (controlsPanel) controlsPanel.SetActive(false);
        if (tutorialPanel) tutorialPanel.SetActive(false);
        if (creditsPanel) creditsPanel.SetActive(false);
        if (mainMenuPanel) mainMenuPanel.SetActive(true);
    }

    public void StartGame()
    {
        SetOpen(false);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void SetHidden(bool hidden)
    {
        SetOpen(!hidden);
    }

    private void ApplyState(bool menuOpen)
    {
        // Pause
        if (pauseTime)
            Time.timeScale = menuOpen ? 0f : 1f;
    }
}
