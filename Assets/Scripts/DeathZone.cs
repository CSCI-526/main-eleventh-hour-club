using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private PlayerController _playerController;
    private SendToGoogle _googleFormSender;

    void Start()
    {
        _playerController = FindObjectOfType<PlayerController>();
        _googleFormSender = FindObjectOfType<SendToGoogle>();

        if (_playerController == null)
        {
            Debug.LogError("PlayerController script not found in the scene!");
        }
        if (_googleFormSender == null)
        {
            Debug.LogError("SendToGoogle script not found in the scene!");
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"Collision detected with: {collision.gameObject.name}");

        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player fell into Death Zone! Initiating fall animation...");

            PlayerController playerController = collision.GetComponent<PlayerController>();
            if (playerController != null)
            {
                // Get the necessary data from the PlayerController
                int currentLevel = playerController.GetCurrentLevel();
                int levelCompleted = playerController.GetLevelCompletedCount();

                // Send the data to Google Forms using the updated Send method
                _googleFormSender.Send(currentLevel, 1, 0, levelCompleted); // DeathTrigger = 1, DoorReached = 0
                Debug.Log("Death Data Sent to Google Forms (from DeathZone.cs)");

                // Reduce player's life (you might still want to keep this logic for gameplay)
                //playerController.DecreaseLife();

                StartCoroutine(playerController.StartFallSequence());
            }
            else
            {
                Debug.LogError("PlayerController component NOT FOUND on colliding object!  Restart sequence will NOT run.");
            }
        }
    }


    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player exited the Death Zone.");
        }
    }


}