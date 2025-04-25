using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class DoorTrigger : MonoBehaviour
{
    [Header("Door Settings")]
    [Tooltip("Name of the level to load after opening the door.")]
    public string levelToLoad;

    [Header("UI Elements")]
    [Tooltip("UI Button shown to open the door.")]
    public GameObject openButton;

    [Header("Latch Settings")]
    [Tooltip("Latch Transform that rotates to open the door.")]
    public Transform latch;

    private bool playerNearby = false;
    private bool isAnimating = false;
    private Button openButtonComponent;

    private void Start()
    {
        if (openButton != null)
        {
            openButton.SetActive(false);
            openButtonComponent = openButton.GetComponent<Button>();
            if (openButtonComponent == null)
            {
                Debug.LogError("OpenButton GameObject must have a Button component!");
            }
        }
        else
        {
            Debug.LogError("OpenButton reference is missing in inspector!");
        }
    }

    private void Update()
    {
        if (playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            OpenDoor();
        }
    }

    public void OpenDoor()
    {
        if (!isAnimating)
        {
            StartCoroutine(SmoothRotateLatchAndEnter());
        }
    }

    private IEnumerator SmoothRotateLatchAndEnter()
    {
        isAnimating = true;

        float speed = 100f; // Increased speed for smoother feeling
        float targetZ = 20f;
        float tolerance = 0.1f;

        while (Mathf.Abs(Mathf.DeltaAngle(latch.localEulerAngles.z, targetZ)) > tolerance)
        {
            float newZ = Mathf.MoveTowardsAngle(latch.localEulerAngles.z, targetZ, speed * Time.deltaTime);
            latch.localEulerAngles = new Vector3(
                latch.localEulerAngles.x,
                latch.localEulerAngles.y,
                newZ
            );
            yield return null;
        }

        // Snap to final angle
        latch.localEulerAngles = new Vector3(
            latch.localEulerAngles.x,
            latch.localEulerAngles.y,
            targetZ
        );

        yield return new WaitForSeconds(0.3f);

        // Save next level to load
        if (!string.IsNullOrEmpty(levelToLoad))
        {
            PlayerPrefs.SetString("NextLevel", levelToLoad);
        }
        else
        {
            Debug.LogWarning("Level to load is not set!");
        }

        // Load the transition scene
        SceneManager.LoadScene("Transition");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerNearby = true;

            if (openButton != null && openButtonComponent != null)
            {
                openButton.SetActive(true);
                openButtonComponent.onClick.RemoveAllListeners();
                openButtonComponent.onClick.AddListener(OpenDoor);
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
