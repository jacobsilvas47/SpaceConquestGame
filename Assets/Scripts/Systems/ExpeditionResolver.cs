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

        // Use INTS for rewards so they match ExpeditionMission reward fields exactly
        int metalGain = 0;
        int crystalGain = 0;
        int gasGain = 0;

        // Clear outcome text just in case
        mission.outcomeSummary = "";

        if (roll < 50)
        {
            Debug.Log("[EXPEDITION] outcome=SINGLE_RESOURCE");

            int which = rng.Next(0, 3); // 0=metal, 1=crystal, 2=gas

            // Guard: ensure max >= min
            int min = Mathf.Max(0, Mathf.FloorToInt(cap * 0.05f));
            int max = Mathf.Max(min, Mathf.FloorToInt(cap * 0.15f));
            int amt = rng.Next(min, max + 1);

            if (which == 0)
            {
                metalGain = amt;
                mission.outcomeSummary = $"Found: +{metalGain} Metal";
            }
            else if (which == 1)
            {
                crystalGain = amt;
                mission.outcomeSummary = $"Found: +{crystalGain} Crystal";
            }
            else
            {
                gasGain = amt;
                mission.outcomeSummary = $"Found: +{gasGain} Gas";
            }
        }
        else if (roll < 75)
        {
            Debug.Log("[EXPEDITION] outcome=MIXED_RESOURCES");

            int mMin = Mathf.Max(0, Mathf.FloorToInt(cap * 0.04f));
            int mMax = Mathf.Max(mMin, Mathf.FloorToInt(cap * 0.10f));
            int cMin = Mathf.Max(0, Mathf.FloorToInt(cap * 0.02f));
            int cMax = Mathf.Max(cMin, Mathf.FloorToInt(cap * 0.06f));
            int gMin = Mathf.Max(0, Mathf.FloorToInt(cap * 0.01f));
            int gMax = Mathf.Max(gMin, Mathf.FloorToInt(cap * 0.04f));

            metalGain = rng.Next(mMin, mMax + 1);
            crystalGain = rng.Next(cMin, cMax + 1);
            gasGain = rng.Next(gMin, gMax + 1);

            mission.outcomeSummary = $"Result: +{metalGain} Metal, +{crystalGain} Crystal, +{gasGain} Gas";
        }
        else if (roll < 90)
        {
            Debug.Log("[EXPEDITION] outcome=EMPTY");
            mission.outcomeSummary = "Result: Came up empty, no resources found.";
        }
        else if (roll < 95)
        {
            Debug.Log("[EXPEDITION] outcome=JACKPOT");

            int mMin = Mathf.Max(0, Mathf.FloorToInt(cap * 0.25f));
            int mMax = Mathf.Max(mMin, Mathf.FloorToInt(cap * 0.50f));
            int cMin = Mathf.Max(0, Mathf.FloorToInt(cap * 0.15f));
            int cMax = Mathf.Max(cMin, Mathf.FloorToInt(cap * 0.30f));
            int gMin = Mathf.Max(0, Mathf.FloorToInt(cap * 0.10f));
            int gMax = Mathf.Max(gMin, Mathf.FloorToInt(cap * 0.20f));

            metalGain = rng.Next(mMin, mMax + 1);
            crystalGain = rng.Next(cMin, cMax + 1);
            gasGain = rng.Next(gMin, gMax + 1);

            mission.outcomeSummary = $"Result: JACKPOT, +{metalGain} Metal, +{crystalGain} Crystal, +{gasGain} Gas";
        }
        else
        {
            Debug.Log("[EXPEDITION] outcome=FOUND_SHIPS");

            // Found abandoned ships (not resource rewards)
            int probesFound = rng.Next(0, Math.Max(1, cap / 2500) + 1);
            int smallCargoFound = rng.Next(0, Math.Max(1, cap / 6000) + 1);
            int largeCargoFound = rng.Next(0, Math.Max(1, cap / 12000) + 1);
            int basicFightersFound = rng.Next(0, Math.Max(1, cap / 5000) + 1);

            if (probesFound + smallCargoFound + largeCargoFound + basicFightersFound == 0)
        {
            int pick = rng.Next(0, 4);
            if (pick == 0) probesFound = 1;
            else if (pick == 1) smallCargoFound = 1;
            else if (pick == 2) largeCargoFound = 1;
            else basicFightersFound = 1;
        }

            if (probesFound > 0) planet.AddStationed(ShipType.Probe, probesFound);
            if (smallCargoFound > 0) planet.AddStationed(ShipType.SmallCargo, smallCargoFound);
            if (largeCargoFound > 0) planet.AddStationed(ShipType.LargeCargo, largeCargoFound);
            if (basicFightersFound > 0) planet.AddStationed(ShipType.BasicFighter, basicFightersFound);

            string parts = "";

            if (probesFound > 0)
                parts += $"+{probesFound} Probes";

            if (smallCargoFound > 0)
                parts += (parts.Length > 0 ? ", " : "") + $"+{smallCargoFound} Small Cargo";

            if (largeCargoFound > 0)
                parts += (parts.Length > 0 ? ", " : "") + $"+{largeCargoFound} Large Cargo";

            if (basicFightersFound > 0)
                parts += (parts.Length > 0 ? ", " : "") + $"+{basicFightersFound} Basic Fighters";

            // ✅ Short, consistent console text:
            mission.outcomeSummary = $"Result: {parts}";
            mission.resultText = mission.outcomeSummary;

            // Ensure resource gains remain 0 for ship-find outcome
            metalGain = 0;
            crystalGain = 0;
            gasGain = 0;
        }

        // ---- Store rewards ONCE (ints match the mission fields exactly) ----
        mission.rewardMetal = metalGain;
        mission.rewardCrystal = crystalGain;
        mission.rewardGas = gasGain;

        // ---- Apply to planet ONCE ----
        planet.metal += metalGain;
        planet.crystal += crystalGain;
        planet.gas += gasGain;

        // ---- Set final UI/log text ONCE ----
        mission.resultText = mission.outcomeSummary;
        mission.resolved = true;

        Debug.Log($"[EXPEDITION] {mission.resultText} | Totals: M={planet.metal:F0} C={planet.crystal:F0} G={planet.gas:F0}");
    }
}