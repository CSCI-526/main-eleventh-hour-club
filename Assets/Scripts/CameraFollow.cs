using UnityEngine;

public class FollowHead : MonoBehaviour
{
    public GameObject target;
    public Vector3 offset = new Vector3(0, 1, 0);

    public float followSpeed = 5f;

    void LateUpdate()
    {
        if (target == null) return;

        transform.position = Vector3.Lerp(transform.position, target.transform.position + offset, followSpeed * Time.deltaTime);
    }
}
