using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

// Handles player interaction with doors that trigger level transitions
public class Door : MonoBehaviour
{
    private bool isClosing = false;
    private Vector3 initialDoorScale;
    private float closeSpeed = 1f;
    private float closeDelay = 0.5f;

    void Start()
    {
        initialDoorScale = transform.localScale;
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        // Start closing sequence only if player is fully inside and door isn’t already closing
        if (!isClosing && collision.CompareTag("Player") && IsPlayerFullyInside(collision))
        {
            Debug.Log("Player fully inside door, centering...");
            StartCoroutine(CenterPlayerAndCloseDoor(collision.gameObject));
        }
    }

    // Ensures entire player bounds are inside the door collider
    bool IsPlayerFullyInside(Collider2D playerCollider)
    {
        Bounds playerBounds = playerCollider.bounds;
        Bounds doorBounds = GetComponent<BoxCollider2D>().bounds;

        return doorBounds.Contains(playerBounds.min) && doorBounds.Contains(playerBounds.max);
    }

    // Handles centering the player, shrinking the door and player, and loading the next level
    IEnumerator CenterPlayerAndCloseDoor(GameObject player)
    {
        isClosing = true;
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null) playerController.DisableControls();

        // Smoothly move player to door center
        Vector3 targetPosition = new Vector3(transform.position.x, player.transform.position.y, player.transform.position.z);
        float moveDuration = 0.5f;
        float elapsedMove = 0f;

        while (elapsedMove < moveDuration)
        {
            player.transform.position = Vector3.Lerp(player.transform.position, targetPosition, elapsedMove / moveDuration);
            elapsedMove += Time.deltaTime;
            yield return null;
        }

        player.transform.position = targetPosition;

        Debug.Log("Player centered, closing door...");
        yield return new WaitForSeconds(closeDelay);

        // Animate door closing and player shrinking
        Vector3 initialPlayerScale = player.transform.localScale;
        float duration = 2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float progress = elapsed / duration;

            transform.localScale = new Vector3(
                Mathf.Lerp(initialDoorScale.x, 0, progress), initialDoorScale.y, initialDoorScale.z
            );

            player.transform.localScale = new Vector3(
                Mathf.Lerp(initialPlayerScale.x, 0, progress), initialPlayerScale.y, initialPlayerScale.z
            );

            elapsed += Time.deltaTime * closeSpeed;
            yield return null;
        }

        // Clean up after animation
        player.SetActive(false);
        gameObject.SetActive(false);
        Debug.Log("Player fully disappeared inside door!");

        // Determine and store the next level
        string nextLevel = GetNextLevel(SceneManager.GetActiveScene().name);
        PlayerPrefs.SetString("NextLevel", nextLevel);

        // Load transition scene
        SceneManager.LoadScene("Transition");
    }

    // Determines the next level based on current scene name
    private string GetNextLevel(string current)
    {
        return current switch
        {
            "Level1_AvoidTheVoid" => "Level2_AvoidTheVoid",
            "Level2_AvoidTheVoid" => "Level3_AvoidTheVoid",
            "Level3_AvoidTheVoid" => "Level4_AvoidTheVoid",
            "Level4_AvoidTheVoid" => "Level5_AvoidTheVoid",
            _ => "Level1_AvoidTheVoid" // Wraps back after Level 5
        };
    }
}

