using UnityEngine;

public class InsideMusicController : MonoBehaviour
{
    public AudioClip insideMusic;
    public AudioSource audioSource;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            audioSource.clip = insideMusic;
            audioSource.Play();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            audioSource.Stop();
        }
    }
}

