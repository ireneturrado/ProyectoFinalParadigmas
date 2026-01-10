using UnityEngine;

public class CamCollision : MonoBehaviour
{
    public Transform cameraTransform;
    public float minDistance = 1f;
    public float maxDistance = 4.5f;
    public float smooth = 10f;

    void LateUpdate()
    {
        Vector3 direction = cameraTransform.localPosition.normalized;
        float desiredDistance = maxDistance;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.forward, out hit, maxDistance))
        {
            desiredDistance = Mathf.Clamp(hit.distance, minDistance, maxDistance);
        }

        Vector3 targetPos = direction * -desiredDistance;
        cameraTransform.localPosition = Vector3.Lerp(
            cameraTransform.localPosition,
            targetPos,
            Time.deltaTime * smooth
        );
    }
}
