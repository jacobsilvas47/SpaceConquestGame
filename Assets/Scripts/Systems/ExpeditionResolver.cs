using UnityEngine;
using System;

public static class ExpeditionResolver
{
    // Very simple loot table for now.
    // Later we’ll scale with fleet size, tech, risk, distance, etc.

#if UNITY_EDITOR
    public static bool DEBUG_ForceFoundShipsOnce = false;

    // Call this from a debug button to force the NEXT expedition to roll "found ships"
    public static void Debug_ForceFoundShipsNext()
    {
        DEBUG_ForceFoundShipsOnce = true;
        Debug.Log("[EXPEDITION][DEBUG] Next expedition will FORCE Found Abandoned Ships.");
    }
#endif

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

#if UNITY_EDITOR
        if (DEBUG_ForceFoundShipsOnce)
        {
            roll = 99; // force "found ships"
            DEBUG_ForceFoundShipsOnce = false;
        }
#endif

        Debug.Log($"[EXPEDITION] roll={roll}");

        double metalGain = 0;
        double crystalGain = 0;
        double gasGain = 0;

        if (roll < 50)
        {
            Debug.Log("[EXPEDITION] outcome=SINGLE_RESOURCE");

            int which = rng.Next(0, 3); // 0=metal, 1=crystal, 2=gas
            double amt = rng.Next((int)(cap * 0.05), (int)(cap * 0.15) + 1);

            if (which == 0)
            {
                metalGain = amt;
                mission.outcomeSummary = $"Found scrap metal: +{metalGain:F0} Metal";
            }
            else if (which == 1)
            {
                crystalGain = amt;
                mission.outcomeSummary = $"Found crystal shards: +{crystalGain:F0} Crystal";
            }
            else
            {
                gasGain = amt;
                mission.outcomeSummary = $"Found compressed gas: +{gasGain:F0} Gas";
            }
        }
        else if (roll < 75)
        {
            Debug.Log("[EXPEDITION] outcome=MIXED_RESOURCES");

            metalGain = rng.Next((int)(cap * 0.04), (int)(cap * 0.10) + 1);
            crystalGain = rng.Next((int)(cap * 0.02), (int)(cap * 0.06) + 1);
            gasGain = rng.Next((int)(cap * 0.01), (int)(cap * 0.04) + 1);
            mission.outcomeSummary = $"+{metalGain:F0} Metal, +{crystalGain:F0} Crystal, +{gasGain:F0} Gas";
        }
        else if (roll < 90)
        {
            Debug.Log("[EXPEDITION] outcome=EMPTY");
            mission.outcomeSummary = "Came up empty, no resources found.";
        }
        else if (roll < 95)
        {
            Debug.Log("[EXPEDITION] outcome=JACKPOT");

            metalGain = rng.Next((int)(cap * 0.25), (int)(cap * 0.50) + 1);
            crystalGain = rng.Next((int)(cap * 0.15), (int)(cap * 0.30) + 1);
            gasGain = rng.Next((int)(cap * 0.10), (int)(cap * 0.20) + 1);
            mission.outcomeSummary = $"JACKPOT, +{metalGain:F0} Metal, +{crystalGain:F0} Crystal, +{gasGain:F0} Gas";
        }
        else
        {
            Debug.Log("[EXPEDITION] outcome=FOUND_SHIPS");

            // ⭐ Found abandoned ships
            // Guarantee at least 1 ship total so the event is obvious.
            int probesFound = rng.Next(0, Math.Max(1, cap / 2500) + 1);
            int smallCargoFound = rng.Next(0, Math.Max(1, cap / 6000) + 1);
            int basicFightersFound = rng.Next(0, Math.Max(1, cap / 5000) + 1);

            if (probesFound + smallCargoFound + basicFightersFound == 0)
            {
                // Force one ship type to be at least 1
                int pick = rng.Next(0, 3);
                if (pick == 0) probesFound = 1;
                else if (pick == 1) smallCargoFound = 1;
                else basicFightersFound = 1;
            }

            if (probesFound > 0)
                planet.AddStationed(ShipType.Probe, probesFound);

            if (smallCargoFound > 0)
                planet.AddStationed(ShipType.SmallCargo, smallCargoFound);

            if (basicFightersFound > 0)
                planet.AddStationed(ShipType.BasicFighter, basicFightersFound);

            string parts = "";

            if (probesFound > 0)
                parts += $"+{probesFound} Probes";

            if (smallCargoFound > 0)
                parts += (parts.Length > 0 ? ", " : "") + $"+{smallCargoFound} Small Cargo";

            if (basicFightersFound > 0)
                parts += (parts.Length > 0 ? ", " : "") + $"+{basicFightersFound} Basic Fighters";

            mission.outcomeSummary = $"Found abandoned ships: {parts}";
        }

        planet.metal += metalGain;
        planet.crystal += crystalGain;
        planet.gas += gasGain;

        mission.resultText = mission.outcomeSummary;
        mission.resolved = true;
    }
}