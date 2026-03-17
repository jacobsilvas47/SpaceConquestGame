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

    public DefenseData(
        DefenseType defenseType,
        string displayName,
        int metalCost,
        int crystalCost,
        int gasCost,
        int attack,
        int hp,
        int shield = 0)
    {
        this.defenseType = defenseType;
        this.displayName = displayName;
        this.metalCost = metalCost;
        this.crystalCost = crystalCost;
        this.gasCost = gasCost;
        this.attack = attack;
        this.hp = hp;
        this.shield = shield;
    }
}