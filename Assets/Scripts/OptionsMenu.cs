using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    public GameObject optionsPanel; // Reference to the options panel GameObject
    public KeyCode toggleKey = KeyCode.Escape; // Keycode to toggle the options panel

    private Button[] buttons; // Array to store all buttons in the scene

    private void Start()
    {
        // Ensure the options panel is initially hidden
        optionsPanel.SetActive(false);
        // Find and store all buttons in the scene
        buttons = FindObjectsOfType<Button>();
    }

    private void Update()
    {
        // Toggle options panel visibility when the toggle key is pressed
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleOptionsMenu();
        }
    }

    public void ToggleOptionsMenu()
    {
        // Toggle the visibility of the options panel
        optionsPanel.SetActive(!optionsPanel.activeSelf);
        // Enable or disable interaction with buttons based on the options panel's state
        SetButtonsInteractable(!optionsPanel.activeSelf);
    }

    private void SetButtonsInteractable(bool isInteractable)
    {
        // Set interactability of all buttons
        foreach (Button button in buttons)
        {
            button.interactable = isInteractable;
        }
    }
}


