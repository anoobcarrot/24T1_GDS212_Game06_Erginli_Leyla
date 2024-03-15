using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Random = UnityEngine.Random;

public enum DialogueOptionType
{
    Positive,
    Neutral,
    Negative
}

[System.Serializable]
public class DialogueOption
{
    public string optionText;
    public DialogueOptionType optionType;
}

[System.Serializable]
public class Dialogue
{
    public string text;
    public List<DialogueOption> options;

    public Dialogue(string text, List<DialogueOption> options)
    {
        this.text = text;
        this.options = options;
    }
}

[System.Serializable]
public class FollowUpDialogue
{
    public DialogueOptionType triggerOption;
    public List<Dialogue> dialogues;

    public FollowUpDialogue(DialogueOptionType triggerOption, List<Dialogue> dialogues)
    {
        this.triggerOption = triggerOption;
        this.dialogues = dialogues;
    }
}

public class CompanionDialogue : MonoBehaviour
{
    public Companion companion;
    public TextMeshProUGUI dialogueText;
    public Image dialogueBoxImage;
    public GameObject optionButtonPrefab;
    public Transform optionButtonParent;

    public List<Dialogue> dialogues = new List<Dialogue>();
    public List<FollowUpDialogue> followUpDialogues = new List<FollowUpDialogue>();

    private MoralityFriendshipManager moralityFriendshipManager;
    private Dialogue currentDialogue;

    void Start()
    {
        moralityFriendshipManager = MoralityFriendshipManager.instance;
        StartCoroutine(ShowRandomDialogueRoutine());
    }

    IEnumerator ShowRandomDialogueRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(30f);
            ShowRandomDialogue();
        }
    }

    void ShowRandomDialogue()
{
    if (dialogues.Count == 0)
    {
        Debug.LogError("No dialogues available.");
        return;
    }

    int randomIndex = Random.Range(0, dialogues.Count);
    currentDialogue = dialogues[randomIndex];
    dialogues.RemoveAt(randomIndex); // Remove the selected dialogue from the list
    ShowDialogue(currentDialogue, true); // Initially show dialogue with options
}

    void ShowDialogue(Dialogue dialogue, bool showOptions)
{
    dialogueText.text = ""; // Clear existing text
    dialogueBoxImage.gameObject.SetActive(true); // Show the dialogue box immediately
    StartCoroutine(AnimateText(dialogue.text, showOptions)); // Start text animation after a short delay
}

    IEnumerator AnimateText(string text, bool showOptions)
    {
        foreach (char letter in text)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.05f);
        }

        // If showing options, display them after text animation completes
        if (showOptions)
        {
            dialogueBoxImage.gameObject.SetActive(true);
            PresentDialogueOptions(currentDialogue.options);
        }
        else
        {
            StartCoroutine(ShowDialogueAfterDelay(currentDialogue, 1f)); // Show dialogue without options
        }
    }

    IEnumerator ShowDialogueAfterDelay(Dialogue dialogue, float delay)
    {
        yield return new WaitForSeconds(delay);
        dialogueBoxImage.gameObject.SetActive(true);
        dialogueText.text = dialogue.text; // Display the entire text
        yield return new WaitForSeconds(1f); // Wait for 1 second after text animation completes
        dialogueBoxImage.gameObject.SetActive(false); // Disable dialogue box
    }

    void PresentDialogueOptions(List<DialogueOption> options)
{
    float buttonHeight = optionButtonPrefab.GetComponent<RectTransform>().rect.height;
    float verticalSpacing = 10f; // Adjust this value to control the spacing between buttons

    for (int i = 0; i < options.Count; i++)
    {
        GameObject buttonGO = Instantiate(optionButtonPrefab, optionButtonParent);
        Button button = buttonGO.GetComponent<Button>();
        int index = i;
        button.onClick.AddListener(() => PlayerChoice(options[index]));
        button.GetComponentInChildren<TextMeshProUGUI>().text = options[i].optionText;

        // Adjust the vertical position of the button
        RectTransform buttonRect = button.GetComponent<RectTransform>();
        Vector2 buttonPosition = buttonRect.anchoredPosition;
        buttonPosition.y = -i * (buttonHeight + verticalSpacing);
        buttonRect.anchoredPosition = buttonPosition;
    }
}

    public void PlayerChoice(DialogueOption selectedOption)
    {
        ModifyMoralityAndFriendship(selectedOption.optionType);
        ClearDialogueOptions();

        // Check if there are follow-up dialogues for the selected option
        foreach (FollowUpDialogue followUpDialogue in followUpDialogues)
        {
            if (followUpDialogue.triggerOption == selectedOption.optionType)
            {
                // Show a random follow-up dialogue
                if (followUpDialogue.dialogues.Count > 0)
                {
                    currentDialogue = followUpDialogue.dialogues[Random.Range(0, followUpDialogue.dialogues.Count)];
                    ShowDialogue(currentDialogue, false); // Show follow-up dialogue without options
                    return;
                }
            }
        }

        // If no follow-up dialogue, disable the dialogue box 
        StartCoroutine(DisableDialogueBoxAfterDelay(1f));
    }

    IEnumerator DisableDialogueBoxAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        dialogueBoxImage.gameObject.SetActive(false);
    }

    void ClearDialogueOptions()
    {
        foreach (Transform child in optionButtonParent)
        {
            Destroy(child.gameObject);
        }
    }

    private void ModifyMoralityAndFriendship(DialogueOptionType optionType)
    {
        int moralityChange = GetMoralityChange(optionType);
        int friendshipChange = GetFriendshipChange(optionType);
        moralityFriendshipManager.ModifyMorality(companion.name, moralityChange);
        moralityFriendshipManager.ModifyFriendship(companion.name, friendshipChange);
    }

    private int GetMoralityChange(DialogueOptionType optionType)
    {
        int moralityTraitValue = companion.personalityTraits[0].value;
        switch (optionType)
        {
            case DialogueOptionType.Positive:
                return (moralityTraitValue > 0) ? 1 : 0;
            case DialogueOptionType.Negative:
                return (moralityTraitValue < 0) ? -1 : 0;
            default:
                return 0;
        }
    }

    private int GetFriendshipChange(DialogueOptionType optionType)
    {
        int friendshipTraitValue = companion.personalityTraits[1].value;
        switch (optionType)
        {
            case DialogueOptionType.Positive:
                return (friendshipTraitValue > 0) ? 1 : 0;
            case DialogueOptionType.Negative:
                return (friendshipTraitValue < 0) ? -1 : 0;
            default:
                return 0;
        }
    }
}






