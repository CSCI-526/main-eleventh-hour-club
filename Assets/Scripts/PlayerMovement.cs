using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float speed = 3f;
    public float jumpForce = 6f;
    private Rigidbody2D rb;
    private Collider2D playerCollider;
    private bool isGrounded;
    private bool isFalling = false;

    private Transform leftLeg;
    private Transform rightLeg;
    private Transform leftHand;
    private Transform rightHand;
    private Transform face;
    private Transform body;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();

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
        rb.linearVelocity = new Vector2(move * speed, rb.linearVelocity.y);

        // Jump if on Ground
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetAxisRaw("Vertical") > 0) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
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
        string currentScene = SceneManager.GetActiveScene().name;
        //Level 1
        if (collision.gameObject.CompareTag("DropTrigger"))
        {
            GameObject dropPlatform = GameObject.Find("DropFloor");

            if (dropPlatform != null)
            {
                DroppingPlatform droppingPlatform = dropPlatform.GetComponent<DroppingPlatform>();
                droppingPlatform?.TriggerDrop();
            }
        }

        if (collision.gameObject.CompareTag("DeathZone"))
        {
            Debug.Log("Player fell into Death Zone!");
            StartCoroutine(StartFallSequence());
        }

        //Level 2
        if (collision.gameObject.CompareTag("DropTriggerForFirst"))
        {
            GameObject dropPlatform = GameObject.Find("DropFloorFirst");

            if (dropPlatform != null)
            {
                DroppingPlatform droppingPlatform = dropPlatform.GetComponent<DroppingPlatform>();
                droppingPlatform?.TriggerDrop();
            }
        }

        if (collision.gameObject.CompareTag("DeathZoneForFirst"))
        {
            Debug.Log("Player fell into Death Zone!");
            StartCoroutine(StartFallSequence());
        }

        if (collision.gameObject.CompareTag("DropTriggerForSecond"))
        {
            GameObject dropPlatform = GameObject.Find("DropFloorSecond");

            if (dropPlatform != null)
            {
                DroppingPlatform droppingPlatform = dropPlatform.GetComponent<DroppingPlatform>();
                droppingPlatform?.TriggerDrop();
            }
        }

        if (collision.gameObject.CompareTag("DeathZoneForSecond"))
        {
            Debug.Log("Player fell into Death Zone!");
            StartCoroutine(StartFallSequence());
        }

        //Level 3

        if (collision.CompareTag("DropTriggerForThird"))
        {
            // Find SafeArea_Right by name or use a public reference.
            GameObject rightFloor = GameObject.Find("SafeArea_Right");
            if (rightFloor != null)
            {
                ShrinkingPlatform shrinkScript = rightFloor.GetComponent<ShrinkingPlatform>();
                if (shrinkScript != null)
                {
                    shrinkScript.TriggerShrink();
                }
            }
        }

        //Level 4

        if (collision.CompareTag("DropTriggerForThird2"))
        {
            // Find SafeArea_Right by name or use a public reference.
            GameObject rightFloor = GameObject.Find("SafeArea_Right");
            if (rightFloor != null)
            {
                ShrinkingPlatform shrinkScript = rightFloor.GetComponent<ShrinkingPlatform>();
                if (shrinkScript != null)
                {
                    shrinkScript.TriggerShrink();
                }
            }

        }

        if (collision.gameObject.CompareTag("DropTriggerForFourth"))
        {
            GameObject dropPlatform = GameObject.Find("DropFloorSecond");

            if (dropPlatform != null)
            {
                DroppingPlatform droppingPlatform = dropPlatform.GetComponent<DroppingPlatform>();
                droppingPlatform?.TriggerDrop();
            }
        }

        // Level 5

        // Check if the player hits the trigger for DropCeilingFirst
        if (collision.gameObject.CompareTag("DropTriggerForFirst")) {
            GameObject dropCeil = GameObject.Find("DropCeilingFirst");

            if (dropCeil != null) {
                DroppingCeiling droppingCeiling = dropCeil.GetComponent<DroppingCeiling>();
                Debug.Log("TriggerDropCeil() called for DropCeilingFirst!");
                droppingCeiling?.TriggerDropCeil();
            }
        }

        // Check if the player hits the trigger for DropCeilingSecond
        if (collision.gameObject.CompareTag("DropTriggerForSecond")) {
            GameObject dropCeil = GameObject.Find("DropCeilingSecond");

            if (dropCeil != null) {
                DroppingCeiling droppingCeiling = dropCeil.GetComponent<DroppingCeiling>();
                Debug.Log("TriggerDropCeil() called for DropCeilingSecond!");
                droppingCeiling?.TriggerDropCeil();
            }
        }

        // Check if the player hits the trigger for DropCeilingThird
        if (collision.gameObject.CompareTag("DropTriggerForThird")) {
            GameObject dropCeil = GameObject.Find("DropCeilingThird");

            if (dropCeil != null) {
                DroppingCeiling droppingCeiling = dropCeil.GetComponent<DroppingCeiling>();
                Debug.Log("TriggerDropCeil() called for DropCeilingThird!");
                droppingCeiling?.TriggerDropCeil();
            }
        }
    }

    public IEnumerator StartFallSequence()
    {
        isFalling = true;
        rb.linearVelocity = new Vector2(0, -0.5f);

        playerCollider.enabled = false;

        rb.gravityScale = 0.7f;

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

    IEnumerator BreakPlayerApart()
    {
        Debug.Log("💀 Player crushed! Breaking apart...");

        isFalling = true; // Disable movement

        // Disable player collision so the broken parts move freely
        playerCollider.enabled = false;
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0; // Disable gravity on main body

        // Detach and scatter limbs
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

    // Function to detach and scatter parts
    void ScatterPart(Transform bodyPart)
    {
        if (bodyPart == null) return;

        bodyPart.parent = null; // Detach from player
        Rigidbody2D partRb = bodyPart.gameObject.AddComponent<Rigidbody2D>();
        partRb.gravityScale = 1;
        partRb.AddForce(new Vector2(Random.Range(-3f, 3f), Random.Range(3f, 6f)), ForceMode2D.Impulse);
        
        Destroy(bodyPart.gameObject, 1.5f); // Destroy parts after delay
    }

    public void DisableControls()
    {
        rb.linearVelocity = Vector2.zero;
        rb.isKinematic = true;
        enabled = false;
    }
}
