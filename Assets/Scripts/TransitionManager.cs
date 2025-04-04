using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class TransitionManager : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("How long (in seconds) to display the transition screen before loading the next level.")]
    public float delayBeforeNextLevel = 3f;

    [Header("UI References")]
    [Tooltip("Assign the TextMeshPro UI element used to display 'Get ready for Level X'.")]
    public TextMeshProUGUI levelInfoText;

    void Start()
    {
        string sceneNameToLoad = PlayerPrefs.GetString("NextLevelToLoad", "Level1_AvoidTheVoid");
        Debug.Log($"Transition Scene: Read scene name from PlayerPrefs['NextLevelToLoad']: '{sceneNameToLoad}'");

        string displayName = "Unknown Level";
        if (sceneNameToLoad == "Level1_AvoidTheVoid") { displayName = "Level 1"; }
        else if (sceneNameToLoad == "Level2_AvoidTheVoid") { displayName = "Level 2"; }
        else if (sceneNameToLoad == "Level3_AvoidTheVoid") { displayName = "Level 3"; }
        else if (sceneNameToLoad == "Level4_AvoidTheVoid") { displayName = "Level 4"; }
        else if (sceneNameToLoad == "Level5_AvoidTheVoid") { displayName = "Level 5"; }

        if (levelInfoText != null)
        {
            levelInfoText.text = "Get ready for " + displayName;
            Debug.Log($"Transition Scene: Displaying text: '{levelInfoText.text}'");
        }
        else
        {
            Debug.LogWarning("TransitionManager: Level Info Text (TextMeshProUGUI) is not assigned in the Inspector.");
        }

        StartCoroutine(LoadNextLevelAfterDelay(sceneNameToLoad));
    }

    IEnumerator LoadNextLevelAfterDelay(string sceneToLoad)
    {
        yield return new WaitForSeconds(delayBeforeNextLevel);

        Debug.Log($"Transition Scene: Delay finished. Attempting to load scene: '{sceneToLoad}'");

        if (string.IsNullOrEmpty(sceneToLoad) || sceneToLoad == "ERROR_NO_SCENE_IN_PREFS")
        {
            Debug.LogError($"Transition Scene: Invalid scene name '{sceneToLoad}' retrieved. Loading default Level 1 instead.");
            SceneManager.LoadScene("Level1_AvoidTheVoid");
        }
        else
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}