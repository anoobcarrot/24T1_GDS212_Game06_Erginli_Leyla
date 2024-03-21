using UnityEngine;
using System.Collections;
using TMPro;

public class NPCController : MonoBehaviour
{
    public float interactRadius = 1f; // Radius for interaction with NPCs
    public KeyCode interactKey = KeyCode.Space; // Key to interact with NPCs
    public Dialogue[] npcDialogues; // Array of NPC initial dialogues
    public Dialogue[] followUpDialogues; // Array of NPC follow-up dialogues
    private int currentDialogueIndex = 0; // Index of the current dialogue line
    private bool allInitialDialoguesSaid = false; // Flag to track if all initial dialogues have been said
    public GameObject dialogueBox; // Reference to the dialogue box GameObject
    public TextMeshProUGUI dialogueText; // Reference to the TextMeshProUGUI component for displaying text
    public float textSpeed = 0.05f; // Speed at which text is displayed
    public GameObject activeObject; // Reference to the GameObject that should be toggled active/inactive

    // Reference to the currently running coroutine
    private Coroutine displayCoroutine;

    private void Update()
    {
        // Check if there's a GameObject tagged "Companion" in the scene
        GameObject companion = GameObject.FindGameObjectWithTag("Companion");

        // If there's a companion, skip NPC dialogues and directly proceed to follow-up dialogues
        if (companion != null)
        {
            if (!allInitialDialoguesSaid)
            {
                ToggleActiveObject();
                allInitialDialoguesSaid = true;
                currentDialogueIndex = npcDialogues.Length; // Skip NPC dialogues
            }
        }

        // Check if the player is within the interact radius
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, interactRadius);
        bool playerWithinInteractRadius = false;
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                playerWithinInteractRadius = true;
                break;
            }
        }

        // If the player is within the interact radius and presses the interact key
        if (playerWithinInteractRadius && Input.GetKeyDown(interactKey))
        {
            // Stop the currently running coroutine if it exists
            if (displayCoroutine != null)
            {
                StopCoroutine(displayCoroutine);
            }

            // If all initial dialogues have been said, display a follow-up dialogue
            if (allInitialDialoguesSaid)
            {
                Dialogue followUpDialogue = followUpDialogues[Random.Range(0, followUpDialogues.Length)];
                DisplayDialogue(followUpDialogue);
            }
            else
            {
                // Check if there are remaining initial dialogues to display
                if (currentDialogueIndex < npcDialogues.Length)
                {
                    DisplayDialogue(npcDialogues[currentDialogueIndex]);
                    currentDialogueIndex++;

                    if (currentDialogueIndex >= npcDialogues.Length)
                    {
                        allInitialDialoguesSaid = true;
                        ToggleActiveObject();
                    }
                }
            }
        }
        // If the player is not within the interact radius, hide the dialogue box
        else if (!playerWithinInteractRadius)
        {
            dialogueBox.SetActive(false);
        }
    }

    private void DisplayDialogue(Dialogue dialogue)
    {
        // Activate the dialogue box
        dialogueBox.SetActive(true);
        // Start displaying text gradually
        displayCoroutine = StartCoroutine(DisplayTextGradually(dialogue.text));
    }

    IEnumerator DisplayTextGradually(string text)
    {
        dialogueText.text = ""; // Clear existing text
        foreach (char letter in text)
        {
            dialogueText.text += letter; // Append the next letter
            yield return new WaitForSeconds(textSpeed); // Wait for the specified time
        }
    }

    private void ToggleActiveObject()
    {
        activeObject.SetActive(!activeObject.activeSelf);
    }

    private void OnDrawGizmosSelected()
    {
        // Draw the interact radius in the Scene view for visualization
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }
}

