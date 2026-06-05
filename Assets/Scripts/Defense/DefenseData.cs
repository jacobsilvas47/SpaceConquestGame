using System.Collections.Generic;

public class DefenseData
{
    public DefenseType defenseType;
    public string displayName;

    public int metalCost;
    public int crystalCost;
    public int gasCost;

    public int attack;
    public int hp;
    public int shield;

    public List<Requirement> requirements = new List<Requirement>();

    public DefenseData(
        DefenseType defenseType,
        string displayName,
        int metalCost,
        int crystalCost,
        int gasCost,
        int attack,
        int hp,
        int shield = 0,
        List<Requirement> requirements = null)
    {
        this.defenseType = defenseType;
        this.displayName = displayName;
        this.metalCost = metalCost;
        this.crystalCost = crystalCost;
        this.gasCost = gasCost;
        this.attack = attack;
        this.hp = hp;
        this.shield = shield;

        if (requirements != null)
            this.requirements = requirements;
    }
}