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
    private bool isFalling = false; // Used for both falling and crushing states

    private Transform leftLeg;
    private Transform rightLeg;
    private Transform leftHand;
    private Transform rightHand;
    private Transform face;
    private Transform body;

    // Analytics Variables
    private SendToGoogle _googleFormSender;
    private int currentLevel = 1; // Track current level, starts at 1
    // Removed levelCompletedCount - uses PlayerPrefs managed by Door.cs / read directly for death

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();

        // Find body parts - Consider using public variables assigned in Inspector for robustness
        leftLeg = transform.Find("LeftLeg/LeftLegSprite");
        rightLeg = transform.Find("RightLeg/RightLegSprite");
        leftHand = transform.Find("Hands/LeftHand");
        rightHand = transform.Find("Hands/RightHand");
        face = transform.Find("Face");
        body = transform.Find("Body"); // Ensure this exists if used in BreakPlayerApart

        // Analytics: Find SendToGoogle script
        _googleFormSender = FindObjectOfType<SendToGoogle>();
        if (_googleFormSender == null)
        {
            Debug.LogError("SendToGoogle script not found in the scene!");
        }

        // Determine current level based on the scene name
        string currentSceneName = SceneManager.GetActiveScene().name;
        if (currentSceneName.StartsWith("Level"))
        {
            // Try parsing number first
            if (int.TryParse(currentSceneName.Substring(5), out int levelNumber))
            {
                currentLevel = levelNumber;
            }
            // Fallback to specific names if parsing fails or for non-numeric levels
            else if (currentSceneName == "Level1_AvoidTheVoid") { currentLevel = 1; }
            else if (currentSceneName == "Level2_AvoidTheVoid") { currentLevel = 2; }
            else if (currentSceneName == "Level3_AvoidTheVoid") { currentLevel = 3; }
            else if (currentSceneName == "Level4_AvoidTheVoid") { currentLevel = 4; }
            else if (currentSceneName == "Level5_AvoidTheVoid") { currentLevel = 5; }
            else
            {
                Debug.LogError("Unknown level name format: " + currentSceneName + ". Defaulting to Level 1.");
                currentLevel = 1;
            }
        }
        else
        {
             Debug.LogWarning("Scene name '" + currentSceneName + "' does not start with 'Level'. Defaulting currentLevel to 1.");
             currentLevel = 1;
        }
        Debug.Log("Current Level set to: " + currentLevel);
    }

    void Update()
    {
        // Disable controls if falling/crushed or script is disabled
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
         // Check if references are valid before animating
        if (leftLeg == null || rightLeg == null) return;

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

        // Check for crushing collision ONLY in Level 5
        if (currentLevel == 5 && collision.gameObject.CompareTag("DroppingCeiling"))
        {
             // Check if the collision is mostly from above to confirm crushing
            ContactPoint2D contact = collision.GetContact(0);
            // Vector2.Dot < -0.7 means the collision normal is pointing significantly downwards
            if (Vector2.Dot(contact.normal, Vector2.up) < -0.7f)
            {
                 Debug.Log("Player crushed by DroppingCeiling.");
                 StartCoroutine(BreakPlayerApart());
            }
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
        // --- Level Specific Triggers ---
        // Using currentLevel check is crucial if tags are reused across levels

        // Level 1 DropTrigger
        if (currentLevel == 1 && collision.gameObject.CompareTag("DropTrigger"))
        {
            GameObject dropPlatform = GameObject.Find("DropFloor"); // Consider caching or using Inspector refs
            dropPlatform?.GetComponent<DroppingPlatform>()?.TriggerDrop();
        }

        // Level 2 Triggers
        if (currentLevel == 2)
        {
            if (collision.gameObject.CompareTag("DropTriggerForFirst"))
            {
                GameObject dropPlatform = GameObject.Find("DropFloorFirst");
                dropPlatform?.GetComponent<DroppingPlatform>()?.TriggerDrop();
            }
            if (collision.gameObject.CompareTag("DropTriggerForSecond"))
            {
                 GameObject dropPlatform = GameObject.Find("DropFloorSecond");
                 dropPlatform?.GetComponent<DroppingPlatform>()?.TriggerDrop();
            }
        }

        // Level 3 Trigger
        if (currentLevel == 3 && collision.CompareTag("DropTriggerForThird"))
        {
            GameObject rightFloor = GameObject.Find("SafeArea_Right");
            rightFloor?.GetComponent<ShrinkingPlatform>()?.TriggerShrink();
        }

        // Level 4 Triggers
        if (currentLevel == 4)
        {
            if (collision.CompareTag("DropTriggerForThird2")) // Unique tag?
            {
                GameObject rightFloor = GameObject.Find("SafeArea_Right"); // Name reused? Be careful
                rightFloor?.GetComponent<ShrinkingPlatform>()?.TriggerShrink();
            }
            if (collision.gameObject.CompareTag("DropTriggerForFourth"))
            {
                 GameObject dropPlatform = GameObject.Find("DropFloorSecond"); // Name reused? Be careful
                 dropPlatform?.GetComponent<DroppingPlatform>()?.TriggerDrop();
            }
        }

         // Level 5 Triggers (Dropping Ceilings)
        if (currentLevel == 5)
        {
             if (collision.gameObject.CompareTag("DropTriggerForFirst"))
             {
                GameObject dropCeil = GameObject.Find("DropCeilingFirst");
                dropCeil?.GetComponent<DroppingCeiling>()?.TriggerDropCeil();
             }
             if (collision.gameObject.CompareTag("DropTriggerForSecond"))
             {
                GameObject dropCeil = GameObject.Find("DropCeilingSecond");
                dropCeil?.GetComponent<DroppingCeiling>()?.TriggerDropCeil();
             }
              if (collision.CompareTag("DropTriggerForThird"))
             {
                GameObject dropCeil = GameObject.Find("DropCeilingThird");
                dropCeil?.GetComponent<DroppingCeiling>()?.TriggerDropCeil();
             }
             if (collision.gameObject.CompareTag("DropTriggerForFourth"))
             {
                 GameObject dropCeil = GameObject.Find("DropCeilingFourth");
                 dropCeil?.GetComponent<DroppingCeiling>()?.TriggerDropCeil();
             }
        }

        // --- General Triggers ---

        // *** DOOR TRIGGER LOGIC REMOVED FROM HERE - Handled by Door.cs ***

    } // End of OnTriggerEnter2D


    // Called by DeathZone.cs when player enters a death trigger zone
    public IEnumerator StartFallSequence()
    {
        if (isFalling) yield break; // Prevent multiple sequences

        Debug.Log("Starting fall sequence...");
        isFalling = true; // Set state
        DisableControls(); // Stop player input and make kinematic initially

        // Prepare for physics-based fall
        rb.isKinematic = false;
        rb.gravityScale = 0.7f;
        rb.linearVelocity = new Vector2(0, -0.5f); // Initial downward push

        if (playerCollider != null) playerCollider.enabled = false;

        // Optional visual feedback
         if (leftHand != null) leftHand.localPosition += new Vector3(0, 0.2f, 0);
         if (rightHand != null) rightHand.localPosition += new Vector3(0, 0.2f, 0);

        // Analytics for falling death should be sent by DeathZone.cs *before* calling this

        float fallDuration = 1.5f;
        float elapsed = 0;
        while (elapsed < fallDuration)
        {
             // Animate legs if they haven't been destroyed (e.g., by BreakPlayerApart)
             if (leftLeg != null && rightLeg != null)
             {
                float swingAngle = Mathf.Sin(Time.time * 15) * 40;
                leftLeg.rotation = Quaternion.Euler(0, 0, swingAngle);
                rightLeg.rotation = Quaternion.Euler(0, 0, -swingAngle);
             }
            elapsed += Time.deltaTime;
            yield return null;
        }

        Debug.Log("Restarting level after fall...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reload current scene
    }


    // Called by OnCollisionEnter2D in Level 5 when crushed
    IEnumerator BreakPlayerApart()
    {
         if (isFalling) yield break; // Prevent starting if already falling/broken

        Debug.Log("💀 Player crushed! Breaking apart...");
        isFalling = true; // Set state
        DisableControls(); // Stop input

        // Disable main physics interactions
        if (playerCollider != null) playerCollider.enabled = false;
        if (rb != null) rb.simulated = false; // Stop main rigidbody simulation

        // Analytics for crushing death - Fetch completed count from PlayerPrefs
        if (_googleFormSender != null)
        {
             int completedCountFromPrefs = PlayerPrefs.GetInt("LevelCompletedCount", 0);
             // Send DeathTrigger = 1 (crushed), DoorReached = 0, levelCompleted = count from Prefs
             _googleFormSender.Send(currentLevel, 1, 0, completedCountFromPrefs);
             // Note: SendToGoogle might internally modify the levelCompleted value based on currentLevel if you kept that last change.
             Debug.Log($"Crushing Death Data Sent - Level: {currentLevel}, Completed Count (from Prefs): {completedCountFromPrefs}");
        }

        // Detach and scatter limbs
        ScatterPart(leftLeg);
        ScatterPart(rightLeg);
        ScatterPart(leftHand);
        ScatterPart(rightHand);
        ScatterPart(face);
        ScatterPart(body); // Ensure 'body' transform is assigned if needed

        // Optionally hide any remaining main player sprites if ScatterPart doesn't handle everything
        // foreach (var renderer in GetComponentsInChildren<SpriteRenderer>()) { renderer.enabled = false; }


        yield return new WaitForSeconds(2.0f); // Wait for parts to scatter

        Debug.Log("🔄 Restarting level after being crushed...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void ScatterPart(Transform bodyPart)
    {
        if (bodyPart == null) return;

        // Ensure necessary components exist for physics/visibility
        SpriteRenderer sr = bodyPart.GetComponent<SpriteRenderer>();
        if (sr == null) sr = bodyPart.gameObject.AddComponent<SpriteRenderer>(); // Basic visibility
         // Add/get collider - BoxCollider2D is a common default
        Collider2D partCollider = bodyPart.GetComponent<Collider2D>();
        if (partCollider == null) partCollider = bodyPart.gameObject.AddComponent<BoxCollider2D>();
        partCollider.enabled = true; // Ensure it can collide
        partCollider.isTrigger = false; // Make sure it's a solid collider for physics scatter

         // Add/get Rigidbody2D
        Rigidbody2D partRb = bodyPart.GetComponent<Rigidbody2D>();
         if (partRb == null) partRb = bodyPart.gameObject.AddComponent<Rigidbody2D>();

        bodyPart.parent = null; // Detach from main player object

        // Configure Rigidbody for scattering
        partRb.gravityScale = 1;
        partRb.isKinematic = false;
        partRb.simulated = true;
        partRb.AddForce(new Vector2(Random.Range(-2f, 2f), Random.Range(2f, 5f)), ForceMode2D.Impulse);
        partRb.AddTorque(Random.Range(-90f, 90f));

        // Clean up scattered parts after a delay
        Destroy(bodyPart.gameObject, 2.0f);
    }

    // --- Helper Methods ---

    // Call this to get the current level number determined in Start()
    public int GetCurrentLevel()
    {
        return currentLevel;
    }

    // Call this to make the player uncontrollable (e.g., during cutscenes, death, door sequence)
    public void DisableControls()
    {
        enabled = false; // Stops Update() loop in this script
         if (rb != null)
         {
             rb.linearVelocity = Vector2.zero; // Stop movement
             rb.isKinematic = true; // Disable physics influence (gravity, forces)
         }
    }

     // Maybe add an EnableControls() if needed later
     // public void EnableControls() { enabled = true; if (rb != null) rb.isKinematic = false; }

    // Reset game progress (called from a menu maybe?) - Resets PlayerPrefs count too
    public void ResetGameProgress()
    {
        PlayerPrefs.SetInt("LevelCompletedCount", 0);
        PlayerPrefs.Save(); // Good practice to save immediately after setting
        Debug.Log("Game progress reset (LevelCompletedCount set to 0 in PlayerPrefs)");
        // Optionally reload the first level or main menu
        // SceneManager.LoadScene("Level1_AvoidTheVoid");
    }
}