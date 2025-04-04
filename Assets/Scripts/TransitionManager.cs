using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

// Handles logic and UI while transitioning between levels
public class TransitionManager : MonoBehaviour
{
    [Header("Settings")]
    public float delayBeforeNextLevel = 3f;
    public TextMeshProUGUI levelInfoText;

    void Start()
    {
        string nextLevelKey = PlayerPrefs.GetString("NextLevel", "Level1_AvoidTheVoid");

        // Friendly display name for level
        string displayName = nextLevelKey.Replace("_AvoidTheVoid", "").Replace("Level", "Level ");

        if (levelInfoText != null)
            levelInfoText.text = $"Get ready for {displayName}";
        else
            Debug.LogWarning("Level Info Text is not assigned in TransitionManager.");

        StartCoroutine(LoadNextLevelAfterDelay());
    }

    IEnumerator LoadNextLevelAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeNextLevel);
        string nextLevel = PlayerPrefs.GetString("NextLevel", "Level1_AvoidTheVoid");
        SceneManager.LoadScene(nextLevel);
    }
}
