using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MoralityFriendshipUIController : MonoBehaviour
{
    public GameObject statsPanel;
    public Image moralityFillImage;
    public Image friendshipFillImage;
    public TextMeshProUGUI companionInfoText; // TextMeshPro component to display companion info
    public float bounceSpeed = 1f; // Speed of the bounce animation
    public float bounceHeight = 10f; // Height of the bounce animation

    private CompanionDialogue companionDialogue;

    void Start()
    {
        // Find the CompanionDialogue script
        companionDialogue = FindObjectOfType<CompanionDialogue>();
        if (companionDialogue == null)
        {
            Debug.LogError("CompanionDialogue not found in the scene.");
        }
        else
        {
            // Subscribe to morality and friendship change events
            companionDialogue.OnMoralityChange.AddListener(UpdateMoralityFill);
            companionDialogue.OnFriendshipChange.AddListener(UpdateFriendshipFill);
        }
    }

    void Update()
    {
        // Check if TAB key is pressed
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleStatsPanel();
        }

        UpdateUI();
        // Perform bounce animation
        PerformBounceAnimation();
    }

    void ToggleStatsPanel()
    {
        // Toggle the visibility of the stats panel
        statsPanel.SetActive(!statsPanel.activeSelf);

        if (statsPanel.activeSelf)
        {
            UpdateUI();
        }
    }

    void UpdateUI()
    {
        if (companionDialogue == null)
        {
            Debug.LogError("CompanionDialogue is not assigned.");
            return;
        }

        // Get initial values from CompanionDialogue
        UpdateMoralityFill(companionDialogue.companion.morality);
        UpdateFriendshipFill(companionDialogue.companion.friendship);

        // Display companion name and personality traits
        DisplayCompanionInfo();
    }

    void UpdateMoralityFill(int morality)
    {
        float normalizedMorality = Mathf.Clamp01((float)morality / 100f);
        moralityFillImage.fillAmount = normalizedMorality;
    }

    void UpdateFriendshipFill(int friendship)
    {
        float normalizedFriendship = Mathf.Clamp01((float)friendship / 100f);
        friendshipFillImage.fillAmount = normalizedFriendship;
    }

    void DisplayCompanionInfo()
    {
        if (companionDialogue != null && companionDialogue.companion != null)
        {
            Companion companion = companionDialogue.companion;

            // Display companion name and personality traits
            companionInfoText.text = "<b>Name:</b> " + companion.name + "\n";

            companionInfoText.text += "<b>Personality Traits:</b>\n";
            foreach (PersonalityTrait trait in companion.personalityTraits)
            {
                companionInfoText.text += "- " + trait.name + "\n";
            }
        }
    }

    void PerformBounceAnimation()
    {
        if (companionInfoText != null)
        {
            // Calculate bounce animation offset using sine function
            float yOffset = Mathf.Sin(Time.time * bounceSpeed) * bounceHeight;

            // Apply the offset to the text's position
            Vector3 pos = companionInfoText.transform.localPosition;
            pos.y = yOffset;
            companionInfoText.transform.localPosition = pos;
        }
    }
}





