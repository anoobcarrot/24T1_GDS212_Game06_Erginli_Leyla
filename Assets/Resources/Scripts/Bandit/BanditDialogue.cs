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
        Evade,
        Attack,
        Kill,
        Spare
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
        public List<BanditAttackDialogueData> followUpDialogues; // Follow-up dialogues for this option

        public BanditAttackOptionData()
        {
            traitEffects = new List<TraitEffect>();
            followUpDialogues = new List<BanditAttackDialogueData>();
        }
    }

    [System.Serializable]
    public class BanditAttackDialogueData
    {
        public List<string> dialogueLines;
        public List<BanditAttackOptionData> options; // List of options for the follow-up dialogue
        public List<TraitEffect> traitEffects;
        public bool hasOptions; // Flag to indicate whether options are available for follow-up dialogues

        public BanditAttackDialogueData(List<BanditAttackOptionData> options, List<TraitEffect> traitEffects, List<string> dialogueLines, bool hasOptions = false)
        {
            this.dialogueLines = dialogueLines;
            this.options = options;
            this.traitEffects = traitEffects;
            this.hasOptions = hasOptions;
        }
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
    private bool enteredTrigger = false;
    private bool banditNotFinished = true;

    private MoralityFriendshipManager moralityFriendshipManager; // Reference to the MoralityFriendshipManager component in the scene

    public BanditAttackData banditAttackData;
    public List<Companion> companions; // List of companions

    private bool canAttack = false;

    private void Start()
    {
        isPopUpActive = false;
        enteredTrigger = false;
        // Find the MoralityFriendshipManager component in the scene
        moralityFriendshipManager = FindObjectOfType<MoralityFriendshipManager>();
        if (moralityFriendshipManager == null)
        {
            Debug.LogError("MoralityFriendshipManager not found in the scene!");
        }

        // Check bandit health periodically
        StartCoroutine(CheckBanditHealth());
    }

    private IEnumerator CheckBanditHealth()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f); // Adjust the interval as needed

            // Check if all bandits have 10 health or lower and the pop-up hasn't been shown yet
            if (AllBanditsLowHealth() && !isPopUpActive && banditNotFinished)
            {
                isPopUpActive = true;

                // Reset text to empty string
                textMeshProText.text = "";

                // Hide any ongoing dialogue
                HideDialogue();
                // HidePopUp();

                // Wait a short moment to ensure any ongoing dialogue is hidden before showing the pop-up
                yield return new WaitForSeconds(3f);

                banditNotFinished = false;
                // Show pop-up with "Kill" or "Spare" options
                ShowKillOrSparePopUp();
            }
        }
    }

    // Method to hide the ongoing dialogue
    private void HideDialogue()
    {
        popUpUI.SetActive(false);
        // Clear text to hide the dialogue
        textMeshProText.text = "";
    }

    // Method to check if all bandits have low health
    private bool AllBanditsLowHealth()
    {
        GameObject[] bandits = GameObject.FindGameObjectsWithTag("Bandit");
        foreach (GameObject bandit in bandits)
        {
            Bandit banditComponent = bandit.GetComponent<Bandit>();
            if (banditComponent != null && banditComponent.currentHealth > 10)
            {
                return false; // At least one bandit has health above 10
            }
        }
        return true; // All bandits have health 10 or lower
    }

    [Header("Kill Option")]
    public BanditAttackOptionData killOptionData;
    public List<string> killOptionDialogues;

    [Header("Spare Option")]
    public BanditAttackOptionData spareOptionData;
    public List<string> spareOptionDialogues;

    private void ShowKillOrSparePopUp()
    {
        // Create a list of options
        List<BanditAttackOptionData> options = new List<BanditAttackOptionData>();

        // Add kill and spare options to the list
        options.Add(killOptionData);
        options.Add(spareOptionData);

        // Show the pop-up with options
        ShowPopUp(options);

        // Show dialogue lines for kill option
        StartCoroutine(ShowDialogue(killOptionDialogues));
    }

    private void ShowKillOrSparePopUp(List<string> dialogueLines)
    {
        // Use the method without dialogueLines parameter to show default dialogue
        ShowKillOrSparePopUp();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (!isPopUpActive && !enteredTrigger)
            {
                enteredTrigger = true;
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

        // Set isPopUpActive to true to indicate that the pop-up is active
        isPopUpActive = true;

        // Activate the pop-up UI
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
        // Deactivate the pop-up UI
        popUpUI.SetActive(false);
    }

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

            // Adjust button position based on vertical positioning number
            RectTransform buttonRect = button.GetComponent<RectTransform>();
            Vector2 buttonPosition = buttonRect.anchoredPosition;
            buttonPosition.y = -i * (buttonHeight + verticalSpacing);

            // Apply an offset based on the vertical positioning number
            float verticalOffset = i * 15f; // Adjust this value as needed
            buttonPosition.y += verticalOffset;

            buttonRect.anchoredPosition = buttonPosition;
        }
    }


    private void PlayerChoice(BanditAttackOptionData selectedOption)
    {
        // Handle trait effects
        HandleTraitEffects(selectedOption.traitEffects);

        // Set canAttack flag to true when the Attack option is selected
        if (selectedOption.option == BanditAttackOption.Attack)
        {
            canAttack = true;
        }
        else
        {
            canAttack = false;
        }

        // Check the selected option and perform corresponding actions
        switch (selectedOption.option)
        {
            case BanditAttackOption.Confront:
                // Handle confrontation option
                break;
            case BanditAttackOption.Negotiate:
                // Handle negotiation option
                break;
            case BanditAttackOption.Evade:
                EvadeBandits();
                break;
            case BanditAttackOption.Attack:
                // Call the Attack methods for both player and bandit
                FindObjectOfType<Bandit>().AttackPlayer();
                FindObjectOfType<PlayerMovement>().AttackBandit();
                break;
            case BanditAttackOption.Kill:
                // Call the Kill method
                KillBandits();
                break;
            case BanditAttackOption.Spare:
                // Call the Spare method
                SpareBandits();
                break;
            default:
                Debug.LogWarning("Unknown option selected.");
                break;
        }

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

        // Trigger dialogue after handling effects
        TriggerDialogue(selectedOption.option);
    }

    private IEnumerator ShowFollowUpDialogues(List<BanditAttackDialogueData> followUpDialogues)
    {
        foreach (var dialogueData in followUpDialogues)
        {
            yield return StartCoroutine(ShowDialogue(dialogueData.dialogueLines)); // Display each follow-up dialogue

            if (dialogueData.hasOptions) // If options are available for this dialogue
            {
                // Create options for the player to choose from
                ShowPopUp(dialogueData.options);

                // Wait until an option is picked
                while (!isPopUpActive)
                {
                    yield return null;
                }

                // Wait until the pop-up is hidden
                while (isPopUpActive)
                {
                    yield return null;
                }

                // If an option with hasOptions checked is encountered, break out of the loop
                break;
            }
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

    // Method to kill all bandits
    private void KillBandits()
    {
        GameObject[] bandits = GameObject.FindGameObjectsWithTag("Bandit");
        foreach (GameObject bandit in bandits)
        {
            Destroy(bandit);
            GameObject[] barriers = GameObject.FindGameObjectsWithTag("Barrier");
            foreach (GameObject barrier in barriers)
            {
                Destroy(barrier);
            }
        }
    }

    // Method to spare all bandits and destroy barriers
    private void SpareBandits()
    {
        GameObject[] bandits = GameObject.FindGameObjectsWithTag("Bandit");
        bool allLowHealth = true;
        foreach (GameObject bandit in bandits)
        {
            Bandit banditComponent = bandit.GetComponent<Bandit>();
            if (banditComponent != null && banditComponent.currentHealth > 10)
            {
                allLowHealth = false;
                break;
            }
        }

        if (allLowHealth)
        {
            GameObject[] barriers = GameObject.FindGameObjectsWithTag("Barrier");
            foreach (GameObject barrier in barriers)
            {
                Destroy(barrier);
            }
        }
    }

    // Method to spare all bandits and destroy barriers
    private void EvadeBandits()
    {
        GameObject[] barriers = GameObject.FindGameObjectsWithTag("Barrier");
        foreach (GameObject barrier in barriers)
        {
            Destroy(barrier);
        }
    }

    // Method to retrieve dialogue data based on selected option
    private BanditAttackDialogueData GetDialogueData(BanditAttackOption option)
    {
        foreach (BanditAttackDialogueData dialogueData in banditAttackData.dialogues)
        {
            // Iterate through each option in the dialogue data
            foreach (BanditAttackOptionData optionData in dialogueData.options)
            {
                // Check if the option matches the specified option
                if (optionData.option == option)
                {
                    return dialogueData;
                }
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

    // Method to set the canAttack variable
    public void SetCanAttack(bool value)
    {
        canAttack = value;
    }

    // Method to check if attacks are allowed
    public bool CanAttack()
    {
        return canAttack;
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









