using System.Collections;
using UnityEngine;
using UnityEngine.Windows;

public class AttackState : IEnemyState
{
    void IEnemyState.Enter(EnemyAI enemy)
    {
        Vector2 dir = enemy.player.position - enemy.transform.position;
        enemy.targetPos = enemy.rigidBody.position + dir.normalized/4f;
        enemy.moveSpeed = 1f;
        enemy.StartCoroutine(DoAttack(enemy));
    }

    void IEnemyState.Update(EnemyAI enemy)
    {
    }

    void IEnemyState.Exit(EnemyAI enemy)
    {
        enemy.attackHitbox.enabled = false;
    }

    private IEnumerator DoAttack(EnemyAI enemy)
    {
        enemy.animator.SetBool("isAttacking", true);
        yield return new WaitForSeconds(0.7f);
        enemy.attackHitbox.enabled = true;
        enemy.attackHitbox.offset = new Vector2(enemy.facingDir.x, enemy.attackHitbox.offset.y);
        yield return new WaitForSeconds(0.2f);
        enemy.attackHitbox.enabled = false;
        yield return new WaitForSeconds(0.6f);
        enemy.animator.SetBool("isAttacking", false);
        yield return new WaitForSeconds(Random.Range(0, 0.1f));

        enemy.ChangeState(new ChaseState());
    }
}
