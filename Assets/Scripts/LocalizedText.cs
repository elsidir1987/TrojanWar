using UnityEngine;
using TMPro; // Required to use TextMeshPro

[RequireComponent(typeof(TextMeshProUGUI))]
public class LocalizedText : MonoBehaviour
{
    [Header("Translations")]
    [TextArea] public string greekText;   // The text in Greek
    [TextArea] public string englishText; // The text in English

    private TextMeshProUGUI textComponent;

    void Start()
    {
        textComponent = GetComponent<TextMeshProUGUI>();
        UpdateText();
    }

    // Call this to update the text based on the saved language
    public void UpdateText()
    {
        if (textComponent == null) return;

        // Read the saved language. If none exists, default to "GR" (Greek)
        string currentLanguage = PlayerPrefs.GetString("Language", "GR");

        // Apply the correct translation
        if (currentLanguage == "EN")
        {
            textComponent.text = englishText;
        }
        else 
        {
            textComponent.text = greekText;
        }
    }
}