using UnityEngine;
using TMPro; 
using UnityEngine.SceneManagement; 

public class LevelDisplay : MonoBehaviour
{
    public TextMeshProUGUI levelText;

    // Use OnEnable so it updates every time the UI becomes visible
    void OnEnable()
    {
        UpdateLevelText();
    }

    // Public method so the LanguageManager can call it directly
    public void UpdateLevelText()
    {
        if (levelText != null)
        {
            // Name of the current scene, e.g. "Level4"
            string sceneName = SceneManager.GetActiveScene().name;
            
            // Extract the number from the scene name by removing the "Level" prefix
            string levelNumber = sceneName.Replace("Level", "");
            
            // Read the current language setting
            string currentLanguage = PlayerPrefs.GetString("Language", "GR");
            
            // Update the text dynamically based on the selected language
            if (currentLanguage == "EN")
            {
                levelText.text = "Level " + levelNumber;
            }
            else
            {
                levelText.text = "Επίπεδο " + levelNumber;
            }
        }
    }
}