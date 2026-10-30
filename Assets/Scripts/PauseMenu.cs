using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject pausePanel; // Reference to the pause panel UI GameObject

    private bool isPaused = false; // Flag to track if the game is paused

    private void Update()
    {
        // Check for pause input (e.g., pressing the Escape key)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Toggle pause state
            TogglePause();
        }
    }

    private void TogglePause()
    {
        // Toggle pause state
        isPaused = !isPaused;

        if (isPaused)
        {
            // Pause the game
            Time.timeScale = 0f;

            // Show the pause panel
            if (pausePanel != null)
            {
                pausePanel.SetActive(true);
            }
        }
        else
        {
            // Resume the game
            Time.timeScale = 1f;

            // Hide the pause panel
            if (pausePanel != null)
            {
                pausePanel.SetActive(false);
            }
        }
    }
}
