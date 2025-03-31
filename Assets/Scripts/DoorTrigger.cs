using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class DoorTrigger : MonoBehaviour
{
    public string levelToLoad;
    public GameObject openButton;
    public Transform latch;
    private bool playerNearby = false;
    private bool isAnimating = false;

    void Start()
    {
        openButton.SetActive(false);
    }

    void Update()
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

        float speed = 20f; // degrees per second
        float targetZ = 20f;

        while (Mathf.Abs(Mathf.DeltaAngle(latch.localEulerAngles.z, targetZ)) > 0.1f)
        {
            float newZ = Mathf.MoveTowardsAngle(latch.localEulerAngles.z, targetZ, speed * Time.deltaTime);
            latch.localEulerAngles = new Vector3(
                latch.localEulerAngles.x,
                latch.localEulerAngles.y,
                newZ
            );
            yield return null;
        }

        latch.localEulerAngles = new Vector3(
            latch.localEulerAngles.x,
            latch.localEulerAngles.y,
            targetZ
        );

        yield return new WaitForSeconds(0.3f);

        PlayerPrefs.SetString("NextLevel", levelToLoad);
        SceneManager.LoadScene("Transition");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerNearby = true;
            openButton.SetActive(true);
            openButton.GetComponent<Button>().onClick.RemoveAllListeners();
            openButton.GetComponent<Button>().onClick.AddListener(OpenDoor);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerNearby = false;
            openButton.SetActive(false);
        }
    }
}
