using System.Collections;
using UnityEngine;

public class FlyerChaseState : IEnemyState
{
    public bool canAttack;
    public FlyerChaseState(bool canAttack)
    {
        this.canAttack = canAttack;
    }

    void IEnemyState.Enter(EnemyAI enemy)
    {
        enemy.moveSpeed = 6f;
        enemy.rigidBody.gravityScale = 0f;
        //enemy.attackHitbox.enabled = false;
    }
    void IEnemyState.Update(EnemyAI enemy)
    {
        if(!canAttack)
        {
            enemy.StartCoroutine(AttackCooldown());
        }

        float offsetX = 2f*Mathf.Sign(enemy.transform.position.x - enemy.player.position.x);
        float offsetY = 2f;
        enemy.targetPos = new Vector2(enemy.player.position.x + offsetX, enemy.player.position.y + offsetY);

        if (Vector3.Distance(enemy.transform.position, enemy.player.position) < 3f && canAttack)
        {
            enemy.ChangeState(new FlyerAttackState());
        }
        else if (Vector3.Distance(enemy.transform.position, enemy.player.position) > 10f)
        {
            enemy.ChangeState(new PerchState());
        }
    }
    void IEnemyState.Exit(EnemyAI enemy)
    {

    }

    private IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(1);
        canAttack = true;
    }
}
