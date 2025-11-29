using UnityEngine;

public class PerchState : IEnemyState
{
    public void Enter(EnemyAI enemy)
    {
        enemy.moveSpeed = 2f;
        enemy.rigidBody.gravityScale = 2f;
        //enemy.attackHitbox.enabled = false;
    }
    public void Update(EnemyAI enemy)
    {
        Debug.DrawRay(enemy.transform.position, enemy.facingDir * 1f, Color.red);
        bool wallHit = Physics2D.Raycast(enemy.transform.position, enemy.facingDir, 1f, LayerMask.GetMask("Wall"));

        if (Mathf.Abs(enemy.transform.position.x - enemy.targetPos.x) < 0.2f)
        {
            SetRandomTarget(enemy);
        }
        if (wallHit)
        {
            SetRandomTarget(enemy);
        }

        if (Vector3.Distance(enemy.transform.position, enemy.player.position) < 10f)
        {
            enemy.ChangeState(new FlyerChaseState(true));
        }
    }

    public void Exit(EnemyAI enemy)
    {

    }

    private void SetRandomTarget(EnemyAI enemy)
    {
        Vector2 enemyPos = enemy.transform.position;
        enemy.targetPos = new Vector2(enemyPos.x + Random.Range(-5f, 5f), enemyPos.y + 0.1f);
    }
}
