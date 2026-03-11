using UnityEngine;

public static class ExpeditionRules
{
    public static ExpeditionTier DetermineTier(Fleet fleet)
    {
        if (fleet == null)
            return ExpeditionTier.Tier1;

        // Temporary placeholder logic
        // We will replace this later with proper fleet composition rules.
        int fleetValue = GetFleetValue(fleet);

        if (fleetValue < 10) return ExpeditionTier.Tier1;
        if (fleetValue < 25) return ExpeditionTier.Tier2;
        if (fleetValue < 50) return ExpeditionTier.Tier3;
        if (fleetValue < 100) return ExpeditionTier.Tier4;

        return ExpeditionTier.Tier5;
    }

    public static ItemRarity GetMaxRarityForTier(ExpeditionTier tier)
    {
        switch (tier)
        {
            case ExpeditionTier.Tier1: return ItemRarity.Common;
            case ExpeditionTier.Tier2: return ItemRarity.Uncommon;
            case ExpeditionTier.Tier3: return ItemRarity.Rare;
            case ExpeditionTier.Tier4: return ItemRarity.Epic;
            case ExpeditionTier.Tier5: return ItemRarity.Legendary;
            default: return ItemRarity.Common;
        }
    }

    public static float GetDurationMultiplier(ExpeditionTier tier)
    {
        switch (tier)
        {
            case ExpeditionTier.Tier1: return 1f;
            case ExpeditionTier.Tier2: return 1.25f;
            case ExpeditionTier.Tier3: return 1.75f;
            case ExpeditionTier.Tier4: return 2.5f;
            case ExpeditionTier.Tier5: return 4f;
            default: return 1f;
        }
    }

    public static float GetDangerMultiplier(ExpeditionTier tier)
    {
        switch (tier)
        {
            case ExpeditionTier.Tier1: return 0.5f;
            case ExpeditionTier.Tier2: return 0.8f;
            case ExpeditionTier.Tier3: return 1.2f;
            case ExpeditionTier.Tier4: return 1.7f;
            case ExpeditionTier.Tier5: return 2.5f;
            default: return 1f;
        }
    }

    private static int GetFleetValue(Fleet fleet)
    {
        int value = 0;

        value += fleet.GetCount(ShipType.Probe) * 1;
        value += fleet.GetCount(ShipType.SmallCargo) * 3;
        value += fleet.GetCount(ShipType.LargeCargo) * 5;
        value += fleet.GetCount(ShipType.BasicFighter) * 4;
        /*
        value += fleet.GetCount(ShipType.HeavyFighter) * 8;
        value += fleet.GetCount(ShipType.Cruiser) * 12;
        value += fleet.GetCount(ShipType.Battleship) * 20;
        */

        return value;
    }
}