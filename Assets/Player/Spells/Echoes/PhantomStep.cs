using UnityEngine;

[CreateAssetMenu(fileName = "PhantomStep", menuName = "Scriptable Objects/PhantomStep")]
public class PhantomStep : EchoAbillity
{
    [SerializeField] private int healthCost;
    public override void OnDashStart(Player player)
    {
        Debug.Log("Dash!");
        player.health.canDamage = false;
        player.health.Damage(healthCost, DamageSource.Self);
        player.health.spriteRenderer.material.SetFloat("_TransparencyAmount", 0.5f);
    }
    public override void OnDashComplete(Player player)
    {
        player.health.canDamage = true;
        player.health.spriteRenderer.material.SetFloat("_TransparencyAmount", 1f);
    }

}
