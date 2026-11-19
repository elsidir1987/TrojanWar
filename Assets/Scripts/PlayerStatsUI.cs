using UnityEngine;
using TMPro;
using UnityEngine.UI; 

public class PlayerStatsUI : MonoBehaviour
{
    [Header("Player Information")]
    public TextMeshProUGUI playerNameText;

    [Header("Bars (Images)")]
    public Image healthBar; 
    public Image manaBar;   

    void Start()
    {
        // 1. Get the name saved from the Login screen
        string savedName = PlayerPrefs.GetString("PlayerName", "Ήρωας");
        
        if (playerNameText != null)
        {
            playerNameText.text = savedName;
        }

        float savedHealth = PlayerPrefs.GetFloat("PlayerHealth", 3f); 
        
        if (healthBar != null) 
        {
            healthBar.fillAmount = savedHealth / 3f; 
        }

        // ------------------------------

        // 2. Initial setup for the mana bar at the start of the level
        if (manaBar != null) manaBar.fillAmount = 0f;     // Empty progress (0%)
    }
    // Call this when the player answers correctly to fill the blue bar
    public void AddProgress(float amount)
    {
        if (manaBar != null)
        {
            manaBar.fillAmount += amount; 
        }
    }

    // Call this when the player answers incorrectly to empty the red bar
    public void TakeDamage(float currentHealth)
    {


        if (healthBar != null)
        {
            healthBar.fillAmount =currentHealth / 3f;
            
            if (healthBar.fillAmount <= 0)
            {
                Debug.Log("Game Over! Player has no health left.");
            }
        }
    }
}