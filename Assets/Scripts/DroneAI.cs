using UnityEngine;
using UnityEngine.AI;

public class DroneAI : MonoBehaviour
{
    enum State { Patrol, Chase, Search }
    State currentState = State.Patrol;

    NavMeshAgent agent;
    DronePatrol patrol;
    DroneVision vision;

    Transform player;
    Vector3 lastKnownPos;
    float searchTimer;

    public float chaseSpeed = 5f;
    public float patrolSpeed = 3.5f;
    public float searchTime = 4f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        patrol = GetComponent<DronePatrol>();
        vision = GetComponent<DroneVision>();

        player = GameObject.FindGameObjectWithTag("Player").transform;

        GoPatrol();
    }

    void Update()
    {
        switch (currentState)
        {
            case State.Patrol:
                if (vision.CanSeePlayer)
                    GoChase();
                break;

            case State.Chase:
                if (vision.CanSeePlayer)
                {
                    lastKnownPos = player.position;
                    agent.SetDestination(lastKnownPos);
                }
                else
                {
                    GoSearch();
                }
                break;

            case State.Search:
                searchTimer -= Time.deltaTime;
                agent.SetDestination(lastKnownPos);

                if (vision.CanSeePlayer)
                    GoChase();
                else if (searchTimer <= 0f)
                    GoPatrol();
                break;
        }
    }

    void GoPatrol()
    {
        currentState = State.Patrol;
        agent.speed = patrolSpeed;
        patrol.enabled = true;
    }

    void GoChase()
    {
        currentState = State.Chase;
        agent.speed = chaseSpeed;
        patrol.enabled = false;
        lastKnownPos = player.position;
        agent.SetDestination(lastKnownPos);
    }

    void GoSearch()
    {
        currentState = State.Search;
        agent.speed = patrolSpeed;
        patrol.enabled = false;
        searchTimer = searchTime;
    }
}
