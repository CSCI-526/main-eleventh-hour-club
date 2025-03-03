using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    private bool isClosing = false;
    private Vector3 initialDoorScale;
    private float closeSpeed = 1f; 
    private float closeDelay = 0.5f; 

    void Start()
    {
        initialDoorScale = transform.localScale;
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (!isClosing && collision.CompareTag("Player"))
        {
            if (IsPlayerFullyInside(collision))
            {
                Debug.Log("Player fully inside door, centering...");
                StartCoroutine(CenterPlayerAndCloseDoor(collision.gameObject));
            }
        }
    }

    bool IsPlayerFullyInside(Collider2D playerCollider)
    {
        Bounds playerBounds = playerCollider.bounds;
        Bounds doorBounds = GetComponent<BoxCollider2D>().bounds;

        return doorBounds.Contains(playerBounds.min) && doorBounds.Contains(playerBounds.max);
    }

    IEnumerator CenterPlayerAndCloseDoor(GameObject player)
    {
        isClosing = true;
        PlayerController playerController = player.GetComponent<PlayerController>();

        if (playerController != null)
        {
            playerController.DisableControls();
        }

        // Moving player to the exact center of the door
        Vector3 targetPosition = new Vector3(transform.position.x, player.transform.position.y, player.transform.position.z);
        float moveDuration = 0.5f;
        float elapsedMove = 0f;

        while (elapsedMove < moveDuration)
        {
            player.transform.position = Vector3.Lerp(player.transform.position, targetPosition, elapsedMove / moveDuration);
            elapsedMove += Time.deltaTime;
            yield return null;
        }

        player.transform.position = targetPosition;

        Debug.Log("Player centered, closing door...");

        yield return new WaitForSeconds(closeDelay);

        Vector3 initialPlayerScale = player.transform.localScale;
        float duration = 2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float scaleFactor = Mathf.Lerp(1, 0, elapsed / duration);

            //Shrinking door vertically
            transform.localScale = new Vector3(
                Mathf.Lerp(initialDoorScale.x, 0, elapsed / duration),
                initialDoorScale.y,
                initialDoorScale.z
            );

            //Animate player disappearing from the sides
            player.transform.localScale = new Vector3(
                Mathf.Lerp(initialPlayerScale.x, 0, elapsed / duration), // Shrinks only horizontally
                initialPlayerScale.y, /* Keep height same */
                initialPlayerScale.z
            );

            elapsed += Time.deltaTime * closeSpeed;
            yield return null;
        }

        player.SetActive(false);
        gameObject.SetActive(false);
        Debug.Log("Player fully disappeared inside door!");

        Debug.Log("Level Transition...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); //Replace this with next level [TEAM]
    }
}
