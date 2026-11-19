using UnityEngine;
using UnityEngine.SceneManagement;

public class GameFlowManager : MonoBehaviour
{
    [Header("Level UI Settings (Optional)")]
    // Drag the Instructions Panel here if the scene has one
    public GameObject instructionsPanel; 

    void Start()
    {
        // Check if the current active scene is the main menu
        if (SceneManager.GetActiveScene().name == "StartScene")
        {
            // 1. Reset the saved volume values in memory
            PlayerPrefs.SetFloat("MusicVolume", 0.5f); // 50% Music
            PlayerPrefs.SetFloat("SFXVolume", 1f);     // 100% SFX
            PlayerPrefs.SetString("Language", "GR"); // Default to Greek language
            PlayerPrefs.Save();

            // 2. Apply the default volume to the Music Manager instantly
            MusicManager myMusicManager = FindFirstObjectByType<MusicManager>();
            if (myMusicManager != null)
            {
                myMusicManager.SetMusicVolume(0.5f);
            }

            // 3. Apply the default volume to the SFX Manager instantly
            if (SFXManager.instance != null)
            {
                SFXManager.instance.SetSFXVolume(0.5f);
            }
            
            Debug.Log("StartScene loaded: Audio settings have been reset to default!");
        }
    }

    // Unpauses the game and hides the instructions
    public void StartBattle()
    {
        if (instructionsPanel != null)
        {
            instructionsPanel.SetActive(false); 
        }
        Time.timeScale = 1f; 
    }

    // ---------------- SCENE NAVIGATION ----------------

    // Loads any scene by its name. Type the name in the Button's Inspector!
    public void LoadSpecificScene(string sceneName)
    {
        Time.timeScale = 1f; // Ensure the game is unpaused before loading
        SceneManager.LoadScene(sceneName);
    }

    // Resets all stats and starts a brand new game at Level 1
    public void StartNewGame()
    {
        // Reset player stats for the new game
        PlayerPrefs.SetInt("SavedQuestionIndex", 0);
        PlayerPrefs.SetInt("QuizScorePercentage", 0);
        PlayerPrefs.SetFloat("PlayerHealth", 3f);

        
        // Save everything securely
        PlayerPrefs.Save();
        
        // Unpause the game and load the first level
        Time.timeScale = 1f;
        SceneManager.LoadScene("Level1");
    }
    // Loads the level where the player died
    public void RetrySavedLevel()
    {
        int levelToLoad = PlayerPrefs.GetInt("LastLevel", 1);
        
        if (levelToLoad == 1)
        {
            PlayerPrefs.SetInt("SavedQuestionIndex", 0); 
        }
        
        PlayerPrefs.Save();
        Time.timeScale = 1f;
        
        SceneManager.LoadScene(levelToLoad);
    }

    // ---------------- APPLICATION CONTROL ----------------

    // Closes the game entirely
    public void QuitGame()
    {
        Debug.Log("Exiting game...");
        Application.Quit();
    }
}