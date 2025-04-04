using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

// Handles all player control, animations, death, and environmental triggers
public class PlayerController : MonoBehaviour
{
    public float speed = 3f;
    public float jumpForce = 6f;

    private Rigidbody2D rb;
    private Collider2D playerCollider;
    private bool isGrounded;
    private bool isFalling = false;

    // Body part references for animation
    private Transform leftLeg, rightLeg, leftHand, rightHand, face, body;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();

        // Find body parts for animations
        leftLeg = transform.Find("LeftLeg/LeftLegSprite");
        rightLeg = transform.Find("RightLeg/RightLegSprite");
        leftHand = transform.Find("Hands/LeftHand");
        rightHand = transform.Find("Hands/RightHand");
        face = transform.Find("Face");
        body = transform.Find("Body");
    }

    void Update()
    {
        if (!enabled || isFalling) return;

        float move = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(move * speed, rb.velocity.y);

        // Handle jumping
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetAxisRaw("Vertical") > 0) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            isGrounded = false;
        }

        AnimateLegs(move);
    }

    void AnimateLegs(float move)
    {
        if (Mathf.Abs(move) > 0.1f)
        {
            float legAngle = Mathf.Sin(Time.time * 10) * 20;
            leftLeg.rotation = Quaternion.Euler(0, 0, legAngle);
            rightLeg.rotation = Quaternion.Euler(0, 0, -legAngle);
        }
        else
        {
            leftLeg.rotation = Quaternion.identity;
            rightLeg.rotation = Quaternion.identity;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // General death zones
        if (collision.CompareTag("DeathZone") || collision.CompareTag("DeathZoneForFirst") || collision.CompareTag("DeathZoneForSecond"))
        {
            Debug.Log("Player fell into Death Zone!");
            StartCoroutine(StartFallSequence());
        }

        // --- Level 1 ---
        if (collision.CompareTag("DropTrigger"))
            TriggerDrop("DropFloor");

        // --- Level 2 ---
        if (collision.CompareTag("DropTriggerForFirst"))
            TriggerDrop("DropFloorFirst");

        if (collision.CompareTag("DropTriggerForSecond"))
            TriggerDrop("DropFloorSecond");

        // --- Level 3 & 4 (Shared platform shrink logic) ---
        if (collision.CompareTag("DropTriggerForThird") || collision.CompareTag("DropTriggerForThird2"))
            TriggerShrink("SafeArea_Right");

        if (collision.CompareTag("DropTriggerForFourth"))
            TriggerDrop("DropFloorSecond");

        // --- Level 5 Ceiling drops ---
        if (collision.CompareTag("DropTriggerForFirst"))
            TriggerCeilingDrop("DropCeilingFirst");

        if (collision.CompareTag("DropTriggerForSecond"))
            TriggerCeilingDrop("DropCeilingSecond");

        if (collision.CompareTag("DropTriggerForThird"))
            TriggerCeilingDrop("DropCeilingThird");

        if (collision.CompareTag("DropTriggerForFourth"))
            TriggerCeilingDrop("DropCeilingFourth");
    }

    // Generic trigger drop logic
    void TriggerDrop(string platformName)
    {
        GameObject dropPlatform = GameObject.Find(platformName);
        if (dropPlatform != null)
        {
            var script = dropPlatform.GetComponent<DroppingPlatform>();
            script?.TriggerDrop();
        }
        else Debug.LogWarning($"Drop platform '{platformName}' not found.");
    }

    // Generic trigger shrink logic
    void TriggerShrink(string platformName)
    {
        GameObject platform = GameObject.Find(platformName);
        if (platform != null)
        {
            var shrinkScript = platform.GetComponent<ShrinkingPlatform>();
            shrinkScript?.TriggerShrink();
        }
        else Debug.LogWarning($"Shrinking platform '{platformName}' not found.");
    }

    // Generic ceiling drop logic
    void TriggerCeilingDrop(string ceilingName)
    {
        GameObject dropCeil = GameObject.Find(ceilingName);
        if (dropCeil != null)
        {
            var ceilingScript = dropCeil.GetComponent<DroppingCeiling>();
            Debug.Log($"TriggerDropCeil() called for {ceilingName}!");
            ceilingScript?.TriggerDropCeil();
        }
        else Debug.LogWarning($"Ceiling '{ceilingName}' not found.");
    }

    // Triggers fall animation, disables controls, restarts scene
    public IEnumerator StartFallSequence()
    {
        isFalling = true;
        rb.velocity = new Vector2(0, -0.5f);
        rb.gravityScale = 0.7f;
        playerCollider.enabled = false;

        // Animate hands slightly up
        leftHand.localPosition += new Vector3(0, 0.2f, 0);
        rightHand.localPosition += new Vector3(0, 0.2f, 0);

        float fallDuration = 1.5f;
        float elapsed = 0;
        while (elapsed < fallDuration)
        {
            float swingAngle = Mathf.Sin(Time.time * 15) * 40;
            leftLeg.rotation = Quaternion.Euler(0, 0, swingAngle);
            rightLeg.rotation = Quaternion.Euler(0, 0, -swingAngle);
            elapsed += Time.deltaTime;
            yield return null;
        }

        Debug.Log("Restarting game...");
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Optional: Death animation when crushed
    IEnumerator BreakPlayerApart()
    {
        Debug.Log("💀 Player crushed! Breaking apart...");
        isFalling = true;
        rb.velocity = Vector2.zero;
        rb.gravityScale = 0;
        playerCollider.enabled = false;

        // Detach body parts and scatter them
        ScatterPart(leftLeg);
        ScatterPart(rightLeg);
        ScatterPart(leftHand);
        ScatterPart(rightHand);
        ScatterPart(face);
        ScatterPart(body);

        yield return new WaitForSeconds(1.5f);
        Debug.Log("🔄 Restarting game...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void ScatterPart(Transform part)
    {
        if (part == null) return;

        part.parent = null;
        var partRb = part.gameObject.AddComponent<Rigidbody2D>();
        partRb.gravityScale = 1;
        partRb.AddForce(new Vector2(Random.Range(-3f, 3f), Random.Range(3f, 6f)), ForceMode2D.Impulse);

        Destroy(part.gameObject, 1.5f); // Cleanup after animation
    }

    // Disables movement and physics on the player
    public void DisableControls()
    {
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
        enabled = false;
    }
}
