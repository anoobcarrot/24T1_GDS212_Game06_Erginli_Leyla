using UnityEngine;
using UnityEngine.UI;

public class MoralityFriendshipUIController : MonoBehaviour
{
    public GameObject statsPanel;
    public Image moralityFillImage;
    public Image friendshipFillImage;

    private MoralityFriendshipManager moralityFriendshipManager;

    void Update()
    {
        // Check if TAB key is pressed
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleStatsPanel();
        }
    }

    void ToggleStatsPanel()
    {
        // Toggle the visibility of the stats panel
        statsPanel.SetActive(!statsPanel.activeSelf);

        if (statsPanel.activeSelf)
        {
            FindMoralityFriendshipManager();
            UpdateUI();
        }
    }

    void FindMoralityFriendshipManager()
    {
        moralityFriendshipManager = FindObjectOfType<MoralityFriendshipManager>();
        if (moralityFriendshipManager == null)
        {
            Debug.LogError("MoralityFriendshipManager not found in the scene.");
        }
    }

    void UpdateUI()
    {
        if (moralityFriendshipManager == null)
        {
            Debug.LogError("MoralityFriendshipManager is not assigned.");
            return;
        }

        Companion companion = moralityFriendshipManager.companions[0]; // Assuming there's only one companion for simplicity
        UpdateMoralityFill(companion.morality);
        UpdateFriendshipFill(companion.friendship);
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




