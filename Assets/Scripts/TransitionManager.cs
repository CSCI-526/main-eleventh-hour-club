using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;  // Required if using standard UI Text
using System.Collections;
using TMPro; // Required if using TextMeshPro

public class TransitionManager : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("How long (in seconds) to display the transition screen before loading the next level.")]
    public float delayBeforeNextLevel = 3f;

    [Header("UI References")]
    [Tooltip("Assign the TextMeshPro UI element used to display 'Get ready for Level X'.")]
    public TextMeshProUGUI levelInfoText; // Reference to the TextMeshPro UI element

    void Start()
    {
        // *** CORRECTED KEY: Use "NextLevelToLoad" to match Door.cs ***
        string sceneNameToLoad = PlayerPrefs.GetString("NextLevelToLoad", "Level1_AvoidTheVoid"); // Default if key not found

        // Add a debug log to see exactly what was retrieved
        Debug.Log($"Transition Scene: Read scene name from PlayerPrefs['NextLevelToLoad']: '{sceneNameToLoad}'");

        // --- Prepare Display Name ---
        // (This part only affects the text shown, not the scene loaded)
        string displayName = "Unknown Level"; // Default display name

        if (sceneNameToLoad == "Level1_AvoidTheVoid") { displayName = "Level 1"; }
        else if (sceneNameToLoad == "Level2_AvoidTheVoid") { displayName = "Level 2"; }
        else if (sceneNameToLoad == "Level3_AvoidTheVoid") { displayName = "Level 3"; }
        else if (sceneNameToLoad == "Level4_AvoidTheVoid") { displayName = "Level 4"; }
        else if (sceneNameToLoad == "Level5_AvoidTheVoid") { displayName = "Level 5"; }
        // Add cases for VictoryScene or other special scenes if needed
        else if (sceneNameToLoad == "VictoryScene") { displayName = "Victory!"; } // Example


        // --- Update UI Text ---
        if (levelInfoText != null)
        {
            levelInfoText.text = "Get ready for " + displayName;
            Debug.Log($"Transition Scene: Displaying text: '{levelInfoText.text}'");
        }
        else
        {
            Debug.LogWarning("TransitionManager: Level Info Text (TextMeshProUGUI) is not assigned in the Inspector.");
        }

        // --- Start Coroutine to Load ---
        // Pass the actual scene name to load to the coroutine
        StartCoroutine(LoadNextLevelAfterDelay(sceneNameToLoad));
    }

    IEnumerator LoadNextLevelAfterDelay(string sceneToLoad)
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delayBeforeNextLevel);

        // Load the scene whose name was retrieved from PlayerPrefs in Start()
        Debug.Log($"Transition Scene: Delay finished. Attempting to load scene: '{sceneToLoad}'");

        // Basic check if the scene name seems valid (optional)
        if (string.IsNullOrEmpty(sceneToLoad) || sceneToLoad == "ERROR_NO_SCENE_IN_PREFS") // Check against potential error strings
        {
             Debug.LogError($"Transition Scene: Invalid scene name '{sceneToLoad}' retrieved. Loading default Level 1 instead.");
             SceneManager.LoadScene("Level1_AvoidTheVoid"); // Load a fallback scene
        }
        else
        {
             SceneManager.LoadScene(sceneToLoad);
        }
    }
}