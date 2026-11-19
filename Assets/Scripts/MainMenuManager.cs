using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    // Function for the START Button
    public void StartGame()
    {
        SceneManager.LoadScene("Level1");
    }

    // Function for the QUIT Button
    public void QuitGame()
    {
        Debug.Log("Application Quit");
        Application.Quit();
    }
}