using UnityEngine;

public class InsideMusicController : MonoBehaviour
{
    [Header("Audio")]
    public AudioClip insideMusic;
    public AudioSource audioSource;

    [Header("Settings")]
    public bool loopMusic = true;

    private void Start()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (audioSource == null || insideMusic == null)
        {
            Debug.LogError("InsideMusicController: Missing AudioSource or AudioClip!");
            enabled = false;
            return;
        }

        audioSource.playOnAwake = false;
        audioSource.loop = loopMusic;
        audioSource.clip = insideMusic;
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
                audioSource.clip == insideMusic &&
                audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }

    private void PlayMusic()
    {
        if (audioSource == null || insideMusic == null)
            return;

        // Don't restart if this specific music is already playing.
        if (audioSource.isPlaying && audioSource.clip == insideMusic)
            return;

        audioSource.clip = insideMusic;
        audioSource.loop = loopMusic;
        audioSource.Play();
    }
}