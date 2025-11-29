using UnityEngine;

[CreateAssetMenu(fileName = "WingsLost", menuName = "Scriptable Objects/WingsLost")]
public class WingsLost : EchoAbillity
{
    public override void OnEquip(Player player)
    {
        player.movement.maxJumpCount++;
        base.OnEquip(player);
    }

    public override void OnUnequip(Player player)
    {
        player.movement.maxJumpCount--;
        base.OnUnequip(player);
    }
}
