using UnityEngine;

public struct DamageData
{
    public float baseDamage;
    public float flatMod;
    public float multiplier;

    public float Resolve()
    {
        return (baseDamage + flatMod) * multiplier;
    }
}
