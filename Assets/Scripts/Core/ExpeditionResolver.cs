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

        // Deterministic-ish: use the mission seed so you can reproduce results later if you want
        var rng = new System.Random(mission.seed);

        // Roll 0–99
        int roll = rng.Next(0, 100);

        // Outcome buckets:
        // 0-49  (50%) small metal
        // 50-74 (25%) mixed
        // 75-89 (15%) nothing
        // 90-99 (10%) jackpot
        double metalGain = 0;
        double crystalGain = 0;
        double gasGain = 0;

        if (roll < 50)
        {
            metalGain = rng.Next(10, 31); // 10-30
            mission.outcomeSummary = $"Found scrap metal: +{metalGain} Metal";
        }
        else if (roll < 75)
        {
            metalGain = rng.Next(8, 21);    // 8-20
            crystalGain = rng.Next(4, 13);  // 4-12
            gasGain = rng.Next(2, 9);       // 2-8
            mission.outcomeSummary = $"+{metalGain} Metal, +{crystalGain} Crystal, +{gasGain} Gas";
        }
        else if (roll < 90)
        {
            mission.outcomeSummary = "Came up empty, no resources found.";
        }
        else
        {
            metalGain = rng.Next(40, 81);    // 40-80
            crystalGain = rng.Next(20, 51);  // 20-50
            gasGain = rng.Next(10, 31);      // 10-30
            mission.outcomeSummary = $"JACKPOT, +{metalGain} Metal, +{crystalGain} Crystal, +{gasGain} Gas";
        }

        // Apply gains
        planet.metal += metalGain;
        planet.crystal += crystalGain;
        planet.gas += gasGain;

        mission.resolved = true;
    }
}