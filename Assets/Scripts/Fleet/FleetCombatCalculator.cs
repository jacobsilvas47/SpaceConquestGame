using System.Collections.Generic;
using UnityEngine;

public static class FleetCombatCalculator
{
    public static int GetTotalAttack(Fleet fleet)
    {
        if (fleet == null) return 0;

        int total = 0;

        foreach (KeyValuePair<ShipType, int> kvp in fleet.ships)
        {
            ShipData data = ShipDatabase.Get(kvp.Key);
            if (data == null) continue;

            total += data.attack * kvp.Value;
        }

        return total;
    }

    public static int GetTotalDefense(Fleet fleet)
    {
        if (fleet == null) return 0;

        int total = 0;

        foreach (KeyValuePair<ShipType, int> kvp in fleet.ships)
        {
            ShipData data = ShipDatabase.Get(kvp.Key);
            if (data == null) continue;

            total += data.defense * kvp.Value;
        }

        return total;
    }

    public static int GetTotalHp(Fleet fleet)
    {
        if (fleet == null) return 0;

        int total = 0;

        foreach (KeyValuePair<ShipType, int> kvp in fleet.ships)
        {
            ShipData data = ShipDatabase.Get(kvp.Key);
            if (data == null) continue;

            total += data.maxHp * kvp.Value;
        }

        return total;
    }

    public static int GetTotalCargo(Fleet fleet)
    {
        if (fleet == null) return 0;

        int total = 0;

        foreach (KeyValuePair<ShipType, int> kvp in fleet.ships)
        {
            ShipData data = ShipDatabase.Get(kvp.Key);
            if (data == null) continue;

            total += data.cargoCapacity * kvp.Value;
        }

        return total;
    }

    public static int GetTotalShipCount(Fleet fleet)
    {
        if (fleet == null) return 0;

        int total = 0;

        foreach (KeyValuePair<ShipType, int> kvp in fleet.ships)
        {
            total += kvp.Value;
        }

        return total;
    }

    public static int GetCombatPowerScore(Fleet fleet)
    {
        if (fleet == null || fleet.ships == null) return 0;

        int total = 0;

        foreach (KeyValuePair<ShipType, int> kvp in fleet.ships)
        {
            ShipType type = kvp.Key;
            int count = kvp.Value;
            if (count <= 0) continue;

            ShipData data = ShipDatabase.Get(type);
            if (data == null) continue;

            float roleMultiplier = GetRolePowerMultiplier(data.role);
            float tierMultiplier = GetTierPowerMultiplier(data.tier);

            float basePower =
                (data.attack * 1.25f) +
                (data.defense * 1.0f) +
                (data.maxHp * 0.12f);

            int perShipPower = Mathf.RoundToInt(basePower * roleMultiplier * tierMultiplier);
            total += perShipPower * count;
        }

        return total;
    }

    private static float GetRolePowerMultiplier(ShipRole role)
    {
        switch (role)
        {
            case ShipRole.Logistics:
                return 0.08f;

            case ShipRole.Support:
                return 0.50f;

            case ShipRole.Combat:
            default:
                return 1f;
        }
    }

    private static float GetTierPowerMultiplier(ShipTier tier)
    {
        switch (tier)
        {
            case ShipTier.Light:
                return 1f;

            case ShipTier.Medium:
                return 1.15f;

            case ShipTier.Heavy:
                return 1.35f;

            case ShipTier.Capital:
                return 1.65f;

            default:
                return 1f;
        }
    }
}