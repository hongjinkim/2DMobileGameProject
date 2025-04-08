using UnityEngine;

public struct AttackInfo
{
    public float Damage;
    public EAttackType AttackType;
    public EFXPoolType EffectType;
    public int HitCount;
    public EAttackerType AttackerType;
    public Vector3 AttackerWorldPosition;
    public int AttackerIndex;
}

public enum EAttackerType
{
    None,
    Hero,
    Tower,
    Monster,
}
