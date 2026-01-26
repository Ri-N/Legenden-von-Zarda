using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class DialogueController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI bodyText;
    [SerializeField] private float typeSpeed = 20f;

    [Header("Choices")]
    [SerializeField] private GameObject choicesRoot;
    [SerializeField] private Transform choicesContainer;
    [SerializeField] private Button choiceButtonPrefab;

    [Header("Visual Root")]
    [Tooltip("Child object that contains the dialogue UI visuals. This will be hidden before outcomes execute.")]
    [SerializeField] private GameObject visualRoot;

    [Header("Input")]
    [Tooltip("Optional. If not set, will try to find GameInput at runtime.")]
    [SerializeField] private GameInput gameInput;

    private bool isSubscribedToInteractAdvance;
    private int lastAdvanceFrame = -1;

    private readonly Queue<DialogueText.DialogueLine> lines = new();

    private bool conversationEnded;
    private bool isTyping;

    private DialogueText.DialogueLine currentLine;
    private string p;
    private Coroutine typeDialogueCoroutine;
    private const string HTML_ALPHA = "<color=#00000000>";
    private const float MAX_TYPE_TIME = 0.1f;

    private DialogueText activeDialogue;
    private bool awaitingChoice;

    private object currentInteractor;

    private readonly List<DialogueOutcomeAction> pendingOutcomes = new();


    public void StartDialogue(DialogueText text, object interactor)
    {
        currentInteractor = interactor;
        DisplayNextParagraph(text);
    }


    public void DisplayNextParagraph(DialogueText dialogueText)
    {
        if (awaitingChoice)
        {
            return;
        }

        if (dialogueText != null)
        {
            if (activeDialogue == null || conversationEnded || lines.Count == 0)
            {
                activeDialogue = dialogueText;
            }
        }

        if (activeDialogue == null)
        {
            return;
        }

        if (lines.Count == 0)
        {
            if (!conversationEnded)
            {
                StartConversation(activeDialogue);
            }
            else if (conversationEnded && !isTyping)
            {
                EndConversation();
                return;
            }
        }

        if (!isTyping)
        {
            currentLine = lines.Dequeue();

            // Update speaker per line (each paragraph may have a different speaker)
            if (nameText != null)
                nameText.text = currentLine.speakerName;

            p = currentLine.text;
            typeDialogueCoroutine = StartCoroutine(TypeDialogueText(p));
        }
        else
        {
            FinishParagraphEarly();
        }

        if (lines.Count == 0)
        {
            if (activeDialogue.choices != null && activeDialogue.choices.Length > 0)
            {
                conversationEnded = false;
                ShowChoices(activeDialogue);
            }
            else
            {
                conversationEnded = true;
            }
        }
    }

    private void SubscribeInteractAdvance()
    {
        if (gameInput == null)
        {
            Debug.LogError("[DialogueController] GameInput is missing.");
            return;
        }

        gameInput.OnInteractAction -= OnInteractAdvance;
        gameInput.OnInteractAction += OnInteractAdvance;
        isSubscribedToInteractAdvance = true;
    }

    private void UnsubscribeInteractAdvance()
    {
        if (!isSubscribedToInteractAdvance) return;

        if (gameInput != null)
            gameInput.OnInteractAction -= OnInteractAdvance;

        isSubscribedToInteractAdvance = false;
    }

    private void OnInteractAdvance(object sender, System.EventArgs e)
    {
        // Debounce: if multiple systems fire in the same frame, do not double-advance.
        if (lastAdvanceFrame == Time.frameCount)
            return;
        lastAdvanceFrame = Time.frameCount;

        // Only advance if dialogue is currently active/visible.
        if (!gameObject.activeInHierarchy) return;
        if (activeDialogue == null) return;
        if (visualRoot != null && !visualRoot.activeSelf) return;

        DisplayNextParagraph(null);
    }

    private void StartConversation(DialogueText dialogueText)
    {
        HideChoices();
        awaitingChoice = false;
        activeDialogue = dialogueText;

        // Block gameplay while dialogue is active (movement, inventory, interact prompt, etc.)
        if (GameBlockController.Instance != null)
            GameBlockController.Instance.SetBlocked(BlockReason.Dialogue, true);

        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        if (visualRoot != null && !visualRoot.activeSelf)
        {
            visualRoot.SetActive(true);
        }

        lines.Clear();

        if (dialogueText.lines == null || dialogueText.lines.Length == 0)
        {
            Debug.LogWarning("DialogueController: DialogueText has no lines.", this);
        }
        else
        {
            foreach (DialogueText.DialogueLine line in dialogueText.lines)
            {
                lines.Enqueue(line);
            }
        }

        SubscribeInteractAdvance();
    }

    private void EndConversation()
    {
        UnsubscribeInteractAdvance();

        if (GameBlockController.Instance != null)
            GameBlockController.Instance.SetBlocked(BlockReason.Dialogue, false);

        object interactor = currentInteractor;

        HideChoices();

        if (visualRoot == null)
        {
            if (transform.childCount > 0)
            {
                visualRoot = transform.GetChild(0).gameObject;
            }
        }

        if (visualRoot != null)
        {
            visualRoot.SetActive(false);
        }
        else
        {
            Debug.LogWarning("DialogueController: visualRoot is not assigned and could not be auto-detected. Outcomes will execute before the panel is disabled.", this);
        }

        awaitingChoice = false;
        activeDialogue = null;
        lines.Clear();
        conversationEnded = false;

        // Run deferred outcomes after the UI has been hidden (next frame), then deactivate this panel.
        StartCoroutine(EndConversationRoutine(interactor, visualRoot != null));
    }

    private IEnumerator EndConversationRoutine(object interactor, bool uiHidden)
    {
        // Wait one frame so the UI hide is actually rendered before side-effects happen.
        if (uiHidden)
            yield return null;

        if (pendingOutcomes.Count > 0)
        {
            foreach (DialogueOutcomeAction outcome in pendingOutcomes)
            {
                if (outcome == null) continue;

                try
                {
                    outcome.Execute(new DialogueOutcomeContext(interactor));
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"DialogueController: Deferred outcome '{outcome.name}' threw an exception: {ex}", this);
                }
            }

            pendingOutcomes.Clear();
        }

        currentInteractor = null;

        // Now we can disable the whole panel safely.
        if (gameObject.activeSelf)
            gameObject.SetActive(false);
    }

    private IEnumerator TypeDialogueText(string p)
    {
        isTyping = true;

        bodyText.text = "";

        string originalText = p;
        string displayedText = "";
        int alphaIndex = 0;

        foreach (char c in p.ToCharArray())
        {
            alphaIndex++;
            bodyText.text = originalText;

            displayedText = bodyText.text.Insert(alphaIndex, HTML_ALPHA);
            bodyText.text = displayedText;

            yield return new WaitForSeconds(MAX_TYPE_TIME / typeSpeed);
        }

        isTyping = false;
    }

    private void FinishParagraphEarly()
    {
        StopCoroutine(typeDialogueCoroutine);
        bodyText.text = p;
        isTyping = false;
    }

    private void ShowChoices(DialogueText dialogueText)
    {
        if (choicesRoot != null)
        {
            choicesRoot.SetActive(true);
        }

        awaitingChoice = true;

        // Clear old buttons
        if (choicesContainer != null)
        {
            for (int i = choicesContainer.childCount - 1; i >= 0; i--)
            {
                Destroy(choicesContainer.GetChild(i).gameObject);
            }
        }

        if (choiceButtonPrefab == null || choicesContainer == null)
        {
            Debug.LogWarning("DialogueController: Choices are configured on the dialogue, but UI references are missing (choicesContainer/choiceButtonPrefab).", this);
            return;
        }

        foreach (ChoiceOption choice in dialogueText.choices)
        {
            Button btn = Instantiate(choiceButtonPrefab, choicesContainer);

            TMP_Text tmp = btn.GetComponentInChildren<TMP_Text>();
            if (tmp != null)
            {
                tmp.text = choice.optionText;
                btn.onClick.AddListener(() => OnChoiceSelected(choice));
            }
        }
    }

    private void HideChoices()
    {
        if (choicesRoot != null)
        {
            choicesRoot.SetActive(false);
        }

        if (choicesContainer != null)
        {
            for (int i = choicesContainer.childCount - 1; i >= 0; i--)
            {
                Destroy(choicesContainer.GetChild(i).gameObject);
            }
        }
    }

    private void OnChoiceSelected(ChoiceOption choice)
    {
        HideChoices();
        awaitingChoice = false;

        if (choice != null && choice.outcomes != null)
        {
            foreach (DialogueOutcomeAction outcome in choice.outcomes)
            {
                if (outcome == null) continue;
                pendingOutcomes.Add(outcome);
            }
        }

        if (choice == null || choice.next == null)
        {
            EndConversation();
            return;
        }

        lines.Clear();
        conversationEnded = false;
        DisplayNextParagraph(choice.next);
    }

    private void OnDisable()
    {
        UnsubscribeInteractAdvance();
    }
}
