using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class SendToGoogle : MonoBehaviour
{
    [SerializeField] private string URL; // Google Form URL - set this in the Inspector

    private long _sessionID;

    private void Awake()
    {
        // Assign sessionID to identify playtests
        _sessionID = DateTime.Now.Ticks;
    }

    public void Send(int currentLevel, int deathTrigger, int doorReached, int levelCompleted)
    {
        StartCoroutine(Post(_sessionID.ToString(), currentLevel.ToString(), deathTrigger.ToString(), doorReached.ToString(), levelCompleted.ToString()));
    }

    private IEnumerator Post(string sessionID, string currentLevel, string deathTrigger, string doorReached, string levelCompleted)
    {
        string url = "https://script.google.com/macros/s/AKfycbwM6GFDl4gfA7RvcosUL2qFgp5oNs4IPQokark6SVHA2LVYs64SVzD3goIQQIK0y1bc4Q/exec";
    
        Debug.Log("Sending data to Google Forms:");
        Debug.Log("Current Level: " + currentLevel);
        Debug.Log("Death Trigger: " + deathTrigger);
        Debug.Log("Door Reached: " + doorReached);
    
        int levelCompletedToSend = 0;
        if (int.TryParse(currentLevel, out int currentLevelInt))
        {
            levelCompletedToSend = Mathf.Max(0, currentLevelInt - 1);
        }
    
        string fullUrl = url + "?deathTrigger=" + deathTrigger + "&doorReached=" + doorReached + "&levelCompleted=" + levelCompletedToSend + "&currentLevel=" + currentLevel;
        Debug.Log("Final URL: " + fullUrl);
    
        using (UnityWebRequest www = UnityWebRequest.Get(fullUrl))
        {
            www.downloadHandler = new DownloadHandlerBuffer();
    
            yield return www.SendWebRequest();
    
            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("✅ Data sent successfully to Google Forms: " + www.downloadHandler.text);
            }
            else
            {
                Debug.LogError("❌ Error sending data: " + www.error);
            }
        }
    }
}
