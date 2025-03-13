// using UnityEngine;
// using System.Collections;

// public class ShrinkingPlatformBothSides : MonoBehaviour
// {
//     [Header("Initial Edges (World Space)")]
//     public float initialLeftX = 0f;   // The platform's initial left boundary in world space
//     public float initialRightX = 10f; // The platform's initial right boundary in world space

//     [Header("Left Shrink Settings")]
//     public float leftShrinkAmount = 3f;      // How many units the left edge moves right
//     public float leftShrinkDuration = 2.5f;  // How long it takes for the left side to finish moving

//     [Header("Right Shrink Settings")]
//     public float rightShrinkAmount = 2f;     // How many units the right edge moves left
//     public float rightShrinkDuration = 5f;   // How long it takes for the right side to finish moving

//     // Optional BoxCollider2D if you want collisions to match the new size
//     public BoxCollider2D platformCollider;

//     private float currentLeftX;
//     private float currentRightX;

//     private float finalLeftX;
//     private float finalRightX;

//     private bool isShrinking = false;

//     void Start()
//     {
//         // Initialize edges
//         currentLeftX = initialLeftX;
//         currentRightX = initialRightX;

//         // Compute final positions for each edge
//         finalLeftX = initialLeftX + leftShrinkAmount;
//         finalRightX = initialRightX - rightShrinkAmount;

//         // Set initial scale & position
//         UpdatePlatform();
//     }

//     public void TriggerShrink()
//     {
//         if (!isShrinking)
//         {
//             StartCoroutine(ShrinkRoutine());
//         }
//     }

//     IEnumerator ShrinkRoutine()
//     {
//         isShrinking = true;

//         float leftElapsed = 0f;
//         float rightElapsed = 0f;

//         // We'll run until both edges finish their movement.
//         while (leftElapsed < leftShrinkDuration || rightElapsed < rightShrinkDuration)
//         {
//             // Advance each timer
//             if (leftElapsed < leftShrinkDuration)
//                 leftElapsed += Time.deltaTime;
//             if (rightElapsed < rightShrinkDuration)
//                 rightElapsed += Time.deltaTime;

//             // Lerp each edge independently
//             currentLeftX = Mathf.Lerp(initialLeftX, finalLeftX, Mathf.Clamp01(leftElapsed / leftShrinkDuration));
//             currentRightX = Mathf.Lerp(initialRightX, finalRightX, Mathf.Clamp01(rightElapsed / rightShrinkDuration));

//             // Update the transform & collider
//             UpdatePlatform();

//             yield return null;
//         }

//         // Ensure final positions
//         currentLeftX = finalLeftX;
//         currentRightX = finalRightX;
//         UpdatePlatform();

//         isShrinking = false;
//     }

//     void UpdatePlatform()
//     {
//         // Calculate current width
//         float currentWidth = currentRightX - currentLeftX;
//         // Calculate center
//         float centerX = (currentLeftX + currentRightX) * 0.5f;

//         // Update transform scale & position
//         Vector3 pos = transform.position;
//         pos.x = centerX;
//         transform.position = pos;

//         Vector3 scale = transform.localScale;
//         scale.x = currentWidth; // Assuming the sprite is 1 unit wide by default
//         transform.localScale = scale;

//         // Update BoxCollider2D if assigned
//         if (platformCollider != null)
//         {
//             Vector2 size = platformCollider.size;
//             size.x = currentWidth;
//             platformCollider.size = size;

//             Vector2 offset = platformCollider.offset;
//             offset.x = currentWidth / 2f; 
//             platformCollider.offset = offset;
//         }
//     }
// }

using UnityEngine;
using System.Collections;

public class ShrinkingPlatformBothSides : MonoBehaviour
{
    public float initialLeftX = 0f;
    public float initialRightX = 10f;

    public float leftShrinkAmount = 3f;
    public float leftShrinkDuration = 2.5f;

    public float rightShrinkAmount = 2f;
    public float rightShrinkDuration = 5f;

    public BoxCollider2D platformCollider;

    private float currentLeftX;
    private float currentRightX;
    private float finalLeftX;
    private float finalRightX;

    private bool isShrinking = false;

    void Start()
    {
        currentLeftX = initialLeftX;
        currentRightX = initialRightX;

        finalLeftX = initialLeftX + leftShrinkAmount;
        finalRightX = initialRightX - rightShrinkAmount;

        UpdatePlatform();
    }

    public void TriggerShrink()
    {
        if (!isShrinking)
        {
            StartCoroutine(ShrinkRoutine());
        }
    }

    IEnumerator ShrinkRoutine()
    {
        isShrinking = true;
        float leftElapsed = 0f;
        float rightElapsed = 0f;

        while (leftElapsed < leftShrinkDuration || rightElapsed < rightShrinkDuration)
        {
            if (leftElapsed < leftShrinkDuration)
                leftElapsed += Time.deltaTime;
            if (rightElapsed < rightShrinkDuration)
                rightElapsed += Time.deltaTime;

            currentLeftX = Mathf.Lerp(initialLeftX, finalLeftX, Mathf.Clamp01(leftElapsed / leftShrinkDuration));
            currentRightX = Mathf.Lerp(initialRightX, finalRightX, Mathf.Clamp01(rightElapsed / rightShrinkDuration));

            UpdatePlatform();
            yield return null;
        }

        currentLeftX = finalLeftX;
        currentRightX = finalRightX;
        UpdatePlatform();

        isShrinking = false;
    }

    void UpdatePlatform()
    {
        float currentWidth = currentRightX - currentLeftX;
        float centerX = (currentLeftX + currentRightX) * 0.5f;

        Vector3 pos = transform.position;
        pos.x = centerX;
        transform.position = pos;

        Vector3 scale = transform.localScale;
        scale.x = currentWidth;
        transform.localScale = scale;
    }

    // ✅ **New: Reset the platform after the player falls**
    public void ResetPlatform()
    {
        StopAllCoroutines();
        isShrinking = false;

        currentLeftX = initialLeftX;
        currentRightX = initialRightX;
        UpdatePlatform();

        Debug.Log($"{gameObject.name} reset to original size!");
    }
}

