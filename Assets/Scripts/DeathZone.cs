using UnityEngine;

// Handles player interactions with death zones in the level
public class DeathZone : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"Collision detected with: {collision.gameObject.name}");

        // Only trigger if the player enters
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player fell into Death Zone! Initiating fall animation...");

            PlayerController playerController = collision.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.StartFallSequence(); // Triggers player's fall and restart logic
            }
        }
    }
}
