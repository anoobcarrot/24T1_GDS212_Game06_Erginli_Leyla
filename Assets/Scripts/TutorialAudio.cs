using UnityEngine;
using System.Collections;

public class TutorialAudio : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;

    [Header("Scene Transition")]
    [SerializeField] private UISceneTransition sceneTransition;
    [SerializeField] private string nextSceneName;

    [Header("Transition Timing")]
    [Tooltip("How many seconds to wait AFTER the audio finishes before transitioning.")]
    [Min(0f)]
    [SerializeField] private float transitionAfterTime = 1f;

    private bool hasTransitioned = false;

    private void Start()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (audioSource == null || audioSource.clip == null)
        {
            Debug.LogError("TutorialAudio: AudioSource or clip is missing!");
            return;
        }

        if (sceneTransition == null)
        {
            Debug.LogError("TutorialAudio: SceneTransition is not assigned!");
            return;
        }

        audioSource.loop = false;
        audioSource.playOnAwake = false;

        StartCoroutine(PlayTutorialAudio());
    }

    private IEnumerator PlayTutorialAudio()
    {
        Debug.Log("TutorialAudio: Starting audio.");

        audioSource.Play();

        float audioLength = audioSource.clip.length;

        Debug.Log("TutorialAudio: Clip length = " + audioLength);

        // Wait for the full audio duration.
        yield return new WaitForSecondsRealtime(audioLength);

        Debug.Log("TutorialAudio: Audio timer finished.");

        // Wait additional seconds AFTER the audio finishes.
        if (transitionAfterTime > 0f)
        {
            Debug.Log("TutorialAudio: Waiting an additional " +
                      transitionAfterTime + " seconds.");

            yield return new WaitForSecondsRealtime(transitionAfterTime);
        }

        if (hasTransitioned)
            yield break;

        hasTransitioned = true;

        if (sceneTransition == null)
        {
            Debug.LogError("TutorialAudio: Transition reference was destroyed!");
            yield break;
        }

        Debug.Log("TutorialAudio: Starting transition to " + nextSceneName);

        sceneTransition.StartTransition(nextSceneName);
    }
}