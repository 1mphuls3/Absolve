using UnityEngine;

public class IdleWispState : IEnemyState
{
    public void Enter(EnemyAI enemy)
    {
        enemy.targetPos = enemy.transform.position;
    }
    public void Update(EnemyAI enemy)
    {
        if (Vector3.Distance(enemy.transform.position, enemy.player.position) < 10f)
        {
            enemy.ChangeState(new AttackingWispState());
        }
    }

    public void Exit(EnemyAI enemy)
    {

    }
}
