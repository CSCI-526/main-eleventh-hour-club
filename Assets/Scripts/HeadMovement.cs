using UnityEngine;
using System.Collections;

public class HeadMovement : MonoBehaviour
{
    public Transform targetDoor;
    public float moveSpeed = 2f;

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetDoor.position, moveSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, targetDoor.position) < 3.0f)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Level2_AvoidTheVoid"); 
        }
    }
}
