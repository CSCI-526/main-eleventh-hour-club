// using UnityEngine;
// using System.Collections;
// using UnityEngine.SceneManagement;

// public class PlayerController : MonoBehaviour
// {
//     public float speed = 3f;
//     public float jumpForce = 6f;
//     private Rigidbody2D rb;
//     private Collider2D playerCollider;
//     private bool isGrounded;
//     private bool isFalling = false;

//     private Transform leftLeg;
//     private Transform rightLeg;
//     private Transform leftHand;
//     private Transform rightHand;
//     private Transform face;

//     void Start()
//     {
//         rb = GetComponent<Rigidbody2D>();
//         playerCollider = GetComponent<Collider2D>();

//         leftLeg = transform.Find("LeftLeg/LeftLegSprite");
//         rightLeg = transform.Find("RightLeg/RightLegSprite");
//         leftHand = transform.Find("Hands/LeftHand");
//         rightHand = transform.Find("Hands/RightHand");
//         face = transform.Find("Face");
//     }

//     void Update()
//     {
//         if (!enabled || isFalling) return;
        
//         float move = Input.GetAxis("Horizontal");
//         rb.linearVelocity = new Vector2(move * speed, rb.linearVelocity.y);

//         // Jump if on Ground
//         if ((Input.GetKeyDown(KeyCode.Space) || Input.GetAxisRaw("Vertical") > 0) && isGrounded)
//         {
//             rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
//             isGrounded = false;
//         }

//         AnimateLegs(move);
//     }

//     void AnimateLegs(float move)
//     {
//         if (Mathf.Abs(move) > 0.1f)
//         {
//             float legAngle = Mathf.Sin(Time.time * 10) * 20;
//             leftLeg.rotation = Quaternion.Euler(0, 0, legAngle);
//             rightLeg.rotation = Quaternion.Euler(0, 0, -legAngle);
//         }
//         else
//         {
//             leftLeg.rotation = Quaternion.identity;
//             rightLeg.rotation = Quaternion.identity;
//         }
//     }

//     void OnCollisionEnter2D(Collision2D collision)
//     {
//         if (collision.gameObject.CompareTag("Ground"))
//         {
//             isGrounded = true;
//         }
//     }

//     void OnCollisionExit2D(Collision2D collision)
//     {
//         if (collision.gameObject.CompareTag("Ground"))
//         {
//             isGrounded = false;
//         }
//     }

//     void OnTriggerEnter2D(Collider2D collision)
//     {
//         //Level 1
//         if (collision.gameObject.CompareTag("DropTrigger"))
//         {
//             GameObject dropPlatform = GameObject.Find("DropFloor");

//             if (dropPlatform != null)
//             {
//                 DroppingPlatform droppingPlatform = dropPlatform.GetComponent<DroppingPlatform>();
//                 droppingPlatform?.TriggerDrop();
//             }
//         }

//         if (collision.gameObject.CompareTag("DeathZone"))
//         {
//             Debug.Log("Player fell into Death Zone!");
//             StartCoroutine(StartFallSequence());
//         }

//         //Level 2
//         if (collision.gameObject.CompareTag("DropTriggerForFirst"))
//         {
//             GameObject dropPlatform = GameObject.Find("DropFloorFirst");

//             if (dropPlatform != null)
//             {
//                 DroppingPlatform droppingPlatform = dropPlatform.GetComponent<DroppingPlatform>();
//                 droppingPlatform?.TriggerDrop();
//             }
//         }

//         if (collision.gameObject.CompareTag("DeathZoneForFirst"))
//         {
//             Debug.Log("Player fell into Death Zone!");
//             StartCoroutine(StartFallSequence());
//         }

//         if (collision.gameObject.CompareTag("DropTriggerForSecond"))
//         {
//             GameObject dropPlatform = GameObject.Find("DropFloorSecond");

//             if (dropPlatform != null)
//             {
//                 DroppingPlatform droppingPlatform = dropPlatform.GetComponent<DroppingPlatform>();
//                 droppingPlatform?.TriggerDrop();
//             }
//         }

//         if (collision.gameObject.CompareTag("DeathZoneForSecond"))
//         {
//             Debug.Log("Player fell into Death Zone!");
//             StartCoroutine(StartFallSequence());
//         }


//         //Level 3

//         if (collision.CompareTag("DropTriggerForThird"))
//         {
//             // Find SafeArea_Right by name or use a public reference.
//             GameObject rightFloor = GameObject.Find("SafeArea_Right");
//             if (rightFloor != null)
//             {
//                 ShrinkingPlatform shrinkScript = rightFloor.GetComponent<ShrinkingPlatform>();
//                 if (shrinkScript != null)
//                 {
//                     shrinkScript.TriggerShrink();
//                 }
//             }
//         }

//         //Level 4

//         if (collision.CompareTag("DropTriggerForThird2"))
//         {
//             // Find SafeArea_Right by name or use a public reference.
//             GameObject rightFloor = GameObject.Find("SafeArea_Right");
//             if (rightFloor != null)
//             {
//                 ShrinkingPlatform shrinkScript = rightFloor.GetComponent<ShrinkingPlatform>();
//                 if (shrinkScript != null)
//                 {
//                     shrinkScript.TriggerShrink();
//                 }
//             }

//         }

//         if (collision.gameObject.CompareTag("DropTriggerForFourth"))
//         {
//             GameObject dropPlatform = GameObject.Find("DropFloorSecond");

//             if (dropPlatform != null)
//             {
//                 DroppingPlatform droppingPlatform = dropPlatform.GetComponent<DroppingPlatform>();
//                 droppingPlatform?.TriggerDrop();
//             }
//         }

//     }

//     public IEnumerator StartFallSequence()
//     {
//         isFalling = true;
//         rb.linearVelocity = new Vector2(0, -0.5f);

//         playerCollider.enabled = false;

//         rb.gravityScale = 0.7f;

//         leftHand.localPosition += new Vector3(0, 0.2f, 0);
//         rightHand.localPosition += new Vector3(0, 0.2f, 0);

//         float fallDuration = 1.5f;
//         float elapsed = 0;
//         while (elapsed < fallDuration)
//         {
//             float swingAngle = Mathf.Sin(Time.time * 15) * 40;
//             leftLeg.rotation = Quaternion.Euler(0, 0, swingAngle);
//             rightLeg.rotation = Quaternion.Euler(0, 0, -swingAngle);
//             elapsed += Time.deltaTime;
//             yield return null;
//         }

//         Debug.Log("Restarting game...");

//         yield return new WaitForSeconds(1.5f);
//         SceneManager.LoadScene(SceneManager.GetActiveScene().name);
//     }

//     public void DisableControls()
//     {
//         rb.linearVelocity = Vector2.zero;
//         rb.isKinematic = true;
//         enabled = false;
//     }
// }

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

    private Vector3 startPos;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();

        rb.gravityScale = 2.5f;

        leftLeg = transform.Find("LeftLeg/LeftLegSprite");
        rightLeg = transform.Find("RightLeg/RightLegSprite");
        leftHand = transform.Find("Hands/LeftHand");
        rightHand = transform.Find("Hands/RightHand");
        face = transform.Find("Face");

        startPos = this.transform.localPosition;
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

        //         //Level 2
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
        //Debug.Log("Restarting game...");

        yield return new WaitForSeconds(0.5f); // change this later to match actual wait time
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

       //ResetPlayer();
    }

    // private void ResetPlayer()
    // {
    //     leftHand.localPosition -= new Vector3(0, 0.2f, 0);
    //     rightHand.localPosition -= new Vector3(0, 0.2f, 0);
    //     enabled = true;
    //     isFalling = false;
    //     playerCollider.enabled = true;
    //     rb.gravityScale = 1f;
    //     this.transform.localPosition = startPos;
    // }

// private void ResetPlayer()
// {
//     leftHand.localPosition -= new Vector3(0, 0.2f, 0);
//     rightHand.localPosition -= new Vector3(0, 0.2f, 0);
//     enabled = true;
//     isFalling = false;
//     playerCollider.enabled = true;
//     rb.gravityScale = 1f;
//     this.transform.localPosition = startPos;

//     // Reset all dropping platforms
//     DroppingPlatform[] droppingPlatforms = Resources.FindObjectsOfTypeAll<DroppingPlatform>();

//     if (droppingPlatforms.Length == 0)
//     {
//         Debug.LogError("No DroppingPlatform objects found! Check if they are active and have the script attached.");
//     }

//     foreach (DroppingPlatform platform in droppingPlatforms)
//     {
//         Debug.Log($"Resetting platform: {platform.gameObject.name}");
//         platform.ResetPlatform();
//     }

//     Debug.Log("All platforms have been reset.");
// }

private void ResetPlayer()
{
    leftHand.localPosition -= new Vector3(0, 0.2f, 0);
    rightHand.localPosition -= new Vector3(0, 0.2f, 0);
    enabled = true;
    isFalling = false;
    isGrounded = true;
    playerCollider.enabled = true;
    jumpForce = 10f

    rb.bodyType = RigidbodyType2D.Kinematic; 
    rb.velocity = Vector2.zero; 
    rb.angularVelocity = 0f; 
    rb.gravityScale = 2.5f; 

    yield return new WaitForFixedUpdate();
    rb.bodyType = RigidbodyType2D.Dynamic; 
    yield return new WaitForSeconds(0.1f);
    
    this.transform.localPosition = startPos;

    // Reset all dropping platforms
    DroppingPlatform[] droppingPlatforms = Resources.FindObjectsOfTypeAll<DroppingPlatform>();
    foreach (DroppingPlatform platform in droppingPlatforms)
    {
        platform.ResetPlatform();
    }

    // ✅ **Reset All Shrinking Platforms**
    ShrinkingPlatform[] shrinkingPlatforms = Resources.FindObjectsOfTypeAll<ShrinkingPlatform>();
    foreach (ShrinkingPlatform platform in shrinkingPlatforms)
    {
        platform.ResetPlatform();
    }

    // ✅ **Reset All Shrinking Platforms (Both Sides)**
    ShrinkingPlatformBothSides[] shrinkingBothPlatforms = Resources.FindObjectsOfTypeAll<ShrinkingPlatformBothSides>();
    foreach (ShrinkingPlatformBothSides platform in shrinkingBothPlatforms)
    {
        platform.ResetPlatform();
    }

    Debug.Log("All platforms have been reset.");
}


    public void DisableControls()
    {
        rb.linearVelocity = Vector2.zero;
        rb.isKinematic = true;
        enabled = false;
    }
}
