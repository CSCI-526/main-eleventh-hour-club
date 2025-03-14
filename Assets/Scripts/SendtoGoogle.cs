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
        string url = "https://script.google.com/macros/s/AKfycbzmkchTYm-M_l6poTlyUMsZMnnuKLOeFkWjd2RLIEgLGuyszQ3JJonnk1TRxzhYq_fi1w/exec";  // Replace with your Apps Script URL
        Debug.Log("Sending data to Google Forms: ");
        Debug.Log("Current Level: " + currentLevel);
        Debug.Log("Death Trigger: " + deathTrigger);
        Debug.Log("Door Reached: " + doorReached);

        int levelCompletedToSend = 0;
        if (int.TryParse(currentLevel, out int currentLevelInt))
        {
            levelCompletedToSend = Mathf.Max(0, currentLevelInt - 1);
        }

        string jsonData = "{\"deathTrigger\":\"" + deathTrigger + "\", \"doorReached\":\"" + doorReached + "\", \"levelCompleted\":\"" + levelCompletedToSend + "\", \"currentLevel\":\"" + currentLevel + "\"}";
        Debug.Log("JSON Data: " + jsonData);
        Debug.Log("Session ID: " + sessionID);
        Debug.Log("URL: " + url);
        using (UnityWebRequest www = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

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