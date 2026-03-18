[System.Serializable]
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
}