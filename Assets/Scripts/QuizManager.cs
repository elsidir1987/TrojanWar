using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections; 

// Wrapper class to read the JSON file
[System.Serializable]
public class QuestionDatabase
{
    public List<Question> allQuestions;
}

[System.Serializable]
public class Question
{
    [TextArea(2, 5)]
    public string questionText;
    public string answerA;
    public string answerB;
    public string answerC;
    public int correctAnswerIndex;
    public int rewardIndex; // Added to match your JSON file exactly
}

public class QuizManager : MonoBehaviour
{
    [Header("UI Connections")]
    public GameObject questionPanel;
    public TextMeshProUGUI questionTextUI;
    public TextMeshProUGUI buttonATextUI;
    public GameObject levelCompletePanel;
    public TextMeshProUGUI buttonBTextUI;
    public TextMeshProUGUI buttonCTextUI;

    [Header("Game Over - No Questions")]
    public GameObject noMoreQuestionsPanel; 

    [Header("Health & Progress System")]
    public Image healthBarFill;
    public float maxHealth = 3f;
    private float currentHealth;

    public Image manaBarFill;
    private float progressPerCorrectAnswer = 4f / 20f;

    [Header("Data-Driven Questions (Localization)")]
    public TextAsset greekQuestionFile;
    public TextAsset englishQuestionFile;
    
    // Initialized to prevent NullReferenceException
    public List<Question> questionsList = new List<Question>();
    
    // This is the GLOBAL index. It will control both Story and Bonus questions in perfect order!
    private int currentQuestionIndex = 0;
    private bool isPlayingBonus = false;

    [Header("Energy System")]
    public PlayerEnergy playerEnergy; 
    public GameObject fillEnergyButton; 

    [Header("UI Panels (Feedback)")]
    public GameObject correctScreenUI; // Panel showing "Correct!" 
    public GameObject wrongScreenUI;   // Panel showing "Wrong!" 

    [Header("Level Completion")]
    public int questonsPerLevel = 10; // How many questions must be answered to complete the level?
    private int questionsAnsweredInThisLevel = 0; // Tracks how many questions have been answered in the current level

    private bool isCheckingAnswer = false; // Prevents spam clicking buttons 

    [Header("Score & Counter UI")]
    public TextMeshProUGUI questionCounterText; // Shows "1 / 12" 
    public TextMeshProUGUI finalScoreText;      // Shows "80%" on level complete 
    
    private int totalQuestionsCount = 0;        // Total number of questions loaded 
    private int correctAnswersCount = 0;        // How many answered correctly on first try 
    private bool failedCurrentQuestion = false; // Tracks if player made a mistake on current question 

    void Start()
    {
        // Read the saved health, don't just set it to max ---
        if (PlayerPrefs.HasKey("PlayerHealth"))
        {
            currentHealth = PlayerPrefs.GetFloat("PlayerHealth");
        }
        else
        {
            currentHealth = maxHealth;
        }
        
        // Load the global progress index
        currentQuestionIndex = PlayerPrefs.GetInt("SavedQuestionIndex", 0);

        if (healthBarFill != null) healthBarFill.fillAmount = currentHealth / maxHealth;
        
        // Ensure feedback screens are closed at start 
        if (correctScreenUI != null) correctScreenUI.SetActive(false);
        if (wrongScreenUI != null) wrongScreenUI.SetActive(false);

        // Load the questions based on language
        LoadQuestionsStrictOrder();

        // Get the name of the current active scene
        string sceneName = SceneManager.GetActiveScene().name;

        // Only auto-load questions if we are in Level 1
        if (sceneName == "Level1")
        {
            if (manaBarFill != null) manaBarFill.fillAmount = 0f; 
            currentQuestionIndex = 0;
            LoadQuestion();
        }
        else
        {
            currentQuestionIndex = PlayerPrefs.GetInt("SavedQuestionIndex", 0);
            // In other Levels, keep the panel hidden at the start
            if (questionPanel != null) questionPanel.SetActive(false);
            Debug.Log("Scene is " + sceneName + ". Ready to continue questions from index: " + currentQuestionIndex);
        }
    }

    private void LoadQuestionsStrictOrder()
    {
        // 1. Find which language the player has selected
        string currentLanguage = PlayerPrefs.GetString("Language", "GR");
        TextAsset fileToLoad = (currentLanguage == "EN") ? englishQuestionFile : greekQuestionFile;

        // 2. Load the questions
        if (fileToLoad != null)
        {
            QuestionDatabase database = JsonUtility.FromJson<QuestionDatabase>(fileToLoad.text);
            questionsList = database.allQuestions;
            totalQuestionsCount = questionsList.Count; 
            Debug.Log("Loaded " + totalQuestionsCount + " questions strictly in order for language: " + currentLanguage);
        }
        else
        {
            Debug.LogError("JSON file is missing for the selected language!");
        }
    }

    public void LoadQuestion()
    {
        isPlayingBonus = false;
        failedCurrentQuestion = false; // Reset failure tracker for the new question

        if (questionsAnsweredInThisLevel < questonsPerLevel && currentQuestionIndex < questionsList.Count)
        {
            // Save index so we never lose our place
            PlayerPrefs.SetInt("SavedQuestionIndex", currentQuestionIndex);
            PlayerPrefs.Save();
            
            ShowQuestionData(questionsList[currentQuestionIndex]);
            
            // Update the counter UI (e.g. "2 / 12")
            if (questionCounterText != null)
            {   
                questionCounterText.text = (questionsAnsweredInThisLevel + 1).ToString() + " / " + questonsPerLevel.ToString();
            }
        }
        else
        {
            Debug.Log("Story Level Complete! No more main questions.");

            if (SFXManager.instance != null)
            {
                SFXManager.instance.PlaySound(SFXManager.instance.winSound);
            }

            if (questionPanel != null) questionPanel.SetActive(false);
            if (levelCompletePanel != null) levelCompletePanel.SetActive(true);
            // Calculate and show the final percentage
            CalculateAndShowFinalScore();
        }
    }

    public void LoadBonusQuestion()
    {
        // If we reached the absolute end of the JSON file
        if (currentQuestionIndex >= questionsList.Count)
        {
            // Show the forced restart panel
            if (noMoreQuestionsPanel != null)
            {
                noMoreQuestionsPanel.SetActive(true);
                Time.timeScale = 0f; // Freeze everything
            }
            return; 
        }
        
        isPlayingBonus = true;
        ShowQuestionData(questionsList[currentQuestionIndex]);
        
        if (questionCounterText != null)
        {
            questionCounterText.text = "Bonus!";
        }
    }

    private void ShowQuestionData(Question q)
    {
        if (q == null) return;

        questionTextUI.text = q.questionText;
        buttonATextUI.text = q.answerA;
        buttonBTextUI.text = q.answerB;
        buttonCTextUI.text = q.answerC;

        if (questionPanel != null) questionPanel.SetActive(true);
        Time.timeScale = 0f;
        isCheckingAnswer = false; // Allow clicking again for the new question 
    }

    public void CheckAnswer(int answerIndex)
    {
        // 1. Safety checks & Spam prevention 
        if (isCheckingAnswer) return; 
        if (currentQuestionIndex >= questionsList.Count) return;

        isCheckingAnswer = true; // Lock buttons until processing is done 
        Question q = questionsList[currentQuestionIndex];

        // 2. Determine if the answer is correct 
        bool isCorrect = (answerIndex == q.correctAnswerIndex);
        
        // 3. Start the process of showing feedback and handling logic 
        StartCoroutine(HandleAnswerRoutine(isCorrect));
    }

    private IEnumerator HandleAnswerRoutine(bool isCorrect)
    {
        string sceneName = SceneManager.GetActiveScene().name;

        // Move to the next question globally, regardless of if it's story or bonus, right or wrong.
        // This ensures the strict order is preserved forever.
        currentQuestionIndex++;
        PlayerPrefs.SetInt("SavedQuestionIndex", currentQuestionIndex);
        PlayerPrefs.Save();

        if (isCorrect)
        {
            Debug.Log("CORRECT!");

            if (SFXManager.instance != null && SFXManager.instance.correctQuizSound != null)
            {
                SFXManager.instance.PlaySound(SFXManager.instance.correctQuizSound);
            }
            
            // Show the "Correct" screen 
            if (correctScreenUI != null) correctScreenUI.SetActive(true);

            // Hide the question panel temporarily so only feedback is visible 
            if (questionPanel != null) questionPanel.SetActive(false);

            // Wait for 1.5 seconds in REAL TIME (since game time is frozen) 
            yield return new WaitForSecondsRealtime(1.5f);

            // Hide the "Correct" screen 
            if (correctScreenUI != null) correctScreenUI.SetActive(false);

            // If they didn't fail previously, give them a point! 
            if (!isPlayingBonus && !failedCurrentQuestion)
            {
                correctAnswersCount++;
            }

            // --- LOGIC AFTER CORRECT ANSWER ---

            // --- STRICTLY ENERGY REPLENISHMENT MODE ---
            if (playerEnergy != null && playerEnergy.isWaitingForBonus)
            {
                playerEnergy.RestoreEnergy();

                if(playerEnergy.IsEnergyFull())
                {
                    Debug.Log("Energy is Full! Auto-closing quiz.");
                    CloseQuizWindow();
                }
                else
                {
                    LoadBonusQuestion(); // Loads the next one in the global line
                }
                yield break; // Stop routine here
            }

            GainProgress();

            if (isPlayingBonus)
            {
                if (questionPanel != null) questionPanel.SetActive(false);
                isPlayingBonus = false;
            }
            else
            {
                questionsAnsweredInThisLevel++;
                LoadQuestion();
            }
        }
        else
        {
            Debug.Log("WRONG!");

            if (SFXManager.instance != null && SFXManager.instance.wrongQuizSound != null)
            {
                SFXManager.instance.PlaySound(SFXManager.instance.wrongQuizSound);
            }
            
            // Show the "Wrong" screen 
            if (wrongScreenUI != null) wrongScreenUI.SetActive(true);

            // Hide the question panel temporarily so only feedback is visible 
            if (questionPanel != null) questionPanel.SetActive(false);

            // Wait for 1.5 seconds in REAL TIME 
            yield return new WaitForSecondsRealtime(1.5f);

            // Hide the "Wrong" screen 
            if (wrongScreenUI != null) wrongScreenUI.SetActive(false);

            // Mark as failed so they don't get the point on the second try 
            if (!isPlayingBonus)
            {
                failedCurrentQuestion = true;
            }

            LoseHealth();

            // If player is trying to get energy, load the next question 
            if (currentHealth > 0)
            {
                if (playerEnergy != null && playerEnergy.isWaitingForBonus)
                {
                    LoadBonusQuestion(); 
                }
                else
                {
                    // In story mode, move to the next question 
                    questionsAnsweredInThisLevel++;
                    LoadQuestion();
                }
                isCheckingAnswer = false; 
            }
            else
            {
                Debug.Log("Player died during quiz! Stopping next questions.");
                yield break; 
            }
        }
    }

    private void LoseHealth()
    {
        if (healthBarFill != null)
        {
            currentHealth = Mathf.Round(healthBarFill.fillAmount * maxHealth);
        }

        currentHealth -= 1f;
        PlayerPrefs.SetFloat("PlayerHealth", currentHealth);
        PlayerPrefs.SetInt("LastLevel", SceneManager.GetActiveScene().buildIndex);
        PlayerPrefs.Save();

        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = currentHealth / maxHealth;
        }
        
        PlayerMovement ajaxMovement = FindAnyObjectByType<PlayerMovement>();
        if (ajaxMovement != null)
        {
            ajaxMovement.isBlocking = false;
            ajaxMovement.TakeDamage(1f);
        }
        else 
        {
            // If PlayerMovement is not found, still update the health bar and check for game over
            PlayerStatsUI statsUI = FindAnyObjectByType<PlayerStatsUI>();
            if (statsUI != null)
            {
                statsUI.TakeDamage(currentHealth); 
            }
        }
        
        if (currentHealth <= 0)
        {
            StopAllCoroutines(); // Stop any ongoing routines to prevent multiple triggers
            StartCoroutine(WaitAndGoToGameOver());
        }
    }

    private IEnumerator WaitAndGoToGameOver()
    {
        if (questionPanel != null) questionPanel.SetActive(false);
        isPlayingBonus = false;

        Time.timeScale = 1f;
        yield return new WaitForSecondsRealtime(0.5f);

        PlayerPrefs.SetFloat("PlayerHealth", maxHealth);
        PlayerPrefs.Save();

        SceneManager.LoadScene("GameOver");
    }

    private void GainProgress()
    {
        if (manaBarFill != null)
        {
            manaBarFill.fillAmount += progressPerCorrectAnswer;
        }
        PlayerStatsUI statsUI = FindAnyObjectByType<PlayerStatsUI>();
        if(statsUI != null)        
        {
            statsUI.AddProgress(progressPerCorrectAnswer);
        }
    }

    public void CloseQuizWindow()
    {   
        if (questionPanel != null)
        {
            questionPanel.SetActive(false);
            isPlayingBonus = false; 
            if(playerEnergy != null)
            {
                playerEnergy.isWaitingForBonus = false;
            }
            if (fillEnergyButton != null) 
            {
                fillEnergyButton.SetActive(true);
            }   

            Time.timeScale = 1f;
            Debug.Log("Quiz closed by the player.");
        }
    }

    private void CalculateAndShowFinalScore()
    {
        if (questonsPerLevel == 0) return;

        // Calculate percentage 
        float percentage = ((float)correctAnswersCount / questonsPerLevel) * 100f;
        int finalPercentage = Mathf.RoundToInt(percentage); // Round to nearest whole number 

        if (finalScoreText != null)
        {
            // Check language to display the correct end screen message
            string currentLang = PlayerPrefs.GetString("Language", "GR");
            if (currentLang == "EN")
            {
                finalScoreText.text = "Success Rate: " + finalPercentage + "%";
            }
            else
            {
                finalScoreText.text = "Ποσοστό Επιτυχίας: " + finalPercentage + "%";
            }
        }

        // Save it to PlayerPrefs so Level 2 can read it! 
        PlayerPrefs.SetInt("QuizScorePercentage", finalPercentage);
        PlayerPrefs.Save();
    }

    // This will be called by the button on the NoMoreQuestionsPanel
    public void RestartFromScratch()
    {
        // Clear all progress! 
        PlayerPrefs.DeleteAll();
        Time.timeScale = 1f; // Ensure time is running before changing scenes
        // Load the first level or Main Menu
        SceneManager.LoadScene("StartScene"); 
    }
}