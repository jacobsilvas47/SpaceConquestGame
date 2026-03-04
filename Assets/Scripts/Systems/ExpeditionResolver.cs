using UnityEngine;
using System;

public static class ExpeditionResolver
{
    // Very simple loot table for now.
    // Later we’ll scale with fleet size, tech, risk, distance, etc.
    public static void Resolve(GameState state, ExpeditionMission mission)
    {
        if (state == null || mission == null) return;
        if (mission.resolved) return;

        var planet = state.GetPlanet(mission.originPlanetId);
        if (planet == null) return;

        var fleet = state.GetFleet(mission.fleetId);
        int cap = (fleet != null) ? fleet.TotalCargoCapacity() : 0;
        if (cap <= 0) cap = 50;

        Debug.Log($"[EXPEDITION] FleetId={mission.fleetId} Ships: " +
                  $"Probe={fleet?.GetCount(ShipType.Probe) ?? -1}, " +
                  $"SC={fleet?.GetCount(ShipType.SmallCargo) ?? -1}, " +
                  $"LC={fleet?.GetCount(ShipType.LargeCargo) ?? -1}, " +
                  $"CargoCap={cap}");

        // Deterministic: use the mission seed so you can reproduce results later
        var rng = new System.Random(mission.seed);

        // Roll 0–99
        int roll = rng.Next(0, 100);

        double metalGain = 0;
        double crystalGain = 0;
        double gasGain = 0;

        if (roll < 50)
        {
            metalGain = rng.Next((int)(cap * 0.05), (int)(cap * 0.15) + 1);
            mission.outcomeSummary = $"Found scrap metal: +{metalGain:F0} Metal";
        }
        else if (roll < 75)
        {
            metalGain   = rng.Next((int)(cap * 0.04), (int)(cap * 0.10) + 1);
            crystalGain = rng.Next((int)(cap * 0.02), (int)(cap * 0.06) + 1);
            gasGain     = rng.Next((int)(cap * 0.01), (int)(cap * 0.04) + 1);
            mission.outcomeSummary = $"+{metalGain:F0} Metal, +{crystalGain:F0} Crystal, +{gasGain:F0} Gas";
        }
        else if (roll < 90)
        {
            mission.outcomeSummary = "Came up empty, no resources found.";
        }
        else
        {
            metalGain   = rng.Next((int)(cap * 0.25), (int)(cap * 0.50) + 1);
            crystalGain = rng.Next((int)(cap * 0.15), (int)(cap * 0.30) + 1);
            gasGain     = rng.Next((int)(cap * 0.10), (int)(cap * 0.20) + 1);
            mission.outcomeSummary = $"JACKPOT, +{metalGain:F0} Metal, +{crystalGain:F0} Crystal, +{gasGain:F0} Gas";
        }

        planet.metal += metalGain;
        planet.crystal += crystalGain;
        planet.gas += gasGain;

        // If your UI reads resultText, set it too:
        mission.resultText = mission.outcomeSummary;

        mission.resolved = true;
    }
}