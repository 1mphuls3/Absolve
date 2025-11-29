using UnityEngine;

public class ChaseState : IEnemyState
{
    void IEnemyState.Enter(EnemyAI enemy)
    {
        enemy.attackHitbox.enabled = false;
        enemy.moveSpeed = 3f;
    }
    void IEnemyState.Update(EnemyAI enemy)
    {
        enemy.targetPos = enemy.player.position;
        Vector3 rayPos = enemy.transform.position + new Vector3((enemy.player.position - enemy.transform.position).normalized.x, 0f);
        Debug.DrawRay(rayPos, Vector2.down * 1.5f, Color.red, 1f);
        bool cliffHit = Physics2D.Raycast(rayPos, Vector2.down * 1.5f, 1f, LayerMask.GetMask("Ground"));

        if (!cliffHit)
        {
            enemy.moveSpeed = 0f;
        }
        else
        {
            enemy.moveSpeed = 3f;
        }

        if (Vector3.Distance(enemy.transform.position, enemy.player.position) > 10f)
        {
            enemy.ChangeState(new PatrolState());
        }
        else if (Vector3.Distance(enemy.transform.position, enemy.player.position) < 2.75f)
        {
            enemy.ChangeState(new AttackState());
        }
    }
    void IEnemyState.Exit(EnemyAI enemy)
    {

    }
}
