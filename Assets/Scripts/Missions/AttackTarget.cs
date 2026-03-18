using System;
using UnityEngine;

[Serializable]
public class AttackTarget
{
    public string targetId;
    public string displayName;
    public TargetType targetType;

    public AIFactionType factionType;
    public AITargetTier tier;

    public Fleet defendingFleet;

    public int defenseAttack;
    public int defenseHP;
    public int defenseShield;

    public double metal;
    public double crystal;
    public double gas;

    public AttackTarget(string displayName, AIFactionType factionType, AITargetTier tier)
    {
        this.targetId = IdUtil.NewId("ai");
        this.displayName = displayName;
        this.targetType = TargetType.AITarget;
        this.factionType = factionType;
        this.tier = tier;
        this.defendingFleet = new Fleet(this.targetId);
    }
}