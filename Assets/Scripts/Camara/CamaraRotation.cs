using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    public float rotationSpeed = 120f;

    void Update()
    {
        float h = Input.GetAxis("Horizontal"); // A / D

        if (Mathf.Abs(h) > 0.2f)
        {
            transform.Rotate(Vector3.up * h * rotationSpeed * Time.deltaTime);
        }
    }
}
 