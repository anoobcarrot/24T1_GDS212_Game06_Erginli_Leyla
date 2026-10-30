using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("Dialogue UI")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI speakerText;
    public TextMeshProUGUI dialogueText;

    [Header("Choices UI")]
    public GameObject choicesPanel;
    public Button choiceButtonPrefab;
    public Transform choicesContainer;

    [Header("Settings")]
    public KeyCode interactKey = KeyCode.Space;
    public float textSpeed = 0.04f;

    private DialogueData currentDialogue;
    private NPCController currentNPC;

    private int currentLineIndex;
    private Coroutine typingCoroutine;

    private bool isTyping;
    private bool waitingForChoice;

    public bool IsDialogueActive => currentDialogue != null;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        dialoguePanel.SetActive(false);
        choicesPanel.SetActive(false);
    }

    private void Update()
    {
        if (!IsDialogueActive)
            return;

        if (waitingForChoice)
            return;

        if (Input.GetKeyDown(interactKey))
        {
            if (isTyping)
            {
                FinishTyping();
            }
            else
            {
                NextLine();
            }
        }
    }

    public void StartDialogue(DialogueData dialogue, NPCController npc)
    {
        if (dialogue == null || dialogue.lines == null ||
            dialogue.lines.Length == 0)
            return;

        currentNPC = npc;
        currentDialogue = dialogue;
        currentLineIndex = 0;

        dialoguePanel.SetActive(true);

        ShowLine();
    }

    private void ShowLine()
    {
        ClearChoices();

        DialogueLine line = currentDialogue.lines[currentLineIndex];

        speakerText.text = line.speakerName;

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(line.text));
    }

    private IEnumerator TypeText(string text)
    {
        isTyping = true;

        dialogueText.text = text;
        dialogueText.maxVisibleCharacters = 0;

        dialogueText.ForceMeshUpdate();

        int characterCount = dialogueText.textInfo.characterCount;

        for (int i = 0; i <= characterCount; i++)
        {
            dialogueText.maxVisibleCharacters = i;

            yield return new WaitForSeconds(textSpeed);
        }

        isTyping = false;
        typingCoroutine = null;

        CheckForChoices();
    }

    private void FinishTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        dialogueText.maxVisibleCharacters = int.MaxValue;

        isTyping = false;

        CheckForChoices();
    }

    private void CheckForChoices()
    {
        DialogueLine line = currentDialogue.lines[currentLineIndex];

        if (line.choices == null ||
            line.choices.options == null ||
            line.choices.options.Length == 0)
            return;

        waitingForChoice = true;

        choicesPanel.SetActive(true);

        foreach (CelestriaDialogueOption option in line.choices.options)
        {
            Button button = Instantiate(
                choiceButtonPrefab,
                choicesContainer
            );

            TextMeshProUGUI buttonText =
                button.GetComponentInChildren<TextMeshProUGUI>();

            buttonText.text = option.optionText;

            DialogueData nextDialogue = option.nextDialogue;

            button.onClick.AddListener(() =>
            {
                SelectChoice(nextDialogue);
            });
        }
    }

    private void SelectChoice(DialogueData nextDialogue)
    {
        ClearChoices();

        if (nextDialogue != null)
        {
            currentDialogue = nextDialogue;
            currentLineIndex = 0;

            ShowLine();
        }
        else
        {
            EndDialogue();
        }
    }

    private void ClearChoices()
    {
        waitingForChoice = false;

        choicesPanel.SetActive(false);

        foreach (Transform child in choicesContainer)
        {
            Destroy(child.gameObject);
        }
    }

    private void NextLine()
    {
        currentLineIndex++;

        if (currentLineIndex < currentDialogue.lines.Length)
        {
            ShowLine();
        }
        else
        {
            EndDialogue();
        }
    }

    public void EndDialogue()
    {
        if (!IsDialogueActive)
            return;

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        ClearChoices();

        dialoguePanel.SetActive(false);

        NPCController npc = currentNPC;

        currentDialogue = null;
        currentNPC = null;

        isTyping = false;

        if (npc != null)
            npc.OnDialogueFinished();
    }

    public void CancelDialogue()
    {
        if (!IsDialogueActive)
            return;

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        ClearChoices();

        dialoguePanel.SetActive(false);

        currentDialogue = null;
        currentNPC = null;

        isTyping = false;
    }
}