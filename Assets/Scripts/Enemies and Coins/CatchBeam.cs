using UnityEngine;

public class CatchBeam : MonoBehaviour
{
    public LineRenderer line;
    public float duration = 0.4f;

    public void Fire(Transform from, Transform to)
    {
        Debug.Log("CATCH BEAM FIRE"); 

        if (line == null || from == null || to == null)
        {
            Debug.LogError("LineRenderer NO asignado");
            return;
        }

        line.enabled = true;
        line.positionCount = 2;

        // Un poco por encima del dron
        Vector3 start = from.position + Vector3.up * 1.2f;
        Vector3 end = to.position + Vector3.up * 1.0f;

        line.SetPosition(0, start);
        line.SetPosition(1, end);

        Destroy(gameObject, duration);
    }
}
