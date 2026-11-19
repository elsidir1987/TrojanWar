using UnityEngine;
using UnityEngine.SceneManagement; 

public class FirstMove : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject helpPanel;       // info/help panel
    public GameObject settingsPanel;   //settings panel

    void Start()
    {
        //
        if (helpPanel != null) helpPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    // start button
    public void PlayGame()
    {
        // unpause the game and load the first level
        Time.timeScale = 1f; 
        SceneManager.LoadScene("MainMenu"); 
    }

    // --- Help Button ---
    public void OpenHelp()
    {
        if (helpPanel != null) helpPanel.SetActive(true);
    }

    public void CloseHelp()
    {
        if (helpPanel != null) helpPanel.SetActive(false);
    }

    // --- Settings Button ---
    public void OpenSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    // --- Quit Button ---
    public void QuitGame()
    {
        Debug.Log("Το παιχνίδι κλείνει!"); 
        Application.Quit();               // stop the application
    }
}