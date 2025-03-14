using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class SendToGoogle : MonoBehaviour
{
    [SerializeField] private string URL; // Google Form or Apps Script URL set in Inspector

    // Instance variables (optional; you can remove if not needed elsewhere)
    private long _sessionID;
    private int _deathTrigger;
    private int _life;
    private int _doorReached;
    private int _levelCompleted;
    private int _level;
    private float _timeSpent;

    private void Awake()
    {
        // Assign a unique session ID for each playtest or game session
        _sessionID = DateTime.Now.Ticks;
    }

    /// <summary>
    /// Sends data to Google Sheets with the specified parameters:
    ///   deathTrigger   -> 1 if player died, 0 otherwise
    ///   life           -> Player's current life count (e.g. 3, 2, 1)
    ///   doorReached    -> 1 if door reached, 0 otherwise
    ///   levelCompleted -> The "level completed" count (>= 1, resets to 1 on restart)
    ///   level          -> Which level the player is currently on
    ///   timeSpent      -> How long (in seconds) the player spent before dying or finishing
    /// </summary>
    public void Send(
        int deathTrigger,
        int life,
        int doorReached,
        int levelCompleted,
        int level,
        float timeSpent)
    {
        // Store locally if needed
        _deathTrigger   = deathTrigger;
        _life           = life;
        _doorReached    = doorReached;
        _levelCompleted = levelCompleted;
        _level          = level;
        _timeSpent      = timeSpent;

        // Format time (HH:MM:SS) or keep as float seconds—your choice
        string formattedTime = FormatTimeSpent(_timeSpent);

        StartCoroutine(Post(
            _sessionID.ToString(),
            _deathTrigger.ToString(),
            _life.ToString(),
            _doorReached.ToString(),
            _levelCompleted.ToString(),
            _level.ToString(),
            formattedTime
        ));
    }

    /// <summary>
    /// Optionally format the time spent from seconds into HH:MM:SS.
    /// </summary>
    private string FormatTimeSpent(float seconds)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
        return string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
    }

    /// <summary>
    /// Coroutine that sends the data to your Google Apps Script in JSON format.
    /// </summary>
    private IEnumerator Post(
        string sessionID,
        string deathTrigger,
        string life,
        string doorReached,
        string levelCompleted,
        string level,
        string timeSpent)
    {
        // Replace with your actual Google Apps Script URL
        string url = URL;

        // Build JSON in the exact order you want:
        // deathTrigger, life, doorReached, levelCompleted, level, timeSpent
        // (SessionID is optional to store, or use for debugging)
        string jsonData = "{" +
            "\"deathTrigger\":\""   + deathTrigger   + "\"," +
            "\"life\":\""           + life           + "\"," +
            "\"doorReached\":\""    + doorReached    + "\"," +
            "\"levelCompleted\":\"" + levelCompleted + "\"," +
            "\"level\":\""          + level          + "\"," +
            "\"timeSpent\":\""      + timeSpent      + "\"" +
        "}";

        Debug.Log("Sending JSON: " + jsonData);

        using (UnityWebRequest www = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            www.uploadHandler   = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("✅ Data sent successfully to Google Forms/Sheet: " + www.downloadHandler.text);
            }
            else
            {
                Debug.LogError("❌ Error sending data: " + www.error);
            }
        }
    }
}