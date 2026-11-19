using UnityEngine;
using UnityEngine.SceneManagement; 

public class PauseManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject pauseMenuPanel;
    public GameObject helpPanel;

    [Header("Quiz Protection")]
    public GameObject questionPanel; 

    private bool isPaused = false;

    void Update()
    {
        // escape button toggles pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Το κουμπί ESC πατήθηκε!");
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    // --- Button (Resume) ---
    public void ResumeGame()
    {
        pauseMenuPanel.SetActive(false);
        if (helpPanel != null) helpPanel.SetActive(false); // close help if it's open
        
        isPaused = false;

        if (questionPanel != null && questionPanel.activeInHierarchy)
        {
            
            Debug.Log("Το Quiz είναι ανοιχτό. Ο χρόνος παραμένει παγωμένος.");
            Time.timeScale = 0f; 
        }
        else
        {
            
            Time.timeScale = 1f; 
        }
    }

    public void PauseGame()
    {
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f; // pause time
        isPaused = true;
    }

    // --- Button (Help) ---
    public void OpenHelp()
    {
        if (helpPanel != null) helpPanel.SetActive(true);
    }
    public void CloseHelp()
    {
        if (helpPanel != null) helpPanel.SetActive(false); // Close the help panel
    }

    // --- Button (Exit) ---
    public void ExitToMainMenu()
    {
        Time.timeScale = 1f; // unpause the game before exiting
        SceneManager.LoadScene("StartScene"); // load the main menu scene
    }
}