using UnityEngine;
using UnityEngine.AI;

public class DronePatrol : MonoBehaviour
{
    public Transform[] waypoints;
    public float hoverHeight = 2.5f;

    private NavMeshAgent agent;
    private int currentIndex = 0;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void SetWaypoints(Transform[] newWaypoints)
    {
        if (newWaypoints == null || newWaypoints.Length == 0)
        {
            Debug.LogError("DronePatrol: waypoints no asignados");
            return;
        }

        waypoints = newWaypoints;
        currentIndex = 0;
        agent.SetDestination(waypoints[currentIndex].position);
    }


    void Update()
    {
        if (waypoints == null || waypoints.Length == 0)
            return;

        // Movimiento de patrulla
        if (!agent.pathPending && agent.remainingDistance < 0.3f)
        {
            currentIndex = (currentIndex + 1) % waypoints.Length;
            agent.SetDestination(waypoints[currentIndex].position);
        }

        // Vuelo visual (offset en Y)
        Vector3 pos = agent.nextPosition;
        pos.y += hoverHeight;
        transform.position = pos;

        // Rotación hacia el movimiento
        if (agent.velocity.sqrMagnitude > 0.1f)
        {
            transform.rotation = Quaternion.LookRotation(agent.velocity);
        }
    }

}

