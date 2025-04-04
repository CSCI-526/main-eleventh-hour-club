using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(BoxCollider2D))]
public class Door : MonoBehaviour
{
    private bool isClosing = false;
    private Vector3 initialDoorScale;
    private BoxCollider2D doorCollider;

    [Header("Animation Settings")]
    [SerializeField] private float centerPlayerDuration = 0.5f;
    [SerializeField] private float closeDelay = 0.5f;
    [SerializeField] private float closeDuration = 1.5f;
    [SerializeField] private string transitionSceneName = "Transition";
    [SerializeField] private int maxLevel = 5;

    // References
    private SendToGoogle _googleFormSender;
    private PlayerController _playerController;

    void Start()
    {
        initialDoorScale = transform.localScale;
        doorCollider = GetComponent<BoxCollider2D>();
        if (!doorCollider.isTrigger)
        {
            Debug.LogWarning("Door Collider was not set to 'Is Trigger'. Setting it now.", this);
            doorCollider.isTrigger = true;
        }

        _googleFormSender = FindObjectOfType<SendToGoogle>();
        _playerController = FindObjectOfType<PlayerController>();

        if (_googleFormSender == null) Debug.LogError("SendToGoogle script not found in the scene!", this);
        if (_playerController == null) Debug.LogError("PlayerController script not found in the scene!", this);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!isClosing && other.CompareTag("Player"))
        {
            if (IsPlayerFullyInside(other))
            {
                Debug.Log("Player fully inside door trigger. Starting sequence.");
                StartCoroutine(CenterPlayerAndCloseDoor(other.gameObject));
            }
        }
    }

    bool IsPlayerFullyInside(Collider2D playerCollider)
    {
        if (doorCollider == null || playerCollider == null) return false;
        Bounds playerBounds = playerCollider.bounds;
        Bounds doorBounds = doorCollider.bounds;
        return doorBounds.Contains(playerBounds.min) && doorBounds.Contains(playerBounds.max);
    }

    IEnumerator CenterPlayerAndCloseDoor(GameObject player)
    {
        isClosing = true;

        if (_playerController != null)
        {
            _playerController.DisableControls();
        }
        else
        {
            Debug.LogError("Cannot disable player controls - PlayerController reference missing!", this);
            yield break;
        }

        // Center the player on the door
        Vector3 startPlayerPos = player.transform.position;
        Vector3 targetPlayerPos = new Vector3(transform.position.x, player.transform.position.y, player.transform.position.z);
        float elapsedMove = 0f;
        Debug.Log("Centering player...");
        while (elapsedMove < centerPlayerDuration)
        {
            player.transform.position = Vector3.Lerp(startPlayerPos, targetPlayerPos, elapsedMove / centerPlayerDuration);
            elapsedMove += Time.deltaTime;
            yield return null;
        }
        player.transform.position = targetPlayerPos;
        Debug.Log("Player centered.");

        yield return new WaitForSeconds(closeDelay);

        // Handle level completion and analytics
        int currentLevel = _playerController.GetCurrentLevel();
        int savedCompletedCount = PlayerPrefs.GetInt("LevelCompletedCount", 0);
        int maxProgressCount = savedCompletedCount;

        if (currentLevel > savedCompletedCount)
        {
            maxProgressCount = currentLevel;
            PlayerPrefs.SetInt("LevelCompletedCount", maxProgressCount);
            PlayerPrefs.Save();
            Debug.Log($"Advanced Progress! New max level completed: {maxProgressCount} (Saved to PlayerPrefs)");
        }
        else
        {
             Debug.Log($"Replayed level {currentLevel}. Max level completed remains {savedCompletedCount}.");
        }

        if (_googleFormSender != null)
        {
            // Send analytics: deathTrigger=0, doorReached=1, levelCompleted = highest level achieved
            _googleFormSender.Send(currentLevel, 0, 1, maxProgressCount);
            Debug.Log($"Door Reached Data Sent - Finished Level: {currentLevel}, Max Level Completed: {maxProgressCount}");
        }

        // Animate door closing and player disappearing
        Debug.Log("Closing door and shrinking player...");
        Vector3 initialPlayerScale = player.transform.localScale;
        float elapsedClose = 0f;
        while (elapsedClose < closeDuration)
        {
            float progress = elapsedClose / closeDuration;
            transform.localScale = new Vector3(Mathf.Lerp(initialDoorScale.x, 0, progress), initialDoorScale.y, initialDoorScale.z);
            player.transform.localScale = new Vector3(Mathf.Lerp(initialPlayerScale.x, 0, progress), initialPlayerScale.y, initialPlayerScale.z);
            elapsedClose += Time.deltaTime;
            yield return null;
        }

        Debug.Log("Animation complete. Deactivating player and door.");
        player.SetActive(false);
        gameObject.SetActive(false);

        // Determine next scene:
        // If current level is less than maxLevel, load the next level.
        // If current level equals maxLevel, wrap around to Level 1.
        string nextSceneToLoad;
        if (currentLevel < maxLevel)
        {
            nextSceneToLoad = $"Level{currentLevel + 1}_AvoidTheVoid";
            Debug.Log($"Preparing transition to next level: {nextSceneToLoad}");
        }
        else
        {
            nextSceneToLoad = "Level1_AvoidTheVoid";
            Debug.Log($"Completed max level ({maxLevel}). Wrapping around to: {nextSceneToLoad}");
        }

        PlayerPrefs.SetString("NextLevelToLoad", nextSceneToLoad);
        PlayerPrefs.Save();
        Debug.Log($"Stored '{nextSceneToLoad}' in PlayerPrefs['NextLevelToLoad']");

        Debug.Log($"Loading transition scene: {transitionSceneName}");
        SceneManager.LoadScene(transitionSceneName);
    }
}