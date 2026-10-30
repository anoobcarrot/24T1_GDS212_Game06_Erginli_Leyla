using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class UISceneTransition : MonoBehaviour
{
    [Header("Transition Settings")]
    public float transitionDuration = 1.0f;

    [Header("UI References")]
    public Image transitionImage;

    private CanvasGroup canvasGroup;
    private bool isTransitioning = false;

    private void Awake()
    {
        // Make sure the transition image is assigned.
        if (transitionImage == null)
        {
            Debug.LogError("UISceneTransition: Transition Image is missing!");
            return;
        }

        // Get or add a CanvasGroup.
        canvasGroup = transitionImage.GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = transitionImage.gameObject.AddComponent<CanvasGroup>();
        }

        // Start fully transparent.
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
    }

    public void StartTransition(string sceneName)
    {
        if (isTransitioning)
            return;

        if (canvasGroup == null)
        {
            Debug.LogError("UISceneTransition: CanvasGroup is missing!");
            return;
        }

        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("UISceneTransition: Next scene name is empty!");
            return;
        }

        isTransitioning = true;

        // Ensure time is running before starting the transition.
        Time.timeScale = 1f;

        // Preserve the transition UI across scene loading.
        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);

        StartCoroutine(Transition(sceneName));
    }

    private IEnumerator Transition(string sceneName)
    {
        canvasGroup.blocksRaycasts = true;

        // Fade to black.
        float elapsed = 0f;
        float duration = Mathf.Max(0.01f, transitionDuration);

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            canvasGroup.alpha = Mathf.Clamp01(elapsed / duration);

            yield return null;
        }

        canvasGroup.alpha = 1f;

        Debug.Log("UISceneTransition: Loading scene " + sceneName);

        // Load the next scene.
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        if (asyncLoad == null)
        {
            Debug.LogError("UISceneTransition: Failed to start loading " + sceneName);
            Destroy(gameObject);
            yield break;
        }

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // IMPORTANT: Reset time when the new scene finishes loading.
        Time.timeScale = 1f;

        Debug.Log("UISceneTransition: Scene loaded successfully!");
        Debug.Log("UISceneTransition: Time scale reset to " + Time.timeScale);

        // Fade back out.
        elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            canvasGroup.alpha = 1f - Mathf.Clamp01(elapsed / duration);

            yield return null;
        }

        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;

        Destroy(gameObject);
    }
}