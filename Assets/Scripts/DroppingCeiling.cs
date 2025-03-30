using UnityEngine;

public class DroppingCeiling : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D solidCollider;
    private bool hasDropped = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        BoxCollider2D[] colliders = GetComponents<BoxCollider2D>();

        // Assume first collider is solid, second is trigger
        solidCollider = colliders[0]; 
        solidCollider.isTrigger = true; // Will switch off when dropping

        rb.gravityScale = 0;
    }

    public void TriggerDropCeil()
    {
        if (!hasDropped)
        {
            hasDropped = true;
            solidCollider.isTrigger = false; // becomes solid to fall
            rb.gravityScale = 1;
            gameObject.tag = "FallingCeiling";
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Trigger collision with: " + collision.gameObject.name);

        if (collision.CompareTag("Face"))
        {
            Debug.Log("☠️ Ceiling hit Face! Breaking player apart...");
            collision.GetComponentInParent<PlayerController>().StartCoroutine("BreakPlayerApart");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("🚀 Collision Detected with: " + collision.gameObject.name);

        if (collision.gameObject.CompareTag("Ground"))
        {
            Debug.Log("✅ Ceiling hit the ground!");

            rb.linearVelocity = Vector2.zero;
            rb.gravityScale = 0;
            rb.isKinematic = true;
        }
    }
}