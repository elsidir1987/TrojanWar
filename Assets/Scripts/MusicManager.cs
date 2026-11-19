using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.SceneManagement; 
public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;
    private AudioSource musicSource;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        
        musicSource = GetComponent<AudioSource>();

        
        float savedVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        if (musicSource != null)
        {
            musicSource.volume = savedVolume;
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SyncSlider();
    }

    private void SyncSlider()
    {
        // 1. We use Resources.FindObjectsOfTypeAll to find the Slider even if the Settings Panel is hidden/inactive
        Slider[] allSliders = Resources.FindObjectsOfTypeAll<Slider>();
        
        foreach (Slider slider in allSliders)
        {
            // 2. CHECK YOUR TAG: Ensure the tag in Unity is exactly "MusicSlider" (check for typos like "MusicSlide")
            if (slider.CompareTag("MusicSlider"))
            {
                // Get the correctly saved volume from memory
                float savedVol = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
                
                // 3. IMPORTANT: We remove listeners before setting the value. 
                // This prevents the slider's default 0.5 value from accidentally 
                // calling SetMusicVolume and overwriting your saved 0.
                slider.onValueChanged.RemoveAllListeners(); 
                
                // This will automatically update both the handle position and the Fill image
                slider.value = savedVol; 
                
                // Re-enable the listener so the player can still change the volume
                slider.onValueChanged.AddListener(SetMusicVolume);
                
                Debug.Log("Successfully synced slider to: " + savedVol);
                break;
            }
        }
    }

    public void SetMusicVolume(float volume)
    {
        if (musicSource != null)
        {
            musicSource.volume = volume;
            
            
            PlayerPrefs.SetFloat("MusicVolume", volume);
            PlayerPrefs.Save();
        }
    }
}