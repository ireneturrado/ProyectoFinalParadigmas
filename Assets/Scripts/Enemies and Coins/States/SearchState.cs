using UnityEngine;

public class SearchState : IEnemyState
{
    private readonly EnemyAIController ai;
    private float timer;

    public SearchState(EnemyAIController ai)
    {
        this.ai = ai;
    }

    public void Enter()
    {
        timer = ai.searchDuration;
        ai.SetMoveSpeed(ai.patrolSpeed);
        PickNewSearchPoint();
    }

    public void Tick()
    {
        if (ai.CanSeePlayer())
        {
            ai.NotifyPlayerSpotted();
            ai.SetState(new ChaseState(ai));
            return;
        }

        timer -= Time.deltaTime;

        if (!ai.HasRoute() || ai.RouteIsTrivial())
            PickNewSearchPoint();

        ai.FollowRoute();


        if (timer <= 0f)
        {
            ai.ClearLastKnown();
            ai.SetState(new PatrolState(ai));
        }
    }

    public void Exit()
    {
        ai.ClearPath();
    }

    private void PickNewSearchPoint()
    {
        Vector3 center = ai.HasLastKnownPos ? ai.LastKnownPlayerPos : ai.transform.position;
        Vector3 random = center + Random.insideUnitSphere * ai.searchRadius;
        random.y = center.y;

        ai.RequestPathTo(random);
    }
}