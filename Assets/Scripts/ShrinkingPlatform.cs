// using UnityEngine;
// using System.Collections;

// public class ShrinkingPlatform : MonoBehaviour
// {
//     [Header("Width Settings (in world units)")]
//     public float initialWidth = 10f;  // Starting width of SafeArea_Right
//     public float finalWidth = 2f;     // Final width after shrinking

//     [Header("Animation Settings")]
//     public float shrinkDuration = 2.0f; // Time over which the platform shrinks

//     // Right edge position (in world space) remains fixed.
//     private float rightEdgeX;

//     private bool isShrinking = false;

//     // Optionally, if you have a BoxCollider2D that should match the new size:
//     public BoxCollider2D platformCollider;

//     void Start()
//     {
//         // Set initial scale assuming the sprite's native size represents 1 unit.
//         transform.localScale = new Vector3(initialWidth, transform.localScale.y, transform.localScale.z);

//         // Calculate the right edge. With a center pivot, right edge = position.x + (width / 2).
//         rightEdgeX = transform.position.x + (initialWidth / 2f);

//         // Update collider if assigned.
//         if(platformCollider != null)
//         {
//             platformCollider.size = new Vector2(initialWidth, platformCollider.size.y);
//             platformCollider.offset = new Vector2(initialWidth / 2f, platformCollider.offset.y);
//         }
//     }

//     // Call this method to trigger the shrinking effect.
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
//         float elapsed = 0f;
//         float startWidth = initialWidth;

//         while (elapsed < shrinkDuration)
//         {
//             elapsed += Time.deltaTime;
//             // Interpolate current width from startWidth to finalWidth.
//             float newWidth = Mathf.Lerp(startWidth, finalWidth, elapsed / shrinkDuration);
//             // Update the platform's scale.
//             transform.localScale = new Vector3(newWidth, transform.localScale.y, transform.localScale.z);

//             // Recalculate the center so that the right edge stays fixed:
//             // newCenter = rightEdgeX - (newWidth / 2)
//             Vector3 newPos = transform.position;
//             newPos.x = rightEdgeX - (newWidth / 2f);
//             transform.position = newPos;

//             // Update the collider to match (if using a manual update).
//             if(platformCollider != null)
//             {
//                 platformCollider.size = new Vector2(newWidth, platformCollider.size.y);
//                 platformCollider.offset = new Vector2(newWidth / 2f, platformCollider.offset.y);
//             }

//             yield return null;
//         }

//         // Ensure final state.
//         transform.localScale = new Vector3(finalWidth, transform.localScale.y, transform.localScale.z);
//         Vector3 finalPos = transform.position;
//         finalPos.x = rightEdgeX - (finalWidth / 2f);
//         transform.position = finalPos;

//         if(platformCollider != null)
//         {
//             platformCollider.size = new Vector2(finalWidth, platformCollider.size.y);
//             platformCollider.offset = new Vector2(finalWidth / 2f, platformCollider.offset.y);
//         }
//         isShrinking = false;
//     }
// }

using UnityEngine;
using System.Collections;

public class ShrinkingPlatform : MonoBehaviour
{
    [Header("Width Settings (in world units)")]
    public float initialWidth = 10f;
    public float finalWidth = 2f;

    [Header("Animation Settings")]
    public float shrinkDuration = 2.0f;

    private float rightEdgeX;
    private bool isShrinking = false;

    public BoxCollider2D platformCollider;

    void Start()
    {
        transform.localScale = new Vector3(initialWidth, transform.localScale.y, transform.localScale.z);
        rightEdgeX = transform.position.x + (initialWidth / 2f);
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
        float elapsed = 0f;
        float startWidth = initialWidth;

        while (elapsed < shrinkDuration)
        {
            elapsed += Time.deltaTime;
            float newWidth = Mathf.Lerp(startWidth, finalWidth, elapsed / shrinkDuration);
            transform.localScale = new Vector3(newWidth, transform.localScale.y, transform.localScale.z);

            Vector3 newPos = transform.position;
            newPos.x = rightEdgeX - (newWidth / 2f);
            transform.position = newPos;

            yield return null;
        }

        transform.localScale = new Vector3(finalWidth, transform.localScale.y, transform.localScale.z);
        Vector3 finalPos = transform.position;
        finalPos.x = rightEdgeX - (finalWidth / 2f);
        transform.position = finalPos;

        isShrinking = false;
    }

    // ✅ **New: Reset the platform after the player falls**
    public void ResetPlatform()
    {
        StopAllCoroutines();  // Stop any ongoing shrinking animation
        isShrinking = false;

        // Restore platform to original size and position
        transform.localScale = new Vector3(initialWidth, transform.localScale.y, transform.localScale.z);
        transform.position = new Vector3(rightEdgeX - (initialWidth / 2f), transform.position.y, transform.position.z);

        Debug.Log($"{gameObject.name} reset to original size!");
    }
}

