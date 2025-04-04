using UnityEngine;
using UnityEngine.SceneManagement;

public class BackButtonHandler : MonoBehaviour
{
    private SendToGoogle googleFormSender;
    private PlayerController playerController;

    void Start()
    {
        // Locate the analytics sender and player controller in the scene
        googleFormSender = FindObjectOfType<SendToGoogle>();
        playerController = FindObjectOfType<PlayerController>();
    }

    void OnMouseDown()
    {
        Debug.Log("🔙 Back button clicked!");

        int currentLevel = 0;
        if (playerController != null)
        {
            currentLevel = playerController.GetCurrentLevel();
        }
        else
        {
            Debug.LogError("PlayerController not found!");
        }

        if (googleFormSender != null)
        {
            // Send analytics: current level, deathTrigger=2, doorReached=0, levelCompleted=0
            googleFormSender.Send(currentLevel, 2, 0, 0);
            Debug.Log($"Back Button Analytics Sent - Level: {currentLevel}, DeathTrigger: 2, DoorReached: 0, LevelCompleted: 0");
        }
        else
        {
            Debug.LogError("SendToGoogle component not found in the scene!");
        }

        // Now load the level select scene
        SceneManager.LoadScene("LevelSelect");
    }
}
