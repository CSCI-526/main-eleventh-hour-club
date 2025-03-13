using UnityEngine;
using UnityEngine.SceneManagement;

public class LivesUIManager : MonoBehaviour
{
    public GameObject[] lifeFilled;
    // public GameObject[] lifeEmpty;
    private int lives = 3;

    void Start()
    {
        UpdateLivesUI();
    }

    public void LoseLife()
    {
        if (lives > 0)
        {
            lives--;
            Debug.Log($"Life lost! Remaining lives: {lives}");
            UpdateLivesUI();
        }

        if (lives <= 0)
        {
            Debug.Log("Game Over! Restarting...");
            Invoke("RestartGame", 2f);
        }
    }

    void UpdateLivesUI()
    {
        for (int i = 0; i < lifeFilled.Length; i++)
        {
            if (lifeFilled[i] != null) 
                lifeFilled[i].SetActive(i < lives); // Hide filled life if lost
            
        }
    }

    void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
