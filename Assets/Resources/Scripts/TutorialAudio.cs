using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialAudio : MonoBehaviour
{
    public AudioSource audioSource; // Reference to the AudioSource component
    public string nextSceneName; // Name of the scene to transition to

    private UISceneTransition sceneTransition; // Reference to the scene transition script

    private void Start()
    {
        // Find the UISceneTransition script in the scene
        sceneTransition = FindObjectOfType<UISceneTransition>();

        // Subscribe to the audio source's event for when the audio finishes playing
        audioSource.loop = false;
        audioSource.playOnAwake = false;
        audioSource.clip = GetComponent<AudioSource>().clip;
        audioSource.Play();

        audioSource.loop = false;
        audioSource.playOnAwake = false;
        audioSource.clip = GetComponent<AudioSource>().clip;
        audioSource.Play();

        audioSource.loop = false;
        audioSource.playOnAwake = false;
        audioSource.clip = GetComponent<AudioSource>().clip;
        audioSource.Play();

        audioSource.loop = false;
        audioSource.playOnAwake = false;
        audioSource.clip = GetComponent<AudioSource>().clip;
        audioSource.Play();
    }

    private void Update()
    {
        // Check if the audio has finished playing
        if (!audioSource.isPlaying)
        {
            // Start the scene transition to the next scene
            sceneTransition.StartTransition(nextSceneName);

            // Destroy this object since it's no longer needed
            Destroy(gameObject);
        }
    }
}

