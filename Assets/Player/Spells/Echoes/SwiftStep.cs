using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "SwiftStep", menuName = "Scriptable Objects/SwiftStep")]
public class SwiftStep : EchoAbillity
{
    [SerializeField] private int dashCost;
    [SerializeField] private float dashDistance = 1f;

    public override void OnEquip(Player player)
    {
        player.movement.dashDistance *= dashDistance;
    }

    public override void OnUnequip(Player player)
    {
        player.movement.dashDistance /= dashDistance;
    }

    public override void OnDashStart(Player player) 
    {
        player.health.Damage(dashCost, DamageSource.Self);
    }
}
