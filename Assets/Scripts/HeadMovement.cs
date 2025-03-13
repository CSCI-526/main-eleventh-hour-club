using UnityEngine;
using UnityEngine.SceneManagement;

public class HeadMovement : MonoBehaviour
{
    public Transform targetDoor;   // Assign the door (or target point) in the Transition scene via the Inspector
    public float moveSpeed = 2f;     // Speed at which the head moves
    public float triggerDistance = 3f;  // Distance at which we consider the movement complete

    void Update()
    {
        // Move toward the target door position
        transform.position = Vector3.MoveTowards(transform.position, targetDoor.position, moveSpeed * Time.deltaTime);
        // When close enough, load the next level
        if (Vector3.Distance(transform.position, targetDoor.position) < triggerDistance)
        {
            // Read next level from PlayerPrefs (or set a default)
            string nextLevel = PlayerPrefs.GetString("NextLevel", "Level1_AvoidTheVoid");
            SceneManager.LoadScene(nextLevel);
        }
    }
}
