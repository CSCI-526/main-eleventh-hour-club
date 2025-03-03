using UnityEngine;

public class DroppingPlatform : MonoBehaviour
{
    public GameObject dropFloor;
    private bool hasDropped = false;

    void Start()
    {
        dropFloor.SetActive(true);
    }

    public void TriggerDrop()
    {
        if (!hasDropped)
        {
            hasDropped = true;
            dropFloor.SetActive(false);
        }
    }
}
