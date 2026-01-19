using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;

public class DialogueController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI bodyText;
    [SerializeField] private float typeSpeed = 20f;

    [TextArea(5, 10)]
    private Queue<string> paragraphs = new Queue<string>();

    private bool conversationEnded;
    private bool isTyping;

    private string p;
    private Coroutine typeDialogueCoroutine;
    private const string HTML_ALPHA = "<color=#00000000>";
    private const float MAX_TYPE_TIME = 0.1f;

    public void DisplayNextParagraph(DialogueText dialogueText)
    {
        if (paragraphs.Count == 0)
        {
            if (!conversationEnded)
            {
                StartConversation(dialogueText);
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
            finishParagraphEarly();
        }

        if (paragraphs.Count == 0)
        {
            conversationEnded = true;
        }
    }

    private void StartConversation(DialogueText dialogueText)
    {
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

    private void finishParagraphEarly()
    {
        StopCoroutine(typeDialogueCoroutine);
        bodyText.text = p;
        isTyping = false;
    }
}
