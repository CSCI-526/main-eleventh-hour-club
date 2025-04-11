using UnityEngine;

// Handles player interactions with designated "death zones" in the level.
// When the player enters the trigger collider of a death zone, their fall animation is triggered
// and the level restart logic can be handled from the PlayerController.
public class DeathZone : MonoBehaviour
{
    // Unity method called automatically when another collider marked as "Is Trigger"
    // enters this GameObject’s 2D trigger collider
    void OnTriggerEnter2D(Collider2D collision)
    {
        // Log the name of the object that triggered the collision for debugging purposes
        Debug.Log($"Collision detected with: {collision.gameObject.name}");

        // Proceed only if the object that entered the trigger has the tag "Player"
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player fell into Death Zone! Initiating fall animation...");

            // Try to get the PlayerController script attached to the player
            PlayerController playerController = collision.GetComponent<PlayerController>();
            
            if (playerController != null)
            {
                // If the player has a PlayerController script, initiate the fall sequence
                // This method typically plays the fall animation, disables controls,
                // and restarts the level or respawns the player
                playerController.StartFallSequence();
            }
            else
            {
                // Optional: log a warning if the script isn't found (for safety)
                Debug.LogWarning("Player object entered death zone but does not have a PlayerController script.");
            }
        }
    }
}
