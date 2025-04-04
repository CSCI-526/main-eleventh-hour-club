using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class DoorTrigger : MonoBehaviour
{
    [Tooltip("The exact scene name to load when this door is used (e.g., Level3_AvoidTheVoid)")]
    public string levelToLoad; // Set correctly in the Inspector

    [Header("UI / Interaction (Optional)")]
    [Tooltip("Assign the 'Press E / Open' button UI element, if used.")]
    public GameObject openButton;
    [Tooltip("Assign the door latch Transform to animate, if used.")]
    public Transform latch;

    private bool playerNearby = false;
    private bool isAnimating = false;

    void Start()
    {
        if (openButton != null)
        {
            openButton.SetActive(false);
        }
    }

    void Update()
    {
        // Allow triggering with 'E' key if player is nearby and if no UI button is used
        if (playerNearby && Input.GetKeyDown(KeyCode.E) && openButton == null)
        {
            TriggerLoadSequence();
        }
    }

    // Public function called by UI Button OnClick event or directly
    public void TriggerLoadSequence()
    {
        if (!isAnimating)
        {
            if (latch != null)
            {
                StartCoroutine(SmoothRotateLatchAndEnter());
            }
            else
            {
                LoadTransition();
            }
        }
    }

    private IEnumerator SmoothRotateLatchAndEnter()
    {
        isAnimating = true;

        float speed = 20f; // degrees per second
        float targetZ = 20f;

        while (Mathf.Abs(Mathf.DeltaAngle(latch.localEulerAngles.z, targetZ)) > 0.1f)
        {
            float newZ = Mathf.MoveTowardsAngle(latch.localEulerAngles.z, targetZ, speed * Time.deltaTime);
            latch.localEulerAngles = new Vector3(latch.localEulerAngles.x, latch.localEulerAngles.y, newZ);
            yield return null;
        }

        latch.localEulerAngles = new Vector3(latch.localEulerAngles.x, latch.localEulerAngles.y, targetZ);

        yield return new WaitForSeconds(0.3f);

        // Set the next level using a consistent key for TransitionManager to read
        PlayerPrefs.SetString("NextLevelToLoad", levelToLoad);
        PlayerPrefs.Save();
        SceneManager.LoadScene("Transition");
    }

    private void LoadTransition()
    {
        if (string.IsNullOrEmpty(levelToLoad))
        {
            Debug.LogError("DoorTrigger: levelToLoad scene name is not set in the Inspector!", this.gameObject);
            return;
        }

        PlayerPrefs.SetString("NextLevelToLoad", levelToLoad);
        PlayerPrefs.Save();
        Debug.Log($"DoorTrigger: Storing '{levelToLoad}' in PlayerPrefs['NextLevelToLoad'] and loading Transition scene.");

        SceneManager.LoadScene("Transition");
    }

    // Detect when the player enters/exits the trigger area
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerNearby = true;
            if (openButton != null)
            {
                openButton.SetActive(true);
                Button buttonComponent = openButton.GetComponent<Button>();
                if (buttonComponent != null)
                {
                    buttonComponent.onClick.RemoveAllListeners();
                    buttonComponent.onClick.AddListener(TriggerLoadSequence);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerNearby = false;
            if (openButton != null)
            {
                openButton.SetActive(false);
            }
        }
    }
}