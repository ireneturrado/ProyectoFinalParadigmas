public class PatrolState : IEnemyState
{
    private readonly EnemyAIController ai;
    private int idx = 0;

    public PatrolState(EnemyAIController ai)
    {
        this.ai = ai;
    }

    public void Enter()
    {
        ai.SetMoveSpeed(ai.patrolSpeed);
        GoNext();
    }

    public void Tick()
    {
        if (ai.CanSeePlayer())
        {
            ai.NotifyPlayerSpotted();
            ai.SetState(new ChaseState(ai));
            return;
        }

        if (!ai.HasRoute() || ai.RouteIsTrivial())
            GoNext();

        ai.FollowRoute();

    }

    public void Exit()
    {
        ai.ClearPath();
    }

    private void GoNext()
    {
        if (ai.patrolPoints == null || ai.patrolPoints.Length == 0)
            return;

        idx = (idx + 1) % ai.patrolPoints.Length;
        ai.RequestPathTo(ai.patrolPoints[idx].position);
    }
}