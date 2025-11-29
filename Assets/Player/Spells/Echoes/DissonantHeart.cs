using UnityEngine;

[CreateAssetMenu(fileName = "DissonantHeart", menuName = "Scriptable Objects/DissonantHeart")]
public class DissonantHeart : EchoAbillity
{
    public override void OnTakeDamage(Player player, ref DamageData damage)
    {
        damage.multiplier *= 2;
    }

    public override void OnTakeSelfDamage(Player player, ref DamageData damage)
    {
        damage.multiplier *= 2;
    }

    public override void OnHitEnemy(Player player, ref DamageData damage)
    {
        damage.multiplier *= 2;
    }
}
