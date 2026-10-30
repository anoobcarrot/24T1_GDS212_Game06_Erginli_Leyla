using UnityEngine;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    [Header("Volume Settings")]
    public Slider volumeSlider;

    private const string VolumeKey = "Volume";
    private const float DefaultVolume = 1f;

    // Shared between all VolumeControl instances in every scene.
    private static float sharedVolume;
    private static bool volumeInitialized = false;

    private void Awake()
    {
        InitializeVolume();
        UpdateSlider();
    }

    private void OnEnable()
    {
        InitializeVolume();
        UpdateSlider();
    }

    private void Start()
    {
        UpdateSlider();
    }

    private static void InitializeVolume()
    {
        if (!volumeInitialized)
        {
            sharedVolume = Mathf.Clamp01(
                PlayerPrefs.GetFloat(VolumeKey, DefaultVolume)
            );

            volumeInitialized = true;
        }

        AudioListener.volume = sharedVolume;
    }

    private void UpdateSlider()
    {
        if (volumeSlider != null)
        {
            volumeSlider.SetValueWithoutNotify(sharedVolume);
        }
    }

    public void SetVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);

        // Update shared volume.
        sharedVolume = volume;

        // Apply global volume.
        AudioListener.volume = sharedVolume;

        // Save shared volume.
        PlayerPrefs.SetFloat(VolumeKey, sharedVolume);
        PlayerPrefs.Save();

        // Synchronize every active volume slider.
        VolumeControl[] volumeControls =
            FindObjectsOfType<VolumeControl>(true);

        foreach (VolumeControl control in volumeControls)
        {
            control.UpdateSlider();
        }

        Debug.Log(
            "[VolumeControl] Shared volume updated: " +
            sharedVolume
        );
    }

    public void OnVolumeChanged()
    {
        if (volumeSlider != null)
        {
            SetVolume(volumeSlider.value);
        }
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            PlayerPrefs.Save();
        }
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.Save();
    }
}