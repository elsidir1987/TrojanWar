using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager instance;

    [Header("Audio Source")]
    public AudioSource sfxSource;

    [Header("Audio Clips")]
    public AudioClip swordAttack;
    public AudioClip enemyDeath;
    public AudioClip playerHurt;
    public AudioClip buttonClick;
    public AudioClip winSound;
    public AudioClip correctQuizSound;
    public AudioClip wrongQuizSound;

    void Awake()
    {
        // Check if another SFXManager already exists
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        // Keep this object alive across all scenes
        DontDestroyOnLoad(gameObject);

        // --- NEW: Load the saved volume when the game starts! ---
        if (sfxSource != null)
        {
            // Get the saved SFX volume. If there is no save, default to 1 (100% volume)
            sfxSource.volume = PlayerPrefs.GetFloat("SFXVolume", 1f); 
        }
    }

    // Call this function from ANY script to play a sound
    public void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    // Easy function to attach to UI Buttons (On Click)
    public void PlayClickSound()
    {
        PlaySound(buttonClick);
    }
    
    // Function to adjust the volume of sound effects via UI Slider
    public void SetSFXVolume(float volume)
    {
        if (sfxSource != null)
        {
            sfxSource.volume = volume;
            
            // --- NEW: Save the volume every time you move the slider ---
            PlayerPrefs.SetFloat("SFXVolume", volume);
            PlayerPrefs.Save();
        }
    }
}