using UnityEngine;

public class ForestMusicController : MonoBehaviour
{
    [Header("Audio")]
    public AudioClip forestMusic;
    public AudioSource audioSource;

    [Header("Settings")]
    public bool loopMusic = true;

    private void Start()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (audioSource == null || forestMusic == null)
        {
            Debug.LogError("ForestMusicController: Missing AudioSource or AudioClip!");
            enabled = false;
            return;
        }

        audioSource.playOnAwake = false;
        audioSource.loop = loopMusic;
        audioSource.clip = forestMusic;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            PlayMusic();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            PlayMusic();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (audioSource != null &&
                audioSource.clip == forestMusic &&
                audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }

    private void PlayMusic()
    {
        if (audioSource == null || forestMusic == null)
            return;

        // Don't restart if this specific music is already playing.
        if (audioSource.isPlaying && audioSource.clip == forestMusic)
            return;

        audioSource.clip = forestMusic;
        audioSource.loop = loopMusic;
        audioSource.Play();
    }
}