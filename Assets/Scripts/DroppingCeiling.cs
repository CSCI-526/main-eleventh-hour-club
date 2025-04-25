using UnityEngine;

public class DroppingCeiling : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D solidCollider;
    private bool hasDropped = false;

    private static int s_totalDroppedCeilings = 0;


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
        bool ready = ValidateDropConditions();
        Debug.Log($"[DropCheck] Ready to drop: {ready}");

        if (!hasDropped)
        {
            hasDropped = true;

            s_totalDroppedCeilings++;
            Debug.Log($"[DroppingCeiling] Total dropped so far: {s_totalDroppedCeilings}");

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

            // —— REPORT COUNT AT TIME OF DEATH ——
            int count = GetTotalDroppedCeilings();
            Debug.Log($"[DroppingCeiling] Ceilings dropped before death: {count}");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        ReportImpactForce(collision);

        Debug.Log("🚀 Collision Detected with: " + collision.gameObject.name);

        if (collision.gameObject.CompareTag("Ground"))
        {
            Debug.Log("✅ Ceiling hit the ground!");

            rb.linearVelocity = Vector2.zero;
            rb.gravityScale = 0;
            rb.isKinematic = true;
        }
    }

    private bool ValidateDropConditions()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        float minDist = float.MaxValue;

        foreach (var p in players)
        {
            float d = Vector2.Distance(transform.position, p.transform.position);
            if (d < minDist) minDist = d;
        }

        Debug.Log($"[ValidateDropConditions] Nearest player at {minDist:F2} units.");
        return true;
    }

    private void ReportImpactForce(Collision2D collision)
    {
        float impact = collision.relativeVelocity.magnitude * rb.mass;
        Debug.Log($"[ReportImpactForce] Approx. impact force: {impact:F1}");
    }

    public static int GetTotalDroppedCeilings()
    {
        return s_totalDroppedCeilings;
    }


}