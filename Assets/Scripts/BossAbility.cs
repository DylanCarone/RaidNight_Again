using System;

[Serializable]
public class BossAbility
{
    public string abilityName;
    public AttackType attackType;
    public float castTime;
    public float damage;
    public float range;
    public bool isInterruptible = false;
    public SpecialAnimations animation;
}


public enum SpecialAnimations
{
    Special,
    Special_2
}

public enum AttackType
{
    SingleTarget,
    RaidWide,
    LineAOE,
    AreaAOE
}