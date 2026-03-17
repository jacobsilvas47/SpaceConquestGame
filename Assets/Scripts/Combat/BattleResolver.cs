using UnityEngine;
using System.Collections.Generic;

public static class BattleResolver
{
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

    private static void ApplyLosses(Fleet fleet, float lossPercent, BattleLosses losses)
    {
        if (fleet == null) return;

        List<ShipType> shipTypes = new List<ShipType>(fleet.ships.Keys);

        foreach (ShipType type in shipTypes)
        {
            int current = fleet.GetCount(type);
            if (current <= 0) continue;

            int lost = Mathf.CeilToInt(current * lossPercent);
            if (lost > current) lost = current;

            if (lost > 0)
            {
                fleet.RemoveShips(type, lost);
                losses.AddLoss(type, lost);
            }
        }
    }
}