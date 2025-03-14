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

    // Analytics Variables
    private SendToGoogle _googleFormSender;
    private int _currentLevel = 1; // Tracks the current level (starts at 1)
    private float _levelStartTime; // Records when the current level started

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();

        leftLeg = transform.Find("LeftLeg/LeftLegSprite");
        rightLeg = transform.Find("RightLeg/RightLegSprite");
        leftHand = transform.Find("Hands/LeftHand");
        rightHand = transform.Find("Hands/RightHand");
        face = transform.Find("Face");

        // Find the SendToGoogle script for analytics
        _googleFormSender = FindObjectOfType<SendToGoogle>();
        if (_googleFormSender == null)
        {
            Debug.LogError("SendToGoogle script not found in the scene! Make sure you have it attached to a GameObject.");
        }

        // Initialize level start time and send initial level data
        _levelStartTime = Time.time;
        SendLevelStartData();
    }

    // Sends level start data (with DeathTrigger = 0, DoorReached = 0)
    private void SendLevelStartData()
    {
        if (_googleFormSender != null)
        {
            _googleFormSender.Send(GetCurrentLevel(), 0, 0, GetTimeSpent());
            Debug.Log("Level Start Data Sent to Google Forms");
        }
    }

    void Update()
    {
        if (!enabled || isFalling)
            return;

        float move = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(move * speed, rb.velocity.y);

        // Jump if on ground
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
        // Level 1 Drop Trigger
        if (collision.gameObject.CompareTag("DropTrigger"))
        {
            GameObject dropPlatform = GameObject.Find("DropFloor");
            if (dropPlatform != null)
            {
                DroppingPlatform droppingPlatform = dropPlatform.GetComponent<DroppingPlatform>();
                droppingPlatform?.TriggerDrop();
            }
        }
        // Additional drop trigger logic for other levels...
        // (Keep any existing drop trigger logic as needed)
    }

    // Called when the player dies (falling into a death zone)
    public IEnumerator StartFallSequence()
    {
        Debug.Log("Starting fall sequence...");
        isFalling = true;
        rb.velocity = new Vector2(0, -0.5f);

        playerCollider.enabled = false;
        rb.gravityScale = 0.7f;
        leftHand.localPosition += new Vector3(0, 0.2f, 0);
        rightHand.localPosition += new Vector3(0, 0.2f, 0);

        if (_googleFormSender != null)
        {
            // Death event: DeathTrigger = 1, DoorReached = 0
            _googleFormSender.Send(GetCurrentLevel(), 1, 0, GetTimeSpent());
            Debug.Log("Death Data Sent to Google Forms");
        }
        else
        {
            Debug.LogWarning("SendToGoogle component NOT FOUND! Analytics will NOT be sent.");
        }

        float fallDuration = 1.5f;
        float elapsed = 0f;
        while (elapsed < fallDuration)
        {
            float swingAngle = Mathf.Sin(Time.time * 15) * 40;
            leftLeg.rotation = Quaternion.Euler(0, 0, swingAngle);
            rightLeg.rotation = Quaternion.Euler(0, 0, -swingAngle);
            elapsed += Time.deltaTime;
            yield return null;
        }

        Debug.Log("Restarting level...");
        yield return new WaitForSeconds(1.5f);
        ResetLevel();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Increment the current level (called on level completion)
    public void IncrementLevel()
    {
        _currentLevel++;
        _levelStartTime = Time.time; // Reset timer for new level
    }

    // Returns the current level number
    public int GetCurrentLevel()
    {
        return _currentLevel;
    }

    // Returns the time spent in the current level (in seconds)
    public float GetTimeSpent()
    {
        return Time.time - _levelStartTime;
    }

    // Resets the level data (used when restarting the level)
    public void ResetLevel()
    {
        _currentLevel = 1;
        _levelStartTime = Time.time;
        SendLevelStartData();
    }

    // Disables player controls (e.g., when transitioning or dying)
    public void DisableControls()
    {
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
        enabled = false;
    }
}