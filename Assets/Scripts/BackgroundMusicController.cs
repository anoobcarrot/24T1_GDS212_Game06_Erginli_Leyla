using UnityEngine;

public class BackgroundMusicController : MonoBehaviour
{
    public AudioClip townMusic;
    public AudioClip forestMusic;
    public AudioClip insideMusic;

    [SerializeField] private AudioSource audioSource;

    private void Start()
    {
        audioSource.loop = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Check the tag of the trigger collider
            if (gameObject.CompareTag("Town"))
            {
                audioSource.clip = townMusic;
                Debug.Log("Playing town music...");
            }
            else if (gameObject.CompareTag("Forest"))
            {
                audioSource.clip = forestMusic;
                Debug.Log("Playing forest music...");
            }
            else if (gameObject.CompareTag("Inside"))
            {
                audioSource.clip = insideMusic;
                Debug.Log("Playing inside music...");
            }

            // Play the selected music
            audioSource.Play();
        }
    }

private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StopMusic();
        }
    }

    private void StopMusic()
    {
        // Stop playing the music
        audioSource.Stop();
    }
}


