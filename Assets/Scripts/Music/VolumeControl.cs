// From my Game A Week 5 Game Studio 2 Game
using UnityEngine;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    public Slider volumeSlider; // Reference to the volume slider in the Inspector

    void Start()
    {
        LoadVolumeSettings();
    }

    void LoadVolumeSettings()
    {
        // Load volume settings from PlayerPrefs if available
        if (PlayerPrefs.HasKey("Volume"))
        {
            float savedVolume = PlayerPrefs.GetFloat("Volume");
            SetVolume(savedVolume);
        }

        // Load slider position
        if (volumeSlider != null && PlayerPrefs.HasKey("SliderValue"))
        {
            float savedSliderValue = PlayerPrefs.GetFloat("SliderValue");
            volumeSlider.value = savedSliderValue;
            SetVolume(savedSliderValue); // Set the volume based on the loaded slider value
        }
    }

    void SaveVolumeSettings(float volume)
    {
        // Save volume setting
        PlayerPrefs.SetFloat("Volume", volume);
        PlayerPrefs.SetFloat("SliderValue", volumeSlider.value);
        PlayerPrefs.Save();
    }

    void OnApplicationQuit()
    {
        SaveVolumeSettings(volumeSlider.value);
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SaveVolumeSettings(volumeSlider.value);
        }
    }

    public void SetVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);
        AudioListener.volume = volume; // Adjust global volume
    }

    // Method to handle volume change event from the slider
    public void OnVolumeChanged()
    {
        if (volumeSlider != null)
        {
            SetVolume(volumeSlider.value);
            SaveVolumeSettings(volumeSlider.value);
        }
        else
        {
            Debug.LogWarning("Volume slider is not assigned.");
        }
    }
}