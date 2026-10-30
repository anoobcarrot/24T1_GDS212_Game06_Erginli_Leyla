using UnityEngine;

public class TownMusicController : MonoBehaviour
{
    [Header("Audio")]
    public AudioClip townMusic;
    public AudioSource audioSource;

    [Header("Settings")]
    public bool loopMusic = true;

    private void Start()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (audioSource == null || townMusic == null)
        {
            Debug.LogError("TownMusicController: Missing AudioSource or AudioClip!");
            enabled = false;
            return;
        }

        audioSource.playOnAwake = false;
        audioSource.loop = loopMusic;
        audioSource.clip = townMusic;
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
                audioSource.clip == townMusic &&
                audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }

    private void PlayMusic()
    {
        if (audioSource == null || townMusic == null)
            return;

        // Don't restart if this specific music is already playing.
        if (audioSource.isPlaying && audioSource.clip == townMusic)
            return;

        audioSource.clip = townMusic;
        audioSource.loop = loopMusic;
        audioSource.Play();
    }
}