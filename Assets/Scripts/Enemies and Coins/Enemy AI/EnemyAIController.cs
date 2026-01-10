using UnityEngine;
using System.Collections.Generic;
using System.Linq;


public enum EnemyType
{
    Scout,
    Guardian
}


public class EnemyAIController : MonoBehaviour
{
    [Header("References")]
    public Transform player;

    [Header("Vision")]
    public float viewDistance = 12f;
    [Range(1f, 179f)] public float viewAngle = 60f;
    public LayerMask obstacleMask;

    [Header("Movement")]
    public float patrolSpeed = 3.5f;
    public float chaseSpeed = 5.5f;

    [Header("Search")]
    public float searchDuration = 5f;
    public float searchRadius = 4f;

    [Header("Catch")]
    public float catchDistance = 2.0f;

    [Header("Catch VFX")]
    public CatchBeam catchBeamPrefab;
    public float catchDelay = 0.4f;
    private bool isCatching = false;

    [Header("Separation")]
    public float separationRadius = 1.2f;
    public float separationStrength = 2.0f;

    [Header("Waypoints")]
    public Transform[] patrolPoints;

    [Header("Enemy Type")]
    public EnemyType enemyType;

    [Header("Catch SFX")]
    public GameObject catchSfxPrefab;

    [Header("Manual Navigation")]
    public float moveSpeed = 3.5f;
    public float rotationSpeed = 240f;
    public float nodeReachDistance = 0.3f;
    public float repathInterval = 0.25f;

    private float repathTimer = 0f;

    [Header("Stuck Detection")]
    public float stuckTime = 0.3f;
    public float minMoveDelta = 0.02f;

    private Vector3 lastPosition;
    private float stuckTimer = 0f;

    // Ruta actual
    private List<Transform> currentRoute = new List<Transform>();
    private int currentRouteIndex = 0;

    // Grafo de navegación (lo asignaremos luego)
    public NavGraph graph;

    // FSM
    private IEnemyState currentState;

    // memoria para búsqueda
    public Vector3 LastKnownPlayerPos { get; private set; }
    public bool HasLastKnownPos { get; private set; }

    protected virtual void Awake()
    {
        lastPosition = transform.position;
        currentRoute = new List<Transform>();
    }

    protected virtual void Start()
    {
        if (player == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        ConfigureByType();         
        SetState(new PatrolState(this));
    }

    protected virtual void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.State != GameState.Playing)
            return;

        currentState?.Tick();

        float moved = Vector3.Distance(transform.position, lastPosition);

        if (moved < minMoveDelta)
        {
            stuckTimer += Time.deltaTime;
        }
        else
        {
            stuckTimer = 0f;
            lastPosition = transform.position;
        }

        float effectiveStuckTime = IsGlobalAlertActive()
            ? stuckTime * 0.5f
            : stuckTime;

        if (stuckTimer >= effectiveStuckTime)
        {
            ClearPath();

            // 1️⃣ intentar ver una salida con los ojos
            if (FindFreeDirection(out Vector3 freeDir))
            {
                // girar hacia la salida
                Quaternion rot = Quaternion.LookRotation(freeDir);
                transform.rotation = rot;

                // avanzar un poco
                transform.position += freeDir * 0.6f;
            }
            else
            {
                // 2️⃣ si no ve nada, retrocede
                Vector3 backEscape = GetBackwardEscapePoint();
                RequestPathTo(backEscape);
            }

            stuckTimer = 0f;
            lastPosition = transform.position;
        }

        if (!isCatching && player != null &&
            Vector3.Distance(transform.position, player.position) <= catchDistance)
        {
            StartCoroutine(CatchSequence());
        }
    }

    public void SetState(IEnemyState next)
    {
        currentState?.Exit();
        currentState = next;
        currentState?.Enter();
    }


    public void RememberPlayerPos(Vector3 pos)
    {
        LastKnownPlayerPos = pos;
        HasLastKnownPos = true;
    }

    public void ClearLastKnown()
    {
        HasLastKnownPos = false;
    }

    public bool CanSeePlayer()
    {
        if (player == null) return false;

        // Vector REAL al jugador
        Vector3 toPlayerFull = player.position - transform.position;

        float dist = toPlayerFull.magnitude;

        float effectiveViewDistance = IsGlobalAlertActive()
        ? viewDistance * 1.4f
        : viewDistance;

        if (dist > effectiveViewDistance) return false;


        // Vector SOLO para el ángulo (en plano XZ)
        Vector3 toPlayerFlat = toPlayerFull;
        toPlayerFlat.y = 0f;

        Vector3 facing = transform.forward;
        float angle = Vector3.Angle(facing, toPlayerFlat);
        if (angle > viewAngle * 0.5f) return false;

        // Raycast a altura de ojos
        Vector3 origin = transform.position + Vector3.up * 1.6f;

        Debug.DrawRay(origin, toPlayerFull.normalized * dist, Color.red);

        if (Physics.Raycast(origin, toPlayerFull.normalized, dist, obstacleMask))
            return false;

        RememberPlayerPos(player.position);
        return true;
    }

    // Hook para extensiones (Guardian)
    protected virtual void OnPlayerSpotted()
    {
        // Scout: nada extra
    }

    // Método “seguro” para usar desde States
    public void NotifyPlayerSpotted()
    {
        GameManager.Instance?.ReportPlayerDetected();
        OnPlayerSpotted();
    }

    public void ConfigureByType()
    {
        switch (enemyType)
        {
            case EnemyType.Scout:
                // Scout: rápido, ve lejos pero con visión estrecha
                patrolSpeed = 3f;
                chaseSpeed = 4f;
                viewDistance = 18f;   // ve a larga distancia
                viewAngle = 45f;     // visión estrecha
                break;

            case EnemyType.Guardian:
                // Guardian: más lento, visión amplia pero menos alcance
                patrolSpeed = 2f;
                chaseSpeed = 2.7f;
                viewDistance = 12f;  // menos distancia
                viewAngle = 100f;    // campo de visión amplio
                break;
        }
    }


    private System.Collections.IEnumerator CatchSequence()
    {
        if (isCatching) yield break;
        isCatching = true;

        //Crear rayo
        if (catchBeamPrefab != null && player != null)
        {
            CatchBeam beam = Instantiate(
                catchBeamPrefab,
                transform.position,
                Quaternion.identity
            );

            beam.Fire(transform, player);
        }

        if (catchSfxPrefab != null)
        {
            Instantiate(catchSfxPrefab, transform.position, Quaternion.identity);
        }

        // 2️⃣ Esperar un poco para que SE VEA
        yield return new WaitForSeconds(catchDelay);

        // 3️⃣ Game Over
        GameManager.Instance?.Lose();
    }

    public void SetMoveSpeed(float speed)
    {
        moveSpeed = speed;
    }

    public void RequestPathTo(Vector3 targetPosition)
    {
        if (graph == null) 
        {
            Debug.LogError("DRON SIN GRAFO");
            return;
        }
       

        Transform start = graph.GetClosestNode(transform.position);
        Transform goal = graph.GetClosestNode(targetPosition);

        if (start == null || goal == null) return;

        currentRoute = AStarPathfinder.FindPath(graph, start, goal);

        currentRouteIndex = 0;

    }

    public bool HasRoute()
    {
        return currentRoute != null && currentRouteIndex < currentRoute.Count;
    }

    public void FollowRoute()
    {
        if (player != null && Vector3.Distance(transform.position, player.position) < 3.0f)
        {
            Vector3 dir = (player.position - transform.position).normalized;

            transform.position += dir * moveSpeed * Time.deltaTime;

            Quaternion rot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                rot,
                rotationSpeed * Time.deltaTime
            );

            return;
        }

        if (!HasRoute()) return;

        Transform targetNode = currentRoute[currentRouteIndex];
        Vector3 target = targetNode.position;
        target.y = transform.position.y;

        Vector3 toTarget = target - transform.position;
        float distance = toTarget.magnitude;

        if (distance <= nodeReachDistance)
        {
            currentRouteIndex++;
            return;
        }

        Vector3 direction = toTarget.normalized;

        // --- Separación entre drones (avoidance manual) ---
        Vector3 separation = Vector3.zero;
        Collider[] nearby = Physics.OverlapSphere(transform.position, separationRadius);

        foreach (var c in nearby)
        {
            if (c.gameObject == gameObject) continue;

            EnemyAIController other = c.GetComponent<EnemyAIController>();
            if (other == null) continue;

            Vector3 away = transform.position - other.transform.position;
            if (away.magnitude > 0.001f)
                separation += away.normalized / away.magnitude;
        }

        // Dirección final combinada
        Vector3 finalDir = (direction + separation * separationStrength).normalized;

        // ROTAR HACIA LA DIRECCIÓN REAL DE MOVIMIENTO
        if (finalDir.sqrMagnitude > 0.0001f)
        {
            Quaternion rot = Quaternion.LookRotation(finalDir);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                rot,
                rotationSpeed * Time.deltaTime
            );
        }

        // MOVER
        transform.position += finalDir * moveSpeed * Time.deltaTime;
    }


    public void TickRepathTo(Vector3 targetPosition)
    {

        repathTimer -= Time.deltaTime;
        if (repathTimer <= 0f)
        {
            RequestPathTo(targetPosition);
            repathTimer = repathInterval;
        }
    }


    public void ClearPath()
    {
        currentRouteIndex = 0;
        if (currentRoute != null)
            currentRoute.Clear();
    }

    public Vector3 GetRandomPatrolTarget()
    {
        // 1️⃣ Si hay waypoints, usar uno aleatorio
        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            Transform wp = patrolPoints[Random.Range(0, patrolPoints.Length)];
            return wp.position;
        }

        // 2️⃣ Si no, moverse aleatoriamente alrededor
        Vector3 random = transform.position + Random.insideUnitSphere * 6f;
        random.y = transform.position.y;
        return random;
    }
    public bool RouteIsTrivial()
    {
        return currentRoute == null || currentRoute.Count <= 1;
    }

    public Vector3 GetBackwardEscapePoint(float distance = 1.2f)
    {
        Vector3 back = -transform.forward * distance;
        Vector3 candidate = transform.position + back;

        candidate.y = transform.position.y;

        // Forzar que el punto esté en el grafo
        if (graph != null)
        {
            Transform node = graph.GetClosestNode(candidate);
            if (node != null)
                return node.position;
        }

        return transform.position;
    }

    public bool FindFreeDirection(out Vector3 freeDir)
    {
        Vector3 origin = transform.position + Vector3.up * 1.6f;

        int rays = 12;              // 360° / 12 = cada 30 grados
        float maxDist = 2.0f;

        for (int i = 0; i < rays; i++)
        {
            float angle = i * (360f / rays);
            Vector3 dir = Quaternion.Euler(0, angle, 0) * transform.forward;

            Debug.DrawRay(origin, dir * maxDist, Color.cyan, 0.2f);

            if (!Physics.Raycast(origin, dir, maxDist, obstacleMask))
            {
                freeDir = dir.normalized;
                return true;
            }
        }

        freeDir = Vector3.zero;
        return false;
    }

    public bool IsGlobalAlertActive()
    {
        return GameManager.Instance != null && GameManager.Instance.GlobalAlertActive;
    }




}
