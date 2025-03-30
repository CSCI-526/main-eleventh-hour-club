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

    // Analytics Variables
    private SendToGoogle _googleFormSender;
    private int currentLevel = 1; // Track current level, starts at 1
    private int levelCompletedCount = 0; // Track number of levels completed
    private int currentLife = 3; // Player starts with 3 lives (you can decide if you still need this for gameplay)

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();

        leftLeg = transform.Find("LeftLeg/LeftLegSprite");
        rightLeg = transform.Find("RightLeg/RightLegSprite");
        leftHand = transform.Find("Hands/LeftHand");
        rightHand = transform.Find("Hands/RightHand");
        face = transform.Find("Face");

        // Analytics: Find SendToGoogle script
        _googleFormSender = FindObjectOfType<SendToGoogle>();
        if (_googleFormSender == null)
        {
            Debug.LogError("SendToGoogle script not found in the scene! Make sure you have it attached to a GameObject.");
        }

        // Determine current level based on the scene name
        string currentSceneName = SceneManager.GetActiveScene().name;
        if (currentSceneName.StartsWith("Level"))
        {
            if (int.TryParse(currentSceneName.Substring(5), out int levelNumber))
            {
                currentLevel = levelNumber;
            }
            else if (currentSceneName == "Level1_AvoidTheVoid")
            {
                currentLevel = 1;
            }
            else if (currentSceneName == "Level2_AvoidTheVoid")
            {
                currentLevel = 2;
            }
            else if (currentSceneName == "Level3_AvoidTheVoid")
            {
                currentLevel = 3;
            }
            else if (currentSceneName == "Level4_AvoidTheVoid")
            {
                currentLevel = 4;
            }
            // Add more conditions for other level names if needed
        }
        Debug.Log("Current Level set to: " + currentLevel);

        // Initialize levelCompletedCount here
        levelCompletedCount = 0;
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
        //Level 1 Death Zone (removed specific death zones, using generic "DeathZone" tag now in DeathZone.cs)


        //Level 2 Drop Triggers
        if (collision.gameObject.CompareTag("DropTriggerForFirst"))
        {
            GameObject dropPlatform = GameObject.Find("DropFloorFirst");
            if (dropPlatform != null)
            {
                DroppingPlatform droppingPlatform = dropPlatform.GetComponent<DroppingPlatform>();
                droppingPlatform?.TriggerDrop();
            }
        }
        //Level 2 Death Zone (removed specific death zones, using generic "DeathZone" tag now in DeathZone.cs)


        if (collision.gameObject.CompareTag("DropTriggerForSecond"))
        {
            GameObject dropPlatform = GameObject.Find("DropFloorSecond");
            if (dropPlatform != null)
            {
                DroppingPlatform droppingPlatform = dropPlatform.GetComponent<DroppingPlatform>();
                droppingPlatform?.TriggerDrop();
            }
        }
        //Level 2 Death Zone (removed specific death zones, using generic "DeathZone" tag now in DeathZone.cs)

        //Level 3 Drop Triggers
        if (collision.CompareTag("DropTriggerForThird"))
        {
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

        //Level 4 Drop Triggers
        if (collision.CompareTag("DropTriggerForThird2"))
        {
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

        // Level Completion Trigger (assuming a tag "Door" for the door object)
        if (collision.CompareTag("Door"))
        {
            Debug.Log("Door Reached! Level Completed.");
            DisableControls();
            if (_googleFormSender != null)
            {
                _googleFormSender.Send(currentLevel, 0, 1, levelCompletedCount); // DeathTrigger = 0, DoorReached = 1
                Debug.Log("Level Completion Data Sent to Google Forms");
            }
            // You might want to load the next level here
            // SceneManager.LoadScene("NextLevelSceneName");
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

        // Check if the player hits the trigger for DropCeilingFourth
        if (collision.gameObject.CompareTag("DropTriggerForFourth")) {
            GameObject dropCeil = GameObject.Find("DropCeilingFourth");

            if (dropCeil != null) {
                DroppingCeiling droppingCeiling = dropCeil.GetComponent<DroppingCeiling>();
                Debug.Log("TriggerDropCeil() called for DropCeilingFourth!");
                droppingCeiling?.TriggerDropCeil();
            }
        }
    }

    public IEnumerator StartFallSequence()
    {
        Debug.Log("Starting fall sequence...");
        isFalling = true;
        rb.linearVelocity = new Vector2(0, -0.5f);

        playerCollider.enabled = false;
        rb.gravityScale = 0.7f;
        leftHand.localPosition += new Vector3(0, 0.2f, 0);
        rightHand.localPosition += new Vector3(0, 0.2f, 0);

        if (_googleFormSender != null) // Send death data
        {
            _googleFormSender.Send(currentLevel, 1, 0, levelCompletedCount); // DeathTrigger = 1, DoorReached = 0
            Debug.Log("Death Data Sent to Google Forms");
        }
        else
        {
            Debug.LogWarning("SendToGoogle component NOT FOUND! Analytics will NOT be sent.");
        }

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
        ResetLevel();
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