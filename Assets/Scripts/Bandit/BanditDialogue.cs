using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class BanditDialogue : MonoBehaviour
{
    [System.Serializable]
    public enum BanditAttackOption
    {
        Confront,
        Negotiate,
        Evade
    }

    [System.Serializable]
    public class TraitEffect
    {
        public string traitName;
        public bool applyMoralityChangePositive;
        public bool applyMoralityChangeNegative;
        public bool applyFriendshipChangePositive;
        public bool applyFriendshipChangeNegative;
        public int moralityChange; // Amount of morality change
        public int friendshipChange; // Amount of friendship change

        public TraitEffect(string name, bool moralityPositive, bool moralityNegative, bool friendshipPositive, bool friendshipNegative, int moralityChange, int friendshipChange)
        {
            traitName = name;
            applyMoralityChangePositive = moralityPositive;
            applyMoralityChangeNegative = moralityNegative;
            applyFriendshipChangePositive = friendshipPositive;
            applyFriendshipChangeNegative = friendshipNegative;
            this.moralityChange = moralityChange;
            this.friendshipChange = friendshipChange;
        }
    }

    [System.Serializable]
    public class BanditAttackOptionData
    {
        public BanditAttackOption option;
        public string optionText;
        public List<TraitEffect> traitEffects; // List of trait effects for this option
        public List<string> followUpDialogues; // Follow-up dialogues for this option

        public BanditAttackOptionData()
        {
            traitEffects = new List<TraitEffect>();
            followUpDialogues = new List<string>();
        }
    }

    [System.Serializable]
    public class BanditAttackDialogueData
    {
        public BanditAttackOption option;
        public List<string> dialogueLines;
    }

    [System.Serializable]
    public class BanditAttackData
    {
        public List<BanditAttackOptionData> options;
        public List<BanditAttackDialogueData> dialogues;
    }

    public GameObject popUpUI; // Reference to the pop-up UI element
    public TextMeshProUGUI textMeshProText; // Text field for displaying pop-up message and dialogue
    public GameObject optionButtonPrefab; // Prefab for option buttons
    public Transform optionButtonParent; // Parent transform for option buttons

    public List<BanditAttackStep> narrativeSteps; // List of narrative steps

    private int currentStepIndex = 0; // Index of the current narrative step
    private bool isPopUpActive = false; // Flag to track if the pop-up is active

    private MoralityFriendshipManager moralityFriendshipManager; // Reference to the MoralityFriendshipManager component in the scene

    public BanditAttackData banditAttackData;
    public List<Companion> companions; // List of companions

    private void Start()
    {
        // Find the MoralityFriendshipManager component in the scene
        moralityFriendshipManager = FindObjectOfType<MoralityFriendshipManager>();
        if (moralityFriendshipManager == null)
        {
            Debug.LogError("MoralityFriendshipManager not found in the scene!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (!isPopUpActive)
            {
                Debug.Log("Player entered bandit trigger");
                StartCoroutine(StartBanditAttack());
            }
        }
    }

    private IEnumerator StartBanditAttack()
    {
        BanditAttackStep currentStep = narrativeSteps[currentStepIndex];
        isPopUpActive = true;
        popUpUI.SetActive(true);

        // Gradually display initial dialogue
        yield return StartCoroutine(ShowDialogue(new List<string> { currentStep.dialogue }));

        // Show pop-up with options
        List<BanditAttackOptionData> optionDataList = new List<BanditAttackOptionData>();
        optionDataList.AddRange(currentStep.options);
        ShowPopUp(optionDataList);
    }

    private IEnumerator ShowDialogue(List<string> dialogueLines)
    {
        foreach (string dialogue in dialogueLines)
        {
            textMeshProText.text = ""; // Clear existing text for the next line

            foreach (char letter in dialogue)
            {
                textMeshProText.text += letter; // Append characters to existing text
                yield return new WaitForSeconds(0.05f); // Adjust speed of text appearance here
            }

            // Delay between lines of dialogue
            yield return new WaitForSeconds(1f);
        }
    }

    // Show pop-up with options
    public void ShowPopUp(List<BanditAttackOptionData> options)
    {
        if (options == null)
        {
            Debug.LogWarning("Options list is null.");
            return;
        }

        isPopUpActive = true;
        popUpUI.SetActive(true);

        // Clear any existing buttons
        foreach (Transform child in optionButtonParent)
        {
            Destroy(child.gameObject);
        }

        // Present dialogue options as buttons
        PresentDialogueOptions(options);
    }

    // Method to hide the pop-up
    public void HidePopUp()
    {
        isPopUpActive = false;
        popUpUI.SetActive(false);
    }

    // Present dialogue options as buttons
    private void PresentDialogueOptions(List<BanditAttackOptionData> options)
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

    private void PlayerChoice(BanditAttackOptionData selectedOption)
    {
        HandleTraitEffects(selectedOption.traitEffects);

        // Display follow-up dialogues if available
        if (selectedOption.followUpDialogues.Count > 0)
        {
            StartCoroutine(ShowFollowUpDialogues(selectedOption.followUpDialogues));
        }
        else
        {
            // If there are no follow-up dialogues, hide the pop-up immediately
            HidePopUp();
        }

        // Destroy all child objects in the optionButtonParent
        foreach (Transform child in optionButtonParent)
        {
            Destroy(child.gameObject);
        }

        TriggerDialogue(selectedOption.option); // Trigger dialogue after handling effects
    }

    private IEnumerator ShowFollowUpDialogues(List<string> followUpDialogues)
    {
        foreach (string dialogue in followUpDialogues)
        {
            yield return StartCoroutine(ShowDialogue(new List<string> { dialogue })); // Display each follow-up dialogue
        }

        // After displaying all follow-up dialogues, hide the pop-up
        HidePopUp();
    }

    // Method to handle trait effects
    private void HandleTraitEffects(List<TraitEffect> traitEffects)
    {
        if (moralityFriendshipManager == null)
        {
            Debug.LogError("MoralityFriendshipManager not found! Cannot modify morality and friendship.");
            return;
        }

        foreach (TraitEffect effect in traitEffects)
        {
            foreach (Companion companion in companions)
            {
                if (effect.applyMoralityChangePositive || effect.applyMoralityChangeNegative)
                {
                    int moralityChange = effect.applyMoralityChangePositive ? effect.moralityChange : -effect.moralityChange;
                    moralityFriendshipManager.ModifyMorality(companion.name, moralityChange, effect.traitName);
                }

                if (effect.applyFriendshipChangePositive || effect.applyFriendshipChangeNegative)
                {
                    int friendshipChange = effect.applyFriendshipChangePositive ? effect.friendshipChange : -effect.friendshipChange;
                    moralityFriendshipManager.ModifyFriendship(companion.name, friendshipChange, effect.traitName);
                }
            }
        }
    }

    // Method to retrieve dialogue data based on selected option
    private BanditAttackDialogueData GetDialogueData(BanditAttackOption option)
    {
        foreach (BanditAttackDialogueData dialogueData in banditAttackData.dialogues)
        {
            if (dialogueData.option == option)
            {
                return dialogueData;
            }
        }
        return null;
    }

    // Method to trigger bandit dialogue based on player's choice
    public void TriggerDialogue(BanditAttackOption option)
    {
        BanditAttackDialogueData dialogueData = GetDialogueData(option);

        if (dialogueData != null)
        {
            StartCoroutine(ShowDialogue(dialogueData.dialogueLines));
        }
        else
        {
            Debug.LogWarning("No dialogue data found for the chosen option.");
        }
    }

    [System.Serializable]
    public class BanditAttackStep
    {
        public string dialogue; // Dialogue for this step
        public List<BanditAttackOptionData> options; // Options for this step

        public BanditAttackStep()
        {
            options = new List<BanditAttackOptionData>();
        }
    }
}









