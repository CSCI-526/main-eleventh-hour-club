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

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();

        leftLeg = transform.Find("LeftLeg/LeftLegSprite");
        rightLeg = transform.Find("RightLeg/RightLegSprite");
        leftHand = transform.Find("Hands/LeftHand");
        rightHand = transform.Find("Hands/RightHand");
        face = transform.Find("Face");
    }

    void Update()
    {
        if (!enabled || isFalling) return;
        
        float move = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(move * speed, rb.linearVelocity.y);

        // Jump if on Ground
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow)) && isGrounded)
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

    public void DisableControls()
    {
        rb.linearVelocity = Vector2.zero;
        rb.isKinematic = true;
        enabled = false;
    }
}
