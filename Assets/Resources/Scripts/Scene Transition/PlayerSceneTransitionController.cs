using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerSceneTransitionController : MonoBehaviour
{
    public float transitionDuration = 1.0f; // Duration of the fade transition
    public Image transitionImage; // Reference to the UI Image used for transition
    public Vector3 playerSpawnPosition; // Spawn position for the player
    public Vector3 companionSpawnPosition; // Spawn position for the companion

    private CanvasGroup canvasGroup;
    private GameObject player;
    private GameObject companion;

    private void Start()
    {
        // Get the CanvasGroup component attached to the transitionImage
        canvasGroup = transitionImage.GetComponent<CanvasGroup>();

        // Ensure the transition image is fully transparent at the start
        canvasGroup.alpha = 0f;
    }

    public void StartTransition(GameObject playerObject, GameObject companionObject)
    {
        // Start the transition coroutine
        StartCoroutine(Transition(playerObject, companionObject));
    }

    private IEnumerator Transition(GameObject playerObject, GameObject companionObject)
    {
        // Fade to black
        while (canvasGroup.alpha < 1)
        {
            Time.timeScale = 1;
            canvasGroup.alpha += Time.deltaTime / transitionDuration;
            yield return null;
        }

        // Ensure the transition image is fully opaque
        canvasGroup.alpha = 1f;

        // Set the player position
        if (playerObject != null)
        {
            playerObject.transform.position = playerSpawnPosition;
        }
        else
        {
            Debug.LogWarning("Player is null. Cannot move player to the target position.");
        }

        // Set the companion position if it exists
        if (companionObject != null)
        {
            companionObject.transform.position = companionSpawnPosition;
        }

        // Fade back out
        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= Time.deltaTime / transitionDuration;
            yield return null;
        }

        // Ensure the transition image is fully transparent
        canvasGroup.alpha = 0f;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the colliding GameObject is tagged as "Player"
        if (other.CompareTag("Player"))
        {
            // Start the transition when the player enters the trigger
            player = other.gameObject;
            companion = GameObject.FindGameObjectWithTag("Companion");

            // Start the transition with or without the companion
            StartTransition(player, companion);
        }
    }
}


