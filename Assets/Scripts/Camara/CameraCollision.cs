using UnityEngine;

public class CameraCollision : MonoBehaviour
{
    public Transform target;      // Player
    public float distance = 6f;
    public float height = 1.6f;
    public float smooth = 10f;
    public LayerMask obstacleMask;

    void LateUpdate()
    {
        Vector3 desiredPos = target.position
                             + Vector3.up * height
                             - target.forward * distance;

        RaycastHit hit;
        if (Physics.Linecast(target.position + Vector3.up * height,
                             desiredPos,
                             out hit,
                             obstacleMask))
        {
            desiredPos = hit.point + hit.normal * 0.3f;
        }

        transform.position = Vector3.Lerp(transform.position, desiredPos, smooth * Time.deltaTime);
        transform.LookAt(target.position + Vector3.up * height);
    }
}
