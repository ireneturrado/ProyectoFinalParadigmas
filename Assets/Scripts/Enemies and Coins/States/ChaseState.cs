using UnityEngine;

public class ChaseState : IEnemyState
{
    private readonly EnemyAIController ai;
    private float lostTimer = 0f;
    private float lostTimeToSearch;

    public ChaseState(EnemyAIController ai)
    {
        this.ai = ai;
        lostTimeToSearch = ai.enemyType == EnemyType.Scout ? 0.8f : 2.0f;
    }

    public void Enter()
    {
        ai.SetMoveSpeed(ai.chaseSpeed);
        if (ai.player != null)
            ai.RequestPathTo(ai.player.position);
    }

    public void Tick()
    {
        if (ai.player == null) return;

        if (ai.CanSeePlayer())
        {
            ai.NotifyPlayerSpotted();
            lostTimer = 0f;
            ai.TickRepathTo(ai.player.position);
        }
        else
        {
            lostTimer += Time.deltaTime;

            if (ai.HasLastKnownPos)
                ai.TickRepathTo(ai.LastKnownPlayerPos);
        }

        ai.FollowRoute();

        if (lostTimer >= lostTimeToSearch)
            ai.SetState(new SearchState(ai));
    }

    public void Exit()
    {
        ai.ClearPath();
    }
}