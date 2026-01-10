using UnityEngine;

public class DroneVision : MonoBehaviour
{
    [Header("References")]
    public Transform eyes;

    [Header("Vision")]
    public float viewDistance = 12f;
    [Range(1f, 179f)] public float viewAngle = 45f;

    [Header("Layers")]
    public LayerMask obstacleMask; // muros/columnas
    public LayerMask playerMask;   // jugador

    public bool CanSeePlayer { get; private set; }

    Transform player;

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    void Update()
    {
        if (player == null || eyes == null)
        {
            CanSeePlayer = false;
            return;
        }

        CanSeePlayer = CheckVision();
    }

    bool CheckVision()
    {
        Vector3 dirToPlayer = (player.position - eyes.position);
        float dist = dirToPlayer.magnitude;

        // 1) Distancia
        if (dist > viewDistance) return false;

        // 2) Ángulo (cono)
        float angle = Vector3.Angle(eyes.forward, dirToPlayer);
        if (angle > viewAngle) return false;

        // 3) Raycast (muro en medio?)
        if (Physics.Raycast(eyes.position, dirToPlayer.normalized, dist, obstacleMask))
            return false;

        return true;
    }

    void OnDrawGizmos()
    {
        if (eyes == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(eyes.position, viewDistance);

        Gizmos.color = CanSeePlayer ? Color.red : Color.green;
        Gizmos.DrawLine(eyes.position, eyes.position + eyes.forward * viewDistance);
    }
}
