using UnityEngine;

public class MusicPreloader : MonoBehaviour
{
    [Header("Music To Preload")]
    public AudioClip mainMenu;
    public AudioClip townMusic;
    public AudioClip forestMusic;
    public AudioClip insideMusic;
    public AudioClip gameIntroBackground;
    public AudioClip gameIntro;

    private void Awake()
    {
        PreloadClip(mainMenu);
        PreloadClip(townMusic);
        PreloadClip(forestMusic);
        PreloadClip(insideMusic);
        PreloadClip(gameIntro);
        PreloadClip(gameIntroBackground);
    }

    private void PreloadClip(AudioClip clip)
    {
        if (clip == null)
            return;

        clip.LoadAudioData();

        Debug.Log(
            "MusicPreloader: Requested loading for " +
            clip.name
        );
    }
}