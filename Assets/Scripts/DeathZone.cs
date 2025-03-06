using UnityEngine;

public class DeathZone : MonoBehaviour
{
    [SerializeField] private LivesUIManager livesUIManager;
    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"Collision detected with: {collision.gameObject.name}");

        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player fell into Death Zone! Initiating fall animation...");

            PlayerController playerController = collision.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.StartFallSequence();
            }

            livesUIManager.LoseLife();
        }
    }
}
