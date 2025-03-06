using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartGame : MonoBehaviour
{
    public GameObject startPanel; // Reference to the Start Panel UI

   public void toggleMenu()
{
    startPanel.SetActive(!startPanel.activeSelf);
}

    public void PlayGame()
    {
        if (startPanel == null)
        {
            Debug.LogError("Start Panel is not assigned in the Inspector!");
            return;
        }

        try 
        {
            startPanel.SetActive(false);
            Debug.Log("Panel hidden successfully.");

            // Attempt to load the scene
            SceneManager.LoadScene("Scenes/Level_AvoidTheVoid");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error loading scene: {e.Message}");
        }
    }
}