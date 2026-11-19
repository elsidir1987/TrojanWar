using UnityEngine;
using TMPro;

public class PlayerSession : MonoBehaviour
{
    // Reference to the Input Field where the player types their name
    public TMP_InputField nameInputField;

    // This runs when the player clicks the "Enter" button
    public void StartSession()
    {
        string playerName = nameInputField.text;

        // Check if the player actually typed something
        if (!string.IsNullOrEmpty(playerName))
        {
            // 1. Save the name to the device's memory
            PlayerPrefs.SetString("PlayerName", playerName);
            PlayerPrefs.Save(); // Explicitly save to ensure it's written immediately
            
            Debug.Log("Player Name Saved successfully: " + playerName);
            
            // Note: Scene changing is now handled by the GameFlowManager
            // attached to the same button's OnClick event.
        }
        else
        {
            Debug.LogWarning("Please enter a name before proceeding!");
        }
    }
}