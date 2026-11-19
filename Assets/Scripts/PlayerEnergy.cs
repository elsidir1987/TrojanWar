using UnityEngine;
using UnityEngine.UI;

public class PlayerEnergy : MonoBehaviour
{
    public Image energyBarFill;
    public GameObject warningCanvas; 
    public GameObject fillEnergyButton;
    
    public int maxEnergySlots = 20;
    public int energyRestoredPerAnswer = 5; 
    private int currentEnergySlots;

    [HideInInspector] public bool isWaitingForBonus = false;

    void Start()
    {
        currentEnergySlots = maxEnergySlots;
        UpdateEnergyBar();
    }

    // Checks if player has enough energy to perform an action
    public bool CanPerformAction()
    {
        return currentEnergySlots > 0;
    }

    // Reduces energy and triggers warning if it drops to 0
    public void UseEnergy()
    {
        if (currentEnergySlots > 0)
        {
            currentEnergySlots--;
            UpdateEnergyBar();
            
            if (currentEnergySlots <= 0)
            {
                isWaitingForBonus = true; 
                ShowWarning();
            }
        }
    }

    // Shows the "Out of Energy" warning panel
    void ShowWarning()
    {
        if (warningCanvas != null)
        {
            warningCanvas.SetActive(true); // Show the warning UI
            
            Time.timeScale = 0f; // Freeze the game
        }
    }

    // Called by the "Let's Go!" button on the warning panel
    public void StartBonusQuiz()
    {
        if (warningCanvas != null) warningCanvas.SetActive(false); 
        if (fillEnergyButton != null) fillEnergyButton.SetActive(false); // Hide the manual refill button during the quiz

        Time.timeScale = 1f; // Unfreeze the game so the quiz can run
        
        // Loads and opens the actual question panel
        FindAnyObjectByType<QuizManager>().LoadBonusQuestion(); 
    }
    
    // Called WHEN THE ANSWER IS CORRECT 
    public void RestoreEnergy()
    {
        currentEnergySlots += energyRestoredPerAnswer;
        if(currentEnergySlots > maxEnergySlots)
        {
            currentEnergySlots = maxEnergySlots;
        }
        UpdateEnergyBar();
        
        Time.timeScale = 1f; // Unfreeze the game
    }

    // Updates the visual UI bar
    void UpdateEnergyBar()
    {
        if (energyBarFill != null)
            energyBarFill.fillAmount = (float)currentEnergySlots / maxEnergySlots;
    }

    // Check if the energy bar is completely full 
    public bool IsEnergyFull()
    {
        return currentEnergySlots >= maxEnergySlots;
    }

    // Called by the manual "+" (Fill Energy) button 
    public void ManualEnergyRefill()
    {
        if (!IsEnergyFull())
        {
            if (fillEnergyButton != null) fillEnergyButton.SetActive(false);
            
            isWaitingForBonus = true; 
            FindAnyObjectByType<QuizManager>().LoadBonusQuestion();
        }
        else
        {
            Debug.Log("Energy is already full! No need to answer questions.");
        }
    }
}