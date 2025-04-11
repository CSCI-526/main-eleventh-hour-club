using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

// Handles logic and UI while transitioning between levels
public class TransitionManager : MonoBehaviour
{
    [Header("Settings")]
    public float delayBeforeNextLevel = 3f; // Time to wait before loading the next level
    public TextMeshProUGUI levelInfoText;   // UI element to display next level info

    void Start()
    {
        // Retrieve the name of the next level from PlayerPrefs (fallback to Level 1 if missing)
        string nextLevelKey = PlayerPrefs.GetString("NextLevel", "Level1_AvoidTheVoid");

        // Generate a cleaner, user-friendly level name for display
        // e.g., "Level3_AvoidTheVoid" becomes "Level 3"
        string displayName = nextLevelKey.Replace("_AvoidTheVoid", "").Replace("Level", "Level ");

        // Show the level info if the UI element is assigned
        if (levelInfoText != null)
            levelInfoText.text = $"Get ready for {displayName}";
        else
            Debug.LogWarning("Level Info Text is not assigned in TransitionManager.");

        // Begin coroutine to load the next level after a short delay
        StartCoroutine(LoadNextLevelAfterDelay());
    }

    // Coroutine that waits and then loads the next level
    IEnumerator LoadNextLevelAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeNextLevel);

        // Re-fetch the next level in case it changed during the delay
        string nextLevel = PlayerPrefs.GetString("NextLevel", "Level1_AvoidTheVoid");
        SceneManager.LoadScene(nextLevel);
    }
}
