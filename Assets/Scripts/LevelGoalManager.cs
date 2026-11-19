using UnityEngine;
using UnityEngine.SceneManagement; 
using System.Collections;

public class LevelGoalManager : MonoBehaviour
{
    [Header("Level Progress Settings")]
    public int enemiesToKill = 5;           // How many enemies Ajax needs to kill 
    private int currentKills = 0;           // Counter for current kills 

    [Header("UI Settings")]
    public GameObject levelCompletePanel;   // The transparent screen to show 
    public string nextLevelName = "Level3"; // The exact name of your level 3 scene 

    // This gets called every time an enemy dies 
    public void AddKill()
    {
        currentKills++; // Add 1 to the score 
        Debug.Log("Enemies killed: " + currentKills);

        if (currentKills >= enemiesToKill)
        {
            StartCoroutine(WaitAndShowVictory());
        }
    }

    IEnumerator WaitAndShowVictory()
    {
        yield return new WaitForSeconds(1.5f); // Wait for 1 second
        Debug.Log("Enemy kill goal reached! Showing victory screen...");

        if (levelCompletePanel != null)
        {
            if (SFXManager.instance != null)
            {
                SFXManager.instance.PlaySound(SFXManager.instance.winSound);
            }
            levelCompletePanel.SetActive(true); // Show the victory screen 
             Time.timeScale = 0f; // Pause the game so enemies stop moving in the background
        }
        else
        {
            if (SFXManager.instance != null)
        {
            SFXManager.instance.PlaySound(SFXManager.instance.winSound);
        }
            Time.timeScale = 1f; // Just in case the panel is missing, unpause the game
            SceneManager.LoadScene(nextLevelName); // Load the next level immediately
        }
       
    }

}
