// using UnityEngine;

// public class DroppingPlatform : MonoBehaviour
// {
//     public GameObject dropFloor;
//     private bool hasDropped = false;

//     void Start()
//     {
//         dropFloor.SetActive(true);
//     }

//     public void TriggerDrop()
//     {
//         if (!hasDropped)
//         {
//             hasDropped = true;
//             dropFloor.SetActive(false);
//         }
//     }
// }

// using UnityEngine;

// public class DroppingPlatform : MonoBehaviour
// {
//     public GameObject dropFloor;
//     private bool hasDropped = false;
//     private Vector3 initialPosition;

//     void Start()
//     {
//         if (dropFloor == null)
//         {
//             Debug.LogError("DropFloor is NOT assigned in the Inspector!");
//             return;
//         }

//         initialPosition = dropFloor.transform.position;
//         dropFloor.SetActive(true);
//     }

//     public void TriggerDrop()
//     {
//         if (!hasDropped)
//         {
//             hasDropped = true;
//             dropFloor.SetActive(false);
//             Debug.Log($"DropFloor {dropFloor.name} dropped!");
//         }
//     }

//     public void ResetPlatform()
//     {
//         if (dropFloor == null)
//         {
//             Debug.LogError("DropFloor is missing during Reset!");
//             return;
//         }

//         // Reset properties
//         hasDropped = false;
//         dropFloor.transform.position = initialPosition;
//         dropFloor.SetActive(true);

//         Debug.Log($"DropFloor {dropFloor.name} reset to original position!");
//     }
// }

using UnityEngine;

public class DroppingPlatform : MonoBehaviour
{
    public GameObject dropFloor;
    private bool hasDropped = false;
    private Vector3 initialPosition;

    [Header("Should the platform be hidden at start? (Set this for Level 3 & 4)")]
    public bool startHidden = false; 

    void Start()
    {
        if (dropFloor == null)
        {
            Debug.LogError("DropFloor is NOT assigned in the Inspector!");
            return;
        }

        initialPosition = dropFloor.transform.position;
        
        if (startHidden)
        {
            dropFloor.SetActive(false); // Hide platform only if startHidden is enabled
        }
        else
        {
            dropFloor.SetActive(true); // Keep visible for Level 1 & 2
        }
    }

    public void TriggerDrop()
    {
        if (!hasDropped)
        {
            hasDropped = true;
            dropFloor.SetActive(false);
            Debug.Log($"DropFloor {dropFloor.name} dropped!");
        }
    }

    public void ResetPlatform()
    {
        if (dropFloor == null)
        {
            Debug.LogError("DropFloor is missing during Reset!");
            return;
        }

        hasDropped = false;
        dropFloor.transform.position = initialPosition;

        if (startHidden)
        {
            dropFloor.SetActive(false); // Hide it again for Level 3 & 4
        }
        else
        {
            dropFloor.SetActive(true); // Keep it visible for Level 1 & 2
        }

        Debug.Log($"DropFloor {dropFloor.name} reset to original position!");
    }

    // ✅ **Make the platform appear when the player is near (only for hidden platforms)**
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && startHidden)
        {
            dropFloor.SetActive(true); // Platform appears only if it was hidden
        }
    }
}


