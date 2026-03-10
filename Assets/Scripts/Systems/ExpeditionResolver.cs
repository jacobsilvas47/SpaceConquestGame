using UnityEngine;
using System;

public static class ExpeditionResolver
{
#if UNITY_EDITOR
    public static bool DEBUG_ForceFoundShipsOnce = false;

    public static void Debug_ForceFoundShipsNext()
    {
        DEBUG_ForceFoundShipsOnce = true;
        Debug.Log("[EXPEDITION][DEBUG] Next expedition will FORCE Found Abandoned Ships.");
    }
#endif

    // ---------------------------
    // Fleet-scaling helpers
    // ---------------------------
    private static int GetFleetValue(Fleet fleet)
    {
        if (fleet == null) return 0;

        // Simple weights, tweak later
        int probes = fleet.GetCount(ShipType.Probe);
        int sc = fleet.GetCount(ShipType.SmallCargo);
        int lc = fleet.GetCount(ShipType.LargeCargo);
        int bf = fleet.GetCount(ShipType.BasicFighter);

        // Probes = exploration, cargos = logistics, fighters = survivability
        int value =
            probes * 1 +
            sc * 6 +
            lc * 12 +
            bf * 10;

        return Mathf.Max(0, value);
    }

    // Returns a 0..1-ish bonus based on fleet strength (soft caps)
    private static float FleetBonus01(int fleetValue)
    {
        // 0 at 0 value, ramps up, then soft caps
        return 1f - Mathf.Exp(-fleetValue / 120f);
    }

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
                  $"BF={fleet?.GetCount(ShipType.BasicFighter) ?? -1}, " +
                  $"CargoCap={cap}");

        // Deterministic RNG
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

        // ---------------------------
        // Fleet scaling (odds + amounts)
        // ---------------------------
        int fleetValue = GetFleetValue(fleet);
        float bonus01 = FleetBonus01(fleetValue);

        // Reduce empties as fleets get stronger, boost jackpots/ships a bit
        int emptyCut = Mathf.RoundToInt(10 * bonus01);     // up to -10% empty bucket
        int jackpotBoost = Mathf.RoundToInt(3 * bonus01);  // up to +3% jackpot bucket
        int shipsBoost = Mathf.RoundToInt(2 * bonus01);    // up to +2% ships bucket

        // Base thresholds (sum to 100):
        // SINGLE <50, MIXED <75, EMPTY <90, JACKPOT <95, else SHIPS (95-99)
        int tSingle = 50;
        int tMixed = 75;
        int tEmpty = 90;
        int tJackpot = 95;

        // Shift probability mass away from EMPTY into MIXED/JACKPOT/SHIPS
        tEmpty -= emptyCut;          // fewer empties
        tJackpot -= jackpotBoost;    // jackpots more likely
        tSingle -= shipsBoost;       // more likely to fall into later buckets (incl ships)

        // Clamp to safe ranges
        tSingle = Mathf.Clamp(tSingle, 30, 60);
        tMixed = Mathf.Clamp(tMixed, tSingle + 10, 85);
        tEmpty = Mathf.Clamp(tEmpty, tMixed + 5, 95);
        tJackpot = Mathf.Clamp(tJackpot, tEmpty + 1, 99);

        // Amount multiplier: up to +35% for strong fleets
        float amountMult = 1f + (0.35f * bonus01);

        Debug.Log($"[EXPEDITION] roll={roll} fleetValue={fleetValue} bonus01={bonus01:0.00} " +
                  $"thresholds: single<{tSingle} mixed<{tMixed} empty<{tEmpty} jackpot<{tJackpot} ships>= {tJackpot} amtMult={amountMult:0.00}");

        // Use INTS for rewards so they match ExpeditionMission reward fields exactly
        int metalGain = 0;
        int crystalGain = 0;
        int gasGain = 0;

        mission.outcomeSummary = "";
        mission.itemsFound.Clear();

        ItemId foundItem = ItemId.None;
        int foundItemAmount = 0;

        if (roll < tSingle)
        {
            Debug.Log("[EXPEDITION] outcome=SINGLE_RESOURCE");

            int which = rng.Next(0, 3); // 0=metal, 1=crystal, 2=gas

            int min = Mathf.Max(0, Mathf.FloorToInt(cap * 0.05f));
            int max = Mathf.Max(min, Mathf.FloorToInt(cap * 0.15f));
            int amt = rng.Next(min, max + 1);

            amt = Mathf.RoundToInt(amt * amountMult);

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
        else if (roll < tMixed)
        {
            Debug.Log("[EXPEDITION] outcome=MIXED_RESOURCES");

            int mMin = Mathf.Max(0, Mathf.FloorToInt(cap * 0.04f));
            int mMax = Mathf.Max(mMin, Mathf.FloorToInt(cap * 0.10f));
            int cMin = Mathf.Max(0, Mathf.FloorToInt(cap * 0.02f));
            int cMax = Mathf.Max(cMin, Mathf.FloorToInt(cap * 0.06f));
            int gMin = Mathf.Max(0, Mathf.FloorToInt(cap * 0.01f));
            int gMax = Mathf.Max(gMin, Mathf.FloorToInt(cap * 0.04f));

            metalGain = Mathf.RoundToInt(rng.Next(mMin, mMax + 1) * amountMult);
            crystalGain = Mathf.RoundToInt(rng.Next(cMin, cMax + 1) * amountMult);
            gasGain = Mathf.RoundToInt(rng.Next(gMin, gMax + 1) * amountMult);

            mission.outcomeSummary = $"Result: +{metalGain} Metal, +{crystalGain} Crystal, +{gasGain} Gas";
        }
        else if (roll < tEmpty)
        {
            Debug.Log("[EXPEDITION] outcome=EMPTY");
            mission.outcomeSummary = "Result: Came up empty, no resources found.";
        }
        else if (roll < tJackpot)
        {
            Debug.Log("[EXPEDITION] outcome=JACKPOT");

            int mMin = Mathf.Max(0, Mathf.FloorToInt(cap * 0.25f));
            int mMax = Mathf.Max(mMin, Mathf.FloorToInt(cap * 0.50f));
            int cMin = Mathf.Max(0, Mathf.FloorToInt(cap * 0.15f));
            int cMax = Mathf.Max(cMin, Mathf.FloorToInt(cap * 0.30f));
            int gMin = Mathf.Max(0, Mathf.FloorToInt(cap * 0.10f));
            int gMax = Mathf.Max(gMin, Mathf.FloorToInt(cap * 0.20f));

            metalGain = Mathf.RoundToInt(rng.Next(mMin, mMax + 1) * amountMult);
            crystalGain = Mathf.RoundToInt(rng.Next(cMin, cMax + 1) * amountMult);
            gasGain = Mathf.RoundToInt(rng.Next(gMin, gMax + 1) * amountMult);

            mission.outcomeSummary = $"Result: JACKPOT, +{metalGain} Metal, +{crystalGain} Crystal, +{gasGain} Gas";
        }
        else
        {
            Debug.Log("[EXPEDITION] outcome=FOUND_SHIPS");

            // Make ships a bit more likely to show something on stronger fleets
            float shipMult = 1f + (0.50f * bonus01); // up to +50% ship finds for strong fleets

            int probesFound = Mathf.RoundToInt(rng.Next(0, Math.Max(1, cap / 2500) + 1) * shipMult);
            int smallCargoFound = Mathf.RoundToInt(rng.Next(0, Math.Max(1, cap / 6000) + 1) * shipMult);
            int largeCargoFound = Mathf.RoundToInt(rng.Next(0, Math.Max(1, cap / 12000) + 1) * shipMult);
            int basicFightersFound = Mathf.RoundToInt(rng.Next(0, Math.Max(1, cap / 5000) + 1) * shipMult);

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

            mission.outcomeSummary = $"Result: {parts}";

            // Ensure resource gains remain 0 for ship-find outcome
            metalGain = 0;
            crystalGain = 0;
            gasGain = 0;
        }

        // ---- Store rewards ONCE ----
        mission.rewardMetal = metalGain;
        mission.rewardCrystal = crystalGain;
        mission.rewardGas = gasGain;

        // ---- Apply resources to planet ONCE ----
        planet.metal += metalGain;
        planet.crystal += crystalGain;
        planet.gas += gasGain;

       // ---- Optional item reward roll ----
        if (ExpeditionItemLoot.TryRollItemReward(rng, out foundItem, out foundItemAmount))
    {
        ItemRarity foundRarity = ExpeditionItemLoot.RollRarity(rng, bonus01);

        state.inventory.Add(foundItem, foundRarity, foundItemAmount);

        mission.itemsFound.Clear();
        mission.itemsFound.Add(new ExpeditionItemReward(foundItem, foundItemAmount));

        string itemText = $"Found item: {ItemDatabase.GetDisplayName(foundItem, foundRarity)} x{foundItemAmount}";

        if (!string.IsNullOrEmpty(mission.outcomeSummary))
            mission.outcomeSummary += $"\n{itemText}";
        else
            mission.outcomeSummary = itemText;

        Debug.Log($"[EXPEDITION] {itemText}");
    }

        // ---- Set final UI/log text ONCE ----
        mission.resultText = mission.outcomeSummary;

        // --- Add entry to expedition log ---
        var entry = new ExpeditionLogEntry();
        entry.timestamp = state.gameTime;
        entry.originPlanetId = mission.originPlanetId;
        entry.missionId = mission.missionId;

        entry.metalGained = metalGain;
        entry.crystalGained = crystalGain;
        entry.gasGained = gasGain;

        if (foundItem != ItemId.None && foundItemAmount > 0)
        {
            entry.itemsGained.Add(new ExpeditionItemReward(foundItem, foundItemAmount));
        }

        entry.summaryText = mission.outcomeSummary;

        state.expeditionLog.Add(entry);

        mission.resolved = true;

        string logLine = $"{mission.resultText} | Totals: M={planet.metal:F0} C={planet.crystal:F0} G={planet.gas:F0}";
        Debug.Log($"[EXPEDITION] {logLine}");
    }
}