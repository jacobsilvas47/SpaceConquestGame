using UnityEngine;
using System;

public static class AITargetGenerator
{
    public static AttackTarget CreateTarget(AIFactionType factionType, AITargetTier tier, string customName = null)
    {
        string name = string.IsNullOrEmpty(customName)
            ? GenerateName(factionType, tier)
            : customName;

        AttackTarget target = new AttackTarget(name, factionType, tier);

        ConfigureFleet(target);
        ConfigureDefenses(target);
        ConfigureResources(target);

        return target;
    }

    private static string GenerateName(AIFactionType factionType, AITargetTier tier)
    {
        switch (factionType)
        {
            case AIFactionType.Pirate:
                return $"Pirate Camp T{(int)tier}";
            case AIFactionType.Alien:
                return $"Alien Nest T{(int)tier}";
            case AIFactionType.Computer:
                return $"Computer Outpost T{(int)tier}";
            default:
                return $"Unknown Target T{(int)tier}";
        }
    }

    private static void ConfigureFleet(AttackTarget target)
    {
        int tierValue = (int)target.tier;

        switch (target.factionType)
        {
            case AIFactionType.Pirate:
                ConfigurePirateFleet(target, tierValue);
                break;

            case AIFactionType.Alien:
                ConfigureAlienFleet(target, tierValue);
                break;

            case AIFactionType.Computer:
                ConfigureComputerFleet(target, tierValue);
                break;
        }
    }

    private static void ConfigureDefenses(AttackTarget target)
    {
        int tierValue = (int)target.tier;

        switch (target.factionType)
        {
            case AIFactionType.Pirate:
                target.defenseAttack = 20 * tierValue;
                target.defenseHP = 100 * tierValue;
                target.defenseShield = 10 * tierValue;
                break;

            case AIFactionType.Alien:
                target.defenseAttack = 15 * tierValue;
                target.defenseHP = 180 * tierValue;
                target.defenseShield = 25 * tierValue;
                break;

            case AIFactionType.Computer:
                target.defenseAttack = 30 * tierValue;
                target.defenseHP = 140 * tierValue;
                target.defenseShield = 20 * tierValue;
                break;
        }
    }

    private static void ConfigureResources(AttackTarget target)
    {
        int tierValue = (int)target.tier;

        switch (target.factionType)
        {
            case AIFactionType.Pirate:
                target.metal = 800 * tierValue;
                target.crystal = 400 * tierValue;
                target.gas = 200 * tierValue;
                break;

            case AIFactionType.Alien:
                target.metal = 500 * tierValue;
                target.crystal = 600 * tierValue;
                target.gas = 500 * tierValue;
                break;

            case AIFactionType.Computer:
                target.metal = 1000 * tierValue;
                target.crystal = 700 * tierValue;
                target.gas = 350 * tierValue;
                break;
        }
    }

    private static void ConfigurePirateFleet(AttackTarget target, int tierValue)
    {
        target.defendingFleet.AddShips(ShipType.BasicFighter, 4 * tierValue);

        if (tierValue >= 2)
            target.defendingFleet.AddShips(ShipType.SmallCargo, 1 * tierValue);

        if (tierValue >= 3)
            target.defendingFleet.AddShips(ShipType.Interceptor, 2 * (tierValue - 1));
    }

    private static void ConfigureAlienFleet(AttackTarget target, int tierValue)
    {
        target.defendingFleet.AddShips(ShipType.BasicFighter, 3 * tierValue);

        if (tierValue >= 2)
            target.defendingFleet.AddShips(ShipType.Interceptor, 3 * tierValue);

        if (tierValue >= 4)
            target.defendingFleet.AddShips(ShipType.WarFrigate, tierValue - 2);
    }

    private static void ConfigureComputerFleet(AttackTarget target, int tierValue)
    {
        target.defendingFleet.AddShips(ShipType.BasicFighter, 3 * tierValue);
        target.defendingFleet.AddShips(ShipType.Interceptor, 2 * tierValue);

        if (tierValue >= 3)
            target.defendingFleet.AddShips(ShipType.SmallCargo, tierValue);

        if (tierValue >= 4)
            target.defendingFleet.AddShips(ShipType.Vanguard, tierValue - 3);
    }
}