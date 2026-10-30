using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransitionController : MonoBehaviour
{
    public float transitionDuration = 1.0f; // Duration of the fade transition
    public Image transitionImage; // Reference to the UI Image used for transition
    public string targetSceneName; // Name of the scene to transition to
    public Vector3 playerSpawnPosition; // Spawn position for the player
    public Vector3 companionSpawnPosition; // Spawn position for the companion

    private CanvasGroup canvasGroup;
    private GameObject player;
    private GameObject companion;
    private Scene previousScene;

    private void Start()
    {
        // Get the CanvasGroup component attached to the transitionImage
        canvasGroup = transitionImage.GetComponent<CanvasGroup>();

        // Ensure the transition image is fully transparent at the start
        canvasGroup.alpha = 0f;

        // Get the current active scene
        previousScene = SceneManager.GetActiveScene();
    }

    public void StartTransition(GameObject player, GameObject companion)
    {
        // Start the transition coroutine
        StartCoroutine(Transition(player, companion));
        DontDestroyOnLoad(gameObject);
    }

    private IEnumerator Transition(GameObject player, GameObject companion)
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

        // Load the target scene asynchronously
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(targetSceneName, LoadSceneMode.Additive);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Set the player and companion positions in the new scene
        SceneManager.MoveGameObjectToScene(player, SceneManager.GetSceneByName(targetSceneName));
        SceneManager.MoveGameObjectToScene(companion, SceneManager.GetSceneByName(targetSceneName));
        player.transform.position = playerSpawnPosition;
        companion.transform.position = companionSpawnPosition;

        // Unload the previous scene
        SceneManager.UnloadSceneAsync(previousScene);

        // Fade back out
        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= Time.deltaTime / transitionDuration;
            yield return null;
        }

        // Ensure the transition image is fully transparent
        canvasGroup.alpha = 0f;

        // Destroy this object after the transition
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the colliding GameObject is tagged as "Player"
        if (other.CompareTag("Player"))
        {
            // Start the transition when the player enters the trigger
            player = other.gameObject;
            companion = GameObject.FindGameObjectWithTag("Companion");

            if (player != null && companion != null)
            {
                StartTransition(player, companion);
            }
            else
            {
                Debug.LogWarning("Player or companion not found.");
            }
        }
    }
}
