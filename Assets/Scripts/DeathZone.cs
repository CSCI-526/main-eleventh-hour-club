using UnityEngine;

// Attach this script to any GameObject that acts as a death trigger zone (e.g., bottomless pits)
[RequireComponent(typeof(Collider2D))] // Ensure a Collider2D exists
public class DeathZone : MonoBehaviour
{
    // References found in Start
    private PlayerController _playerController; // Reference to the main PlayerController script
    private SendToGoogle _googleFormSender; // Reference to the analytics sender script

    private bool playerInside = false; // Flag to prevent multiple triggers per entry

    void Start()
    {
        // Find necessary components in the scene
        // Using FindObjectOfType is okay in Start, but avoid it in Update/frequent calls
        _playerController = FindObjectOfType<PlayerController>();
        _googleFormSender = FindObjectOfType<SendToGoogle>();

        // Log errors if essential components are missing
        if (_playerController == null)
        {
            Debug.LogError("DeathZone Error: PlayerController script not found in the scene!", this);
        }
        if (_googleFormSender == null)
        {
            Debug.LogError("DeathZone Error: SendToGoogle script not found in the scene!", this);
        }

        // Ensure the collider is set to be a trigger
        Collider2D col = GetComponent<Collider2D>();
        if (col != null && !col.isTrigger)
        {
             Debug.LogWarning("DeathZone Collider was not set to 'Is Trigger'. Setting it now.", this);
             col.isTrigger = true;
        }
        else if (col == null)
        {
            Debug.LogError("DeathZone Error: No Collider2D component found on this GameObject!", this);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the colliding object is the player and if we haven't already triggered for this entry
        if (!playerInside && collision.CompareTag("Player"))
        {
            playerInside = true; // Set flag to prevent re-entry triggers immediately
            Debug.Log($"DeathZone: Player '{collision.gameObject.name}' entered trigger.");

            // Double-check we have the PlayerController reference
            if (_playerController != null)
            {
                // --- Send Analytics for Falling Death ---
                if (_googleFormSender != null)
                {
                    int currentLevel = _playerController.GetCurrentLevel();
                    // *** CORRECTED: Get completed count from PlayerPrefs ***
                    int completedCountFromPrefs = PlayerPrefs.GetInt("LevelCompletedCount", 0);

                    // Send DeathTrigger = 1 (fell), DoorReached = 0, levelCompleted = count from Prefs
                    _googleFormSender.Send(currentLevel, 1, 0, completedCountFromPrefs);
                    Debug.Log($"DeathZone Data Sent - Level: {currentLevel}, Completed Count Before Death: {completedCountFromPrefs}");
                }
                else
                {
                     Debug.LogWarning("DeathZone: SendToGoogle component not found. Cannot send death analytics.");
                }

                // --- Initiate Player's Fall Sequence ---
                Debug.Log("DeathZone: Initiating player fall sequence...");
                StartCoroutine(_playerController.StartFallSequence());
            }
            else
            {
                // This case should ideally not happen if Start() succeeded, but good failsafe
                Debug.LogError("DeathZone Error: PlayerController reference is missing when trying to trigger death sequence!", this);
                // Attempt to find PlayerController again as a last resort? Or just log the error.
                PlayerController pcOnObject = collision.GetComponent<PlayerController>();
                if (pcOnObject != null)
                {
                    Debug.LogWarning("DeathZone: Found PlayerController on colliding object directly. Attempting fall sequence.");
                     StartCoroutine(pcOnObject.StartFallSequence());
                } else {
                     Debug.LogError("DeathZone FATAL: Cannot find PlayerController to start fall sequence!");
                }
            }
        }
    }

    // Reset the flag when the player leaves the trigger zone
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("DeathZone: Player exited trigger.");
            playerInside = false; // Allow triggering again if player re-enters
        }
    }
}