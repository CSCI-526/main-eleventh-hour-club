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
        if (!isClosing && collision.CompareTag("Player") && IsPlayerFullyInside(collision))
        {
            Debug.Log("Player fully inside door, centering...");
            StartCoroutine(CenterPlayerAndCloseDoor(collision.gameObject));
        }
    }

    bool IsPlayerFullyInside(Collider2D playerCollider)
    {
        Bounds playerBounds = playerCollider.bounds;
        Bounds doorBounds = GetComponent<BoxCollider2D>().bounds;

        return doorBounds.Contains(playerBounds.min) && doorBounds.Contains(playerBounds.max);
    }

    IEnumerator CenterPlayerAndCloseDoor(GameObject player)
    {
        isClosing = true;

        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
            playerController.DisableControls();

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

        Vector3 initialPlayerScale = player.transform.localScale;
        float duration = 2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float progress = elapsed / duration;

            transform.localScale = new Vector3(
                Mathf.Lerp(initialDoorScale.x, 0, progress),
                initialDoorScale.y,
                initialDoorScale.z
            );

            player.transform.localScale = new Vector3(
                Mathf.Lerp(initialPlayerScale.x, 0, progress),
                initialPlayerScale.y,
                initialPlayerScale.z
            );

            elapsed += Time.deltaTime * closeSpeed;
            yield return null;
        }

        player.SetActive(false);
        gameObject.SetActive(false);
        Debug.Log("Player fully disappeared inside door!");

        string nextLevel = GetNextLevel(SceneManager.GetActiveScene().name);
        PlayerPrefs.SetString("NextLevel", nextLevel);

        SceneManager.LoadScene("Transition");
    }

    private string GetNextLevel(string current)
    {
        // Assumes your level names are "Level1_AvoidTheVoid", "Level2_AvoidTheVoid", etc.
        string baseName = "Level";
        string suffix = "_AvoidTheVoid";

        // Try to extract the level number
        if (current.StartsWith(baseName) && current.EndsWith(suffix))
        {
            string numberPart = current.Substring(baseName.Length, current.Length - baseName.Length - suffix.Length);

            if (int.TryParse(numberPart, out int currentLevel))
            {
                int nextLevel = currentLevel + 1;

                // If you only have 7 levels, cycle back to Level 1 after Level 7
                if (nextLevel > 7)
                    nextLevel = 1;

                return $"{baseName}{nextLevel}{suffix}";
            }
        }

        // If anything goes wrong, fall back to Level 1
        Debug.LogWarning($"Unexpected level name format: {current}. Defaulting to Level1_AvoidTheVoid.");
        return "Level1_AvoidTheVoid";
    }
}
