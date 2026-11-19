using UnityEngine;

public class QuestionTrigger : MonoBehaviour
{
    [Header("Quiz Connection")]
    public QuizManager quizManager; 
    
    // Prevents the question from popping up multiple times while the player stands on the trigger
    private bool hasBeenTriggered = false; 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Checks if the object that collided with us is the Player
        if (collision.CompareTag("Player") && !hasBeenTriggered)
        {
            hasBeenTriggered = true; // Locks the trigger so it doesn't fire again
            
            // Calls the QuizManager to display the question
            quizManager.LoadQuestion(); 
        }
    }
}