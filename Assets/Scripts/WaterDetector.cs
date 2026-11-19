using UnityEngine;
using UnityEngine.SceneManagement;

public class WaterDetector : MonoBehaviour
{
    [Header("Settings")]
    public float maxHealth = 3f;

    // Checks if the player hits a TRIGGER collider tagged as water
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Water"))
        {
            TriggerGameOver();
        }
    }

    // Checks if the player hits a SOLID collider tagged as water
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Water"))
        {
            TriggerGameOver();
        }
    }

    private void TriggerGameOver()
    {
        Debug.Log("Ajax fell into the water! Game Over.");

        // Reset health for the next try and save the current level 
        PlayerPrefs.SetInt("LastLevel", SceneManager.GetActiveScene().buildIndex);
        PlayerPrefs.SetFloat("PlayerHealth", maxHealth);
        PlayerPrefs.Save();

        // Load the Game Over scene instantly
        SceneManager.LoadScene("GameOver");
    }
}