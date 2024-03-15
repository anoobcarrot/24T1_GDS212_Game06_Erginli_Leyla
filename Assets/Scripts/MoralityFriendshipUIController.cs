using UnityEngine;
using UnityEngine.UI;

public class MoralityFriendshipUIController : MonoBehaviour
{
    public CompanionDialogue companionDialogue;
    public GameObject moralityFriendshipUI;
    public Slider moralitySlider;
    public Slider friendshipSlider;

    private bool isMoralityFriendshipUIVisible = false;

    void Update()
    {
        // Check if TAB key is pressed
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleMoralityFriendshipUI();
        }
    }

    void ToggleMoralityFriendshipUI()
    {
        isMoralityFriendshipUIVisible = !isMoralityFriendshipUIVisible;
        moralityFriendshipUI.SetActive(isMoralityFriendshipUIVisible);

        if (isMoralityFriendshipUIVisible)
        {
            UpdateMoralityFriendshipUI();
        }
    }

    void UpdateMoralityFriendshipUI()
    {
        // Update morality and friendship bars
        moralitySlider.value = CalculateMoralityFill();
        friendshipSlider.value = CalculateFriendshipFill();
    }

    float CalculateMoralityFill()
    {
        // Calculate morality fill value based on the current morality value of the companion
        return (float)companionDialogue.companion.morality / 100f;
    }

    float CalculateFriendshipFill()
    {
        // Calculate friendship fill value based on the current friendship value of the companion
        return (float)companionDialogue.companion.friendship / 100f;
    }
}


