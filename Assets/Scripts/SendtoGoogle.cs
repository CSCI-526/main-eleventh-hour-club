using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class SendToGoogle : MonoBehaviour
{
    // Assign your Google Apps Script URL in the Inspector
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
        // Ensure URL is valid before proceeding
        if (string.IsNullOrEmpty(URL))
        {
             Debug.LogError("Cannot send data: Google Form URL is not configured.");
             yield break; // Stop the coroutine
        }

        // *** ADDED: Calculate levelCompletedToSend = currentLevel - 1 ***
        int levelCompletedToSend = 0; // Default to 0
        if (int.TryParse(currentLevel, out int currentLevelInt))
        {
            // Calculate currentLevel - 1, ensuring it's not negative
            levelCompletedToSend = Mathf.Max(0, currentLevelInt - 1);
            Debug.Log($"SendToGoogle: Calculating levelCompleted parameter as currentLevel({currentLevelInt}) - 1 = {levelCompletedToSend}");
        }
        else
        {
             Debug.LogError($"SendToGoogle: Could not parse currentLevel '{currentLevel}' to an integer.");
             // Keep levelCompletedToSend = 0 or handle error as needed
        }
        // Convert the calculated value to string for the URL
        string levelCompletedParamValue = levelCompletedToSend.ToString();
        // ***************************************************************


        // Construct the final URL with query parameters matching your Google Apps Script GET parameters
        // *** MODIFIED: Using the calculated 'levelCompletedParamValue' instead of the passed 'levelCompleted' ***
        string fullUrl = URL
                         + "?sessionID=" + sessionID // Add sessionID if your script handles it
                         + "&currentLevel=" + currentLevel
                         + "&deathTrigger=" + deathTrigger
                         + "&doorReached=" + doorReached
                         + "&levelCompleted=" + levelCompletedParamValue; // Use the calculated value

        // The original 'levelCompleted' parameter passed to this coroutine is now effectively ignored.

        Debug.Log($"Sending data to Google Forms: {fullUrl}");

        // Use UnityWebRequest for GET request
        using (UnityWebRequest www = UnityWebRequest.Get(fullUrl))
        {
            // Set a buffer to receive the response (optional, but good practice)
            www.downloadHandler = new DownloadHandlerBuffer();

            // Send the request and wait for it to complete
            yield return www.SendWebRequest();

            // Check the result of the request
            #if UNITY_2020_1_OR_NEWER // Use result property for newer Unity versions
            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError || www.result == UnityWebRequest.Result.DataProcessingError)
            {
                Debug.LogError($"❌ Error sending data to Google Forms: {www.error} | Response Code: {www.responseCode}");
            }
            else
            {
                Debug.Log($"✅ Data sent successfully to Google Forms. Response: {www.downloadHandler.text}");
            }
            #else // Use older properties for versions before 2020.1
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError($"❌ Error sending data to Google Forms: {www.error} | Response Code: {www.responseCode}");
            }
            else
            {
                Debug.Log($"✅ Data sent successfully to Google Forms. Response: {www.downloadHandler.text}");
            }
            #endif
        }
    }
}