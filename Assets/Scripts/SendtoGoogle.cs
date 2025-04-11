using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class SendToGoogle : MonoBehaviour
{
    // Assign your Google Apps Script URL in the Inspector
    // Canvas is fixed and data is being collected correctly
    [SerializeField] private string URL = "https://script.google.com/macros/s/AKfycbwM6GFDl4gfA7RvcosUL2qFgp5oNs4IPQokark6SVHA2LVYs64SVzD3goIQQIK0y1bc4Q/exec"; // Replace with YOUR URL if different

    private long _sessionID;

    private void Awake()
    {
        // Generate a unique session ID for this play session
        _sessionID = DateTime.Now.Ticks;

        // Basic check if URL is set
        if (string.IsNullOrEmpty(URL))
        {
            Debug.LogError("Google Form URL is not set in the Inspector on SendToGoogle script!", this);
        }
    }

    // Public method called by other scripts to send data
    // Note: The 'levelCompleted' argument passed here will be IGNORED below.
    public void Send(int currentLevel, int deathTrigger, int doorReached, int levelCompleted)
    {
        // Start the coroutine to handle the web request
        // Pass all arguments as strings
        StartCoroutine(Post(_sessionID.ToString(), currentLevel.ToString(), deathTrigger.ToString(), doorReached.ToString(), levelCompleted.ToString()));
    }

    // Coroutine that handles the actual web request
    // Note: The 'levelCompleted' string parameter received here is IGNORED when building the URL.
  private IEnumerator Post(string sessionID, string currentLevel, string deathTrigger, string doorReached, string levelCompleted)
{
    if (string.IsNullOrEmpty(URL))
    {
         Debug.LogError("Cannot send data: Google Form URL is not configured.");
         yield break;
    }

    // For back button events (deathTrigger "2"), force levelCompleted to "0"
    string levelCompletedParamValue = "0";
    if (deathTrigger != "2")
    {
         if (int.TryParse(currentLevel, out int currentLevelInt))
         {
             int levelCompletedToSend = Mathf.Max(0, currentLevelInt - 1);
             levelCompletedParamValue = levelCompletedToSend.ToString();
             Debug.Log($"SendToGoogle: Calculating levelCompleted parameter as currentLevel({currentLevelInt}) - 1 = {levelCompletedParamValue}");
         }
         else
         {
             Debug.LogError($"SendToGoogle: Could not parse currentLevel '{currentLevel}' to an integer.");
         }
    }
    else
    {
         Debug.Log("SendToGoogle: Back button event detected, setting levelCompleted parameter to 0");
    }

    // Build the full URL with all parameters
    string fullUrl = URL
                     + "?sessionID=" + sessionID
                     + "&currentLevel=" + currentLevel
                     + "&deathTrigger=" + deathTrigger
                     + "&doorReached=" + doorReached
                     + "&levelCompleted=" + levelCompletedParamValue;

    Debug.Log($"Sending data to Google Forms: {fullUrl}");

    using (UnityWebRequest www = UnityWebRequest.Get(fullUrl))
    {
         www.downloadHandler = new DownloadHandlerBuffer();
         yield return www.SendWebRequest();

         if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError || www.result == UnityWebRequest.Result.DataProcessingError)
         {
             Debug.LogError($"❌ Error sending data to Google Forms: {www.error} | Response Code: {www.responseCode}");
         }
         else
         {
             Debug.Log($"✅ Data sent successfully to Google Forms. Response: {www.downloadHandler.text}");
         }
    }
}
}