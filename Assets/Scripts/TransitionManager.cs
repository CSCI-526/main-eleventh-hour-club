using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;  // Needed for UI Text
using System.Collections;
using TMPro;


public class TransitionManager : MonoBehaviour
{
    [Header("Settings")]
    public float delayBeforeNextLevel = 3f;  // How long to wait in the transition scene
    public TextMeshProUGUI levelInfoText; // Reference to the UI Text in your Canvas

    void Start()
    {
        // Retrieve the next level name (default to Level1_AvoidTheVoid if not set)
        string nextLevel = PlayerPrefs.GetString("NextLevel", "Level1_AvoidTheVoid");

        if (nextLevel == "Level1_AvoidTheVoid")
        {
            nextLevel = "Level 1";
        }
        else if (nextLevel == "Level2_AvoidTheVoid")
        {
            nextLevel = "Level 2";
        }
        else if (nextLevel == "Level3_AvoidTheVoid")
        {
            nextLevel = "Level 3";
        }
        else if (nextLevel == "Level4_AvoidTheVoid")
        {
            nextLevel = "Level 4";
        }
        else if (nextLevel == "Level5_AvoidTheVoid")
        {
            nextLevel = "Level 5";
        }

        // Update the UI text with the next level info
        if (levelInfoText != null)
        {
            levelInfoText.text = "Get ready for " + nextLevel;
        }
        else
        {
            Debug.LogWarning("Level Info Text is not assigned in TransitionManager.");
        }

        StartCoroutine(LoadNextLevelAfterDelay());
    }

    IEnumerator LoadNextLevelAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeNextLevel);
        string nextLevel = PlayerPrefs.GetString("NextLevel", "Level1_AvoidTheVoid");
        SceneManager.LoadScene(nextLevel);
    }
}
