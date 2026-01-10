using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    public float zoomSpeed = 2f;
    public float keyZoomSpeed = 3f;
    public float minZ = -2.5f;
    public float maxZ = -8f;

    void Update()
    {
        // Touchpad / ratón (scroll)
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        // Teclado (por si no hay scroll)
        // Q = acercar, E = alejar (puedes cambiarlo)
        float key = 0f;
        if (Input.GetKey(KeyCode.Q)) key += 1f;
        if (Input.GetKey(KeyCode.E)) key -= 1f;

        Vector3 pos = transform.localPosition;

        if (Mathf.Abs(scroll) > 0.001f)
            pos.z += scroll * zoomSpeed;

        if (Mathf.Abs(key) > 0.001f)
            pos.z += key * keyZoomSpeed * Time.deltaTime;

        pos.z = Mathf.Clamp(pos.z, maxZ, minZ);
        transform.localPosition = pos;
    }
}
