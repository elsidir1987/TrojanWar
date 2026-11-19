using UnityEngine;

public class LanguageManager : MonoBehaviour
{
    void Start()
    {
        // Force the game to start in Greek every time the scene loads
        // AND tell all texts to update immediately so they don't get stuck!
        ChangeLanguage("GR");
    }

    // This is the only function the button needs to call now
    public void ToggleLanguage()
    {
        // Read the current language (default is Greek)
        string currentLanguage = PlayerPrefs.GetString("Language", "GR");

        // Swap it to the other language
        if (currentLanguage == "GR")
        {
            ChangeLanguage("EN");
        }
        else
        {
            ChangeLanguage("GR");
        }
    }

    private void ChangeLanguage(string langCode)
    {
        // 1. Save the new language
        PlayerPrefs.SetString("Language", langCode);
        PlayerPrefs.Save();

        // 2. Find ALL standard texts in the scene that have the LocalizedText script 
        LocalizedText[] allTexts = FindObjectsByType<LocalizedText>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        
        // Tell them to update immediately
        foreach (LocalizedText text in allTexts)
        {
            text.UpdateText();
        }

        // 3. Find ALL LevelDisplays in the scene (even hidden ones)
        LevelDisplay[] allLevelDisplays = FindObjectsByType<LevelDisplay>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        
        // Tell them to update immediately
        foreach (LevelDisplay ld in allLevelDisplays)
        {
            ld.UpdateLevelText();
        }
        
        Debug.Log("Language changed to: " + langCode);
    }
}