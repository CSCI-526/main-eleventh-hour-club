using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

// Handles player interaction with doors that trigger level transitions
public class Door : MonoBehaviour
{
    private bool isClosing = false; // Ensures door closing logic runs only once per interaction
    private Vector3 initialDoorScale; // Stores the initial scale of the door for animation reset
    private float closeSpeed = 1f; // Speed at which the door and player shrink
    private float closeDelay = 0.5f; // Delay after centering before starting door close animation

    void Start()
    {
        // Save the door's original scale for use in the shrink animation
        initialDoorScale = transform.localScale;
    }

    // Called every frame another collider stays inside this trigger
    void OnTriggerStay2D(Collider2D collision)
    {
        // If player is inside, and door isn’t already closing, check if fully within the door bounds
        if (!isClosing && collision.CompareTag("Player") && IsPlayerFullyInside(collision))
        {
            Debug.Log("Player fully inside door, centering...");
            StartCoroutine(CenterPlayerAndCloseDoor(collision.gameObject));
        }
    }

    // Ensures entire player bounds are within the door bounds
    bool IsPlayerFullyInside(Collider2D playerCollider)
    {
        Bounds playerBounds = playerCollider.bounds;
        Bounds doorBounds = GetComponent<BoxCollider2D>().bounds;

        // Check if both corners of the player's bounds are inside the door
        return doorBounds.Contains(playerBounds.min) && doorBounds.Contains(playerBounds.max);
    }

    // Orchestrates centering player, shrinking them and the door, and loading next level
    IEnumerator CenterPlayerAndCloseDoor(GameObject player)
    {
        isClosing = true;

        // Disable player controls to prevent movement during transition
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
            playerController.DisableControls();

        // Move player to center of door (x-axis only, keep y/z for animation consistency)
        Vector3 targetPosition = new Vector3(transform.position.x, player.transform.position.y, player.transform.position.z);
        float moveDuration = 0.5f;
        float elapsedMove = 0f;

        // Smooth transition to door center using linear interpolation
        while (elapsedMove < moveDuration)
        {
            player.transform.position = Vector3.Lerp(player.transform.position, targetPosition, elapsedMove / moveDuration);
            elapsedMove += Time.deltaTime;
            yield return null;
        }

        // Ensure player is exactly centered at the end
        player.transform.position = targetPosition;

        Debug.Log("Player centered, closing door...");
        yield return new WaitForSeconds(closeDelay); // Short pause before animation

        // Store original scale of player for smooth shrink effect
        Vector3 initialPlayerScale = player.transform.localScale;
        float duration = 2f; // Total shrink animation time
        float elapsed = 0f;

        // Shrink both the door and the player over time
        while (elapsed < duration)
        {
            float progress = elapsed / duration;

            // Shrink door horizontally
            transform.localScale = new Vector3(
                Mathf.Lerp(initialDoorScale.x, 0, progress),
                initialDoorScale.y,
                initialDoorScale.z
            );

            // Shrink player horizontally (can modify to shrink vertically too if needed)
            player.transform.localScale = new Vector3(
                Mathf.Lerp(initialPlayerScale.x, 0, progress),
                initialPlayerScale.y,
                initialPlayerScale.z
            );

            elapsed += Time.deltaTime * closeSpeed;
            yield return null;
        }

        // Deactivate player and door after they disappear
        player.SetActive(false);
        gameObject.SetActive(false);
        Debug.Log("Player fully disappeared inside door!");

        // Determine which level comes next and save it using PlayerPrefs for access in the transition scene
        string nextLevel = GetNextLevel(SceneManager.GetActiveScene().name);
        PlayerPrefs.SetString("NextLevel", nextLevel);

        // Load a transition scene that can display loading UI, animation, or sound
        SceneManager.LoadScene("Transition");
    }

    // Maps current level name to next level name; cycles back to Level 1 after Level 5
    private string GetNextLevel(string current)
    {
        return current switch
        {
            "Level1_AvoidTheVoid" => "Level2_AvoidTheVoid",
            "Level2_AvoidTheVoid" => "Level3_AvoidTheVoid",
            "Level3_AvoidTheVoid" => "Level4_AvoidTheVoid",
            "Level4_AvoidTheVoid" => "Level5_AvoidTheVoid",
            _ => "Level1_AvoidTheVoid" // Default: start over if level name is unknown
        };
    }
}
