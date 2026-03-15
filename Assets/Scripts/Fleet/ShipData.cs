using System;
using System.Collections.Generic;

[Serializable]
public class ShipData
{
    public string displayName;

    public int maxHp;
    public int attack;
    public int defense;
    public int speed;
    public int cargoCapacity;

    public int buildTimeSeconds;

    public int metalCost;
    public int crystalCost;
    public int gasCost;

    public List<Requirement> requirements = new List<Requirement>();

    public ShipData(
        string displayName,
        int maxHp,
        int attack,
        int defense,
        int speed,
        int cargoCapacity,
        int buildTimeSeconds,
        int metalCost,
        int crystalCost,
        int gasCost,
        List<Requirement> requirements = null)
    {
        this.displayName = displayName;
        this.maxHp = maxHp;
        this.attack = attack;
        this.defense = defense;
        this.speed = speed;
        this.cargoCapacity = cargoCapacity;
        this.buildTimeSeconds = buildTimeSeconds;
        this.metalCost = metalCost;
        this.crystalCost = crystalCost;
        this.gasCost = gasCost;
        this.requirements = requirements ?? new List<Requirement>();
    }
}