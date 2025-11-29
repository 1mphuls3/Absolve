using Unity.Burst.Intrinsics;
using UnityEngine;


// TODO implement Rarity generation and stuff
public struct RarityData
{
    public enum Rarity : int
    {
        Common = 0,
        Uncommon = 1,
        Rare = 2,
        Legendary = 3,
    }
    public float GetProbability(Rarity rarity)
    {
        switch(rarity){
            case Rarity.Common: return 0.6f;
            case Rarity.Uncommon: return 0.25f;
            case Rarity.Rare: return 0.1f;
            case Rarity.Legendary: return 0.05f;
            default: return 0f;
        }
    }
}

[CreateAssetMenu(fileName = "EchoAbillity", menuName = "Scriptable Objects/EchoAbillity")]
public class EchoAbillity : ScriptableObject
{
    public string echoName;
    public string description;
    public Sprite icon;

    public virtual void OnEquip(Player player) { }
    public virtual void OnUnequip(Player player) { }
    public virtual void OnAttack(Player player) { }
    public virtual void OnHitEnemy(Player player, ref DamageData damage) { }
    public virtual void OnTakeDamage(Player player, ref DamageData damage) { }
    public virtual void OnTakeSelfDamage(Player player, ref DamageData damage) { }
    public virtual void OnDashStart(Player player) { }
    public virtual void OnDashComplete(Player player) { }
    public virtual void OnUpdate(Player player) { }

}
