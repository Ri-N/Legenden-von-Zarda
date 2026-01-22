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

    [TextArea(5, 10)]
    private readonly Queue<string> paragraphs = new();

    private bool conversationEnded;
    private bool isTyping;

    private string p;
    private Coroutine typeDialogueCoroutine;
    private const string HTML_ALPHA = "<color=#00000000>";
    private const float MAX_TYPE_TIME = 0.1f;

    private DialogueText activeDialogue;
    private bool awaitingChoice;

    private object currentInteractor;


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
            if (activeDialogue == null || conversationEnded || paragraphs.Count == 0)
            {
                activeDialogue = dialogueText;
            }
        }

        if (activeDialogue == null)
        {
            return;
        }

        if (paragraphs.Count == 0)
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
            p = paragraphs.Dequeue();
            typeDialogueCoroutine = StartCoroutine(TypeDialogueText(p));
        }
        else
        {
            FinishParagraphEarly();
        }

        if (paragraphs.Count == 0)
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

    private void StartConversation(DialogueText dialogueText)
    {
        HideChoices();
        awaitingChoice = false;
        activeDialogue = dialogueText;

        Player.Instance.SetCanMove(false);
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        nameText.text = dialogueText.speakerName;

        foreach (string paragraph in dialogueText.paragraphs)
        {
            paragraphs.Enqueue(paragraph);
        }
    }

    private void EndConversation()
    {
        Player.Instance.SetCanMove(true);

        HideChoices();
        awaitingChoice = false;
        activeDialogue = null;
        currentInteractor = null;
        paragraphs.Clear();

        conversationEnded = false;

        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
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

        // Execute side-effects tied to this choice (optional)
        if (choice != null && choice.outcomes != null)
        {
            foreach (DialogueOutcomeAction outcome in choice.outcomes)
            {
                if (outcome == null) continue;

                try
                {
                    outcome.Execute(new DialogueOutcomeContext(currentInteractor));
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"DialogueController: Outcome '{outcome.name}' threw an exception: {ex}", this);
                }
            }
        }

        if (choice == null || choice.next == null)
        {
            EndConversation();
            return;
        }

        paragraphs.Clear();
        conversationEnded = false;
        DisplayNextParagraph(choice.next);
    }
}
