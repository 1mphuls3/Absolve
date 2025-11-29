using System.Collections;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class FlyerAttackState : IEnemyState
{
    void IEnemyState.Enter(EnemyAI enemy)
    {
        enemy.moveSpeed = 0f;
        enemy.rigidBody.gravityScale = 0f;
        float offsetX = 0.5f * Mathf.Sign(enemy.transform.position.x - enemy.player.position.x);
        float offsetY = 0.5f * Mathf.Sign(enemy.transform.position.y - enemy.player.position.y);
        Vector2 targetPos = new Vector2(enemy.player.position.x + offsetX, enemy.player.position.y + offsetY);

        enemy.StartCoroutine(DoAttack((EnemyFlyingAI)enemy, enemy.rigidBody.position, targetPos));
    }
    void IEnemyState.Update(EnemyAI enemy)
    {
    }
    void IEnemyState.Exit(EnemyAI enemy)
    {

    }

    private IEnumerator DoAttack(EnemyFlyingAI enemy, Vector2 startPos, Vector2 endPos)
    {
        Vector2 backDir = (endPos - startPos).normalized;
        Vector2 backPos = startPos - backDir;
        float backDuration = 0.2f;

        // Fly back a little bit before attacking for predictability
        float elapsed = 0f;
        while (elapsed < backDuration)
        {
            elapsed += Time.deltaTime;
            float t = enemy.curve.Evaluate(Mathf.Clamp01(elapsed / backDuration));
            enemy.transform.position = Vector3.Lerp(startPos, backPos, t);
            yield return null;
        }

        endPos = enemy.player.position;
        Vector2 direction = (endPos - startPos).normalized;
        Vector2 newEndPos = endPos + direction;
        float distance = Vector3.Distance(backPos, newEndPos);
        float duration = distance / 12f;

        // Move from the backward pos to the end targetc
        elapsed = 0f;
        enemy.animator.SetBool("isAttacking", true);
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = enemy.curve.Evaluate(Mathf.Clamp01(elapsed / (duration)));
            enemy.rigidBody.MovePosition(Vector3.Lerp(backPos, newEndPos, t));
            yield return null;
        }

        enemy.animator.SetBool("isAttacking", false);

        enemy.ChangeState(new FlyerChaseState(false));
    }
}
