using UnityEngine;
using UnityEngine.UI; // Required to use Sliders

public class VolumeSettings : MonoBehaviour
{
    [Header("UI Sliders")]
    public Slider musicSlider; // Drag the Music Slider here in the Inspector
    public Slider sfxSlider;   // Drag the SFX Slider here in the Inspector

    // OnEnable is called exactly when the Settings Panel is activated/opened
    void OnEnable()
    {
        // --- SYNC MUSIC SLIDER ---
        if (musicSlider != null)
        {
            // Read the saved music volume. If none exists, default to 0.5f (50%)
            float savedMusicVol = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
            
            // Move the visual bar to the saved value WITHOUT triggering the sound update function
            musicSlider.SetValueWithoutNotify(savedMusicVol);
        }

        // --- SYNC SFX SLIDER ---
        if (sfxSlider != null)
        {
            // Read the saved SFX volume. If none exists, default to 0.5f (50%) to match SFXManager
            float savedSFXVol = PlayerPrefs.GetFloat("SFXVolume", 0.5f);
            
            // Move the visual bar to the saved value
            sfxSlider.SetValueWithoutNotify(savedSFXVol);
        }
    }

    // Updates the music volume when the player moves the Music Slider
    public void UpdateMusicVolume(float volume)
    {
        // Find the MusicManager object dynamically to prevent the "CS0117" error!
        MusicManager myMusicManager = FindFirstObjectByType<MusicManager>();
        
        if (myMusicManager != null)
        {
            myMusicManager.SetMusicVolume(volume);
            
            // Save the new music volume
            PlayerPrefs.SetFloat("MusicVolume", volume);
            PlayerPrefs.Save();
        }
    }

    // Updates the SFX volume when the player moves the SFX Slider
    public void UpdateSFXVolume(float volume)
    {
        if (SFXManager.instance != null)
        {
            SFXManager.instance.SetSFXVolume(volume);
            // (Saving is already handled inside SFXManager.SetSFXVolume)
        }
    }
}