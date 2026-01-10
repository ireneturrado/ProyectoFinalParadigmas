using UnityEngine;

public class GuardianAI : EnemyAIController
{
    protected override void OnPlayerSpotted()
    {
        base.OnPlayerSpotted();
        GameManager.Instance?.TriggerGlobalAlert();
    }
}
