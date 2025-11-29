using UnityEngine;

[CreateAssetMenu(fileName = "VengefulStrike", menuName = "Scriptable Objects/VengefulStrike")]
public class VengefulStrike : EchoAbillity
{
    [SerializeField] private float damageMult;
    private bool isVengeful;

    // When you take damage, activate vengeful strike
    public override void OnTakeDamage(Player player, ref DamageData damage) 
    {
        isVengeful = true;
    }

    // If vengeful strike is active when you attack, multiply the damage
    public override void OnHitEnemy(Player player, ref DamageData damage)
    {
        if (isVengeful)
        {
            damage.multiplier *= damageMult;
        }
        isVengeful = false;
    }
}
