using System.Collections;
using UnityEngine;

public class FadeInOut : MonoBehaviour
{
    public CanvasGroup canvasGroup; // Reference to the CanvasGroup component used for fading
    public float fadeInTime = 0.1f; // Time taken to fade in (in seconds)
    public float fadeOutTime = 1f; // Time taken to fade out (in seconds)
    public float timeBetweenFades = 2f; // Time to stay black between fades (in seconds)

    private FadeInOut fadeInOut;

    private void Start()
    {
        fadeInOut = GetComponent<FadeInOut>();
        DontDestroyOnLoad(gameObject); // Prevent the GameObject from being destroyed on scene load
    }

    public void StartFade()
    {
        StartCoroutine(FadeInOutRoutine());
    }

    private IEnumerator FadeInOutRoutine()
    {
        // Fade in
        yield return Fade(canvasGroup, 0f, 1f, fadeInTime);

        // Wait while fully faded in
        yield return new WaitForSeconds(timeBetweenFades);

        // Fade out
        yield return Fade(canvasGroup, 1f, 0f, fadeOutTime);

        // Destroy the GameObject when done fading out
        Destroy(gameObject);
    }

    private IEnumerator Fade(CanvasGroup group, float startAlpha, float targetAlpha, float duration)
    {
        float startTime = Time.time;
        float endTime = startTime + duration;

        while (Time.time < endTime)
        {
            float normalizedTime = (Time.time - startTime) / duration;
            group.alpha = Mathf.Lerp(startAlpha, targetAlpha, normalizedTime);
            yield return null;
        }

        group.alpha = targetAlpha; // Ensure the target alpha is reached
    }
}


