using UnityEngine;
using System.Collections.Generic;

public static class BattleResolver
{
    public static float GetTierDamageMultiplier(ShipTier attackerTier, ShipTier defenderTier)
    {
        int gap = (int)defenderTier - (int)attackerTier;

        if (gap <= 0) return 1f;
        if (gap == 1) return 0.5f;
        if (gap == 2) return 0.2f;
        return 0.05f;
    }
    public static BattleResult Resolve(Fleet attacker, Fleet defender)
    {
        return Resolve(attacker, defender, null);
    }

    public static BattleResult Resolve(Fleet attacker, Fleet defender, PlanetState defenderPlanet)
    {
        BattleResult result = new BattleResult();

        if (attacker == null || defender == null)
        {
            Debug.LogWarning("BattleResolver.Resolve called with null fleet.");
            return result;
        }

        int defenseAttack = 0;
        int defenseHP = 0;
        int defenseShield = 0;

        if (defenderPlanet != null)
        {
            defenseAttack = defenderPlanet.GetTotalDefenseAttack();
            defenseHP = defenderPlanet.GetTotalDefenseHP();
            defenseShield = defenderPlanet.GetTotalDefenseShield();
        }

        int attackerPower = attacker.CombatPowerScore();

        int defenderFleetPower = defender.CombatPowerScore();
        int defenderDefensePower = defenseAttack + defenseHP + defenseShield;
        int defenderPower = defenderFleetPower + defenderDefensePower;

        result.attackerPower = attackerPower;
        result.defenderPower = defenderPower;

        result.attackerWon = attackerPower >= defenderPower;

        float ratio;

        if (result.attackerWon)
        {
            ratio = defenderPower <= 0 ? 999f : (float)attackerPower / defenderPower;

            result.attackerLossPercent = CalculateWinnerLossPercent(ratio);
            result.defenderLossPercent = CalculateLoserLossPercent(ratio);
        }
        else
        {
            ratio = attackerPower <= 0 ? 999f : (float)defenderPower / attackerPower;

            result.attackerLossPercent = CalculateLoserLossPercent(ratio);
            result.defenderLossPercent = CalculateWinnerLossPercent(ratio);
        }

        ApplyLosses(attacker, result.attackerLossPercent, result.attackerLosses);
        ApplyLosses(defender, result.defenderLossPercent, result.defenderLosses);

        if (defenderPlanet != null)
        {
            float defenseLossPercent = Mathf.Clamp01(result.defenderLossPercent * 0.65f);
            defenderPlanet.ApplyDefenseLossesByPercent(defenseLossPercent);
        }

        return result;
    }

    private static float CalculateWinnerLossPercent(float ratio)
    {
        if (ratio >= 3f) return 0.10f;
        if (ratio >= 2f) return 0.20f;
        if (ratio >= 1.5f) return 0.30f;
        return 0.40f;
    }

    private static float CalculateLoserLossPercent(float ratio)
    {
        if (ratio >= 3f) return 1.00f;
        if (ratio >= 2f) return 0.85f;
        if (ratio >= 1.5f) return 0.70f;
        return 0.55f;
    }

    private static float GetLossVulnerabilityMultiplier(ShipData data)
    {
        if (data == null) return 1f;

        float roleMultiplier = GetRoleLossMultiplier(data.role);
        float tierMultiplier = GetTierLossMultiplier(data.tier);

        return roleMultiplier * tierMultiplier;
    }

    private static float GetRoleLossMultiplier(ShipRole role)
    {
        switch (role)
        {
            case ShipRole.Logistics:
                return 1.6f;   // logistics die more easily

            case ShipRole.Support:
                return 0.9f;   // support slightly safer than average

            case ShipRole.Combat:
            default:
                return 1f;
        }
    }

    private static float GetTierLossMultiplier(ShipTier tier)
    {
        switch (tier)
        {
            case ShipTier.Light:
                return 1.25f;  // light ships die easier

            case ShipTier.Medium:
                return 1f;

            case ShipTier.Heavy:
                return 0.55f;  // heavy ships resist losses

            case ShipTier.Capital:
                return 0.25f;  // capital ships are very hard to lose

            default:
                return 1f;
        }
    }

    private static void ApplyLosses(Fleet fleet, float lossPercent, BattleLosses losses)
    {
        if (fleet == null || fleet.ships == null) return;

        List<ShipType> shipTypes = new List<ShipType>(fleet.ships.Keys);

        foreach (ShipType type in shipTypes)
        {
            int current = fleet.GetCount(type);
            if (current <= 0) continue;

            ShipData data = ShipDatabase.Get(type);
            if (data == null) continue;

            float adjustedLossPercent = lossPercent * GetLossVulnerabilityMultiplier(data);

            int lost = Mathf.FloorToInt(current * adjustedLossPercent);

            // Only force a minimum loss for fragile ship classes when the loss percent is meaningful
            if (lost == 0 && adjustedLossPercent >= 0.5f && current > 0)
            {
                if (data.role == ShipRole.Logistics || data.tier == ShipTier.Light)
                    lost = 1;
            }

            if (lost > current)
                lost = current;

            if (lost > 0)
            {
                fleet.RemoveShips(type, lost);
                losses.AddLoss(type, lost);
            }
        }
    }
}