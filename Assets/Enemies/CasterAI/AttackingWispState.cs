using System.Collections;
using UnityEngine;

public class AttackingWispState : IEnemyState
{
    public void Enter(EnemyAI enemy)
    {
        enemy.targetPos = enemy.player.position;
        enemy.StartCoroutine(CastSpell((EnemyCastingAI)enemy));
    }
    public void Update(EnemyAI enemy)
    {
    }

    public void Exit(EnemyAI enemy)
    {

    }

    private IEnumerator CastSpell(EnemyCastingAI enemy)
    {
        GameObject spell = Object.Instantiate(enemy.spell, enemy.targetPos, Quaternion.identity);
        yield return new WaitForSeconds(2f + Random.Range(-1f, 1f));

        enemy.ChangeState(new IdleWispState());
    }
}
