using UnityEngine;
using UnityEngine.UI;

public class MoralityFriendshipUIController : MonoBehaviour
{
    public GameObject statsPanel;
    public Image moralityFillImage;
    public Image friendshipFillImage;

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
}





