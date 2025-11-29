using UnityEngine;
using UnityEngine.Windows;

public class PatrolState : IEnemyState
{
    void IEnemyState.Enter(EnemyAI enemy)
    {
        enemy.attackHitbox.enabled = false;
        enemy.moveSpeed = 2f;
        SetRandomTarget(enemy);
    }

    void IEnemyState.Update(EnemyAI enemy)
    {
        Debug.DrawRay(enemy.transform.position, enemy.facingDir * 1f, Color.red);
        bool wallHit = Physics2D.Raycast(enemy.transform.position, enemy.facingDir, 1f, LayerMask.GetMask("Wall"));

        Debug.DrawRay(enemy.transform.position + new Vector3(enemy.facingDir.x, enemy.facingDir.y), Vector2.down*1.5f, Color.red, 1f);
        bool cliffHit = Physics2D.Raycast(enemy.transform.position + new Vector3(enemy.facingDir.x, enemy.facingDir.y), Vector2.down*1.5f, 1f, LayerMask.GetMask("Ground"));

        if (Mathf.Abs(enemy.transform.position.x - enemy.targetPos.x) < 0.2f)
        {
            SetRandomTarget(enemy);
        }
        if (wallHit)
        {
            SetRandomTarget(enemy);
        }
        if(!cliffHit)
        {
            SetRandomTarget(enemy);
        }

        if (Vector3.Distance(enemy.transform.position, enemy.player.position) < 10f)
        {
            enemy.ChangeState(new ChaseState());
        }
        else if (Vector3.Distance(enemy.transform.position, enemy.player.position) < 2.75f)
        {
            enemy.ChangeState(new AttackState());
        }
    }

    void IEnemyState.Exit(EnemyAI enemy)
    {

    }

    private void SetRandomTarget(EnemyAI enemy)
    {
        Vector2 enemyPos = enemy.transform.position;
        enemy.targetPos = new Vector2(enemyPos.x + Random.Range(-5f, 5f), enemyPos.y);
    }
}
