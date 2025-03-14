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
                // Send analytics data: current level, deathTrigger = 1, doorReached = 0, and time spent in level.
                _googleFormSender.Send(playerController.GetCurrentLevel(), 1, 0, playerController.GetTimeSpent());
                Debug.Log("Death Data Sent to Google Forms (from DeathZone.cs)");
                StartCoroutine(playerController.StartFallSequence());
            }
            else
            {
                Debug.LogError("PlayerController component NOT FOUND on colliding object! Restart sequence will NOT run.");
            }
        }
    }
}