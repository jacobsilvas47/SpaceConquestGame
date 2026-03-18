using UnityEngine;
using System;
using System.Collections.Generic;

public static class MissionEngine
{
    // Creates an expedition mission for an existing fleet.
    // durationSeconds is one-way travel time. Total mission time = 2x duration.
    public static string TrySendExpedition(GameState state, string fleetId, double durationSeconds)
    {
        if (state == null)
        {
            Debug.LogError("TrySendExpedition: state is null");
            return null;
        }

        if (string.IsNullOrEmpty(fleetId))
        {
            Debug.LogError("TrySendExpedition: fleetId is null/empty");
            return null;
        }

        if (durationSeconds <= 0)
        {
            Debug.LogError("TrySendExpedition: durationSeconds <= 0");
            return null;
        }

        Fleet fleet = state.GetFleet(fleetId);
        if (fleet == null)
        {
            Debug.LogError($"TrySendExpedition: fleet '{fleetId}' not found");
            return null;
        }

        if (fleet.IsEmpty())
        {
            Debug.LogError($"TrySendExpedition: fleet '{fleetId}' is empty");
            return null;
        }

        int slots = Mathf.Max(1, state.expeditionSlotsUnlocked);
        int active = GameStateQueries.GetActiveExpeditions(state, fleet.originPlanetId);

        if (active >= slots)
        {
            Debug.Log($"Cannot send expedition: slots full ({active}/{slots}).");
            return null;
        }

        int seed = new System.Random().Next(1, int.MaxValue);

        var mission = new ExpeditionMission(fleet.originPlanetId, fleetId, seed);
        mission.tier = ExpeditionRules.DetermineTier(fleet);

        int cap = fleet.TotalCargo();
        mission.cargoCapacity = cap;

        double tierDurationMultiplier = ExpeditionRules.GetDurationMultiplier(mission.tier);

        double now = state.gameTime;
        mission.departTime = now;
        mission.arriveTime = now + (durationSeconds * tierDurationMultiplier);
        mission.returnTime = now + (durationSeconds * 2.0 * tierDurationMultiplier);

        state.missions.Add(mission);

        Debug.Log($"[EXPEDITION] Created mission {mission.missionId}, fleetId={fleetId}, tier={mission.tier}, cargo={mission.cargoCapacity}");

        return mission.missionId;
    }

    public static string TrySendAttack(GameState state, string fleetId, string targetPlanetId, double durationSeconds)
    {
        if (state == null)
        {
            Debug.LogError("TrySendAttack: state is null");
            return null;
        }

        if (string.IsNullOrEmpty(fleetId))
        {
            Debug.LogError("TrySendAttack: fleetId is null/empty");
            return null;
        }

        if (string.IsNullOrEmpty(targetPlanetId))
        {
            Debug.LogError("TrySendAttack: targetPlanetId is null/empty");
            return null;
        }

        if (durationSeconds <= 0)
        {
            Debug.LogError("TrySendAttack: durationSeconds <= 0");
            return null;
        }

        Fleet fleet = state.GetFleet(fleetId);
        if (fleet == null)
        {
            Debug.LogError($"TrySendAttack: fleet '{fleetId}' not found");
            return null;
        }

        if (fleet.IsEmpty())
        {
            Debug.LogError($"TrySendAttack: fleet '{fleetId}' is empty");
            return null;
        }

        var mission = new Mission(MissionType.Attack, fleet.originPlanetId, targetPlanetId, fleetId);

        double now = state.gameTime;
        mission.departTime = now;
        mission.arriveTime = now + durationSeconds;
        mission.returnTime = now + (durationSeconds * 2.0);

        state.missions.Add(mission);

        Debug.Log($"[ATTACK] Created mission {mission.missionId}, fleetId={fleetId}, target={targetPlanetId}");

        return mission.missionId;
    }

    // Advances mission statuses based on current gameTime
    public static void UpdateMissions(GameState state)
    {
    if (state == null) return;

    double now = state.gameTime;

    for (int i = 0; i < state.missions.Count; i++)
    {
        Mission m = state.missions[i];
        if (m == null) continue;

        // Completed or failed missions do nothing
        if (m.status == MissionStatus.Completed || m.status == MissionStatus.Failed)
            continue;

        // En route → Arrived
        if (m.status == MissionStatus.EnRoute && now >= m.arriveTime)
        {
            m.status = MissionStatus.Arrived;
        }

        // Arrived → resolve based on mission type
        if (m.status == MissionStatus.Arrived)
        {
            switch (m.missionType)
            {
                case MissionType.Expedition:
                    m.status = MissionStatus.Returning;
                    break;

                case MissionType.Attack:
                    ResolveAttack(state, m);

                    Fleet attackFleet = state.GetFleet(m.fleetId);
                    if (attackFleet != null && !attackFleet.IsEmpty())
                        m.status = MissionStatus.Returning;
                    else
                        m.status = MissionStatus.Completed;

                    break;

                case MissionType.Transport:
                    m.status = MissionStatus.Returning;
                    break;

                case MissionType.Deploy:
                    m.status = MissionStatus.Returning;
                    break;

                case MissionType.Spy:
                    m.status = MissionStatus.Returning;
                    break;

                case MissionType.Harvest:
                    m.status = MissionStatus.Returning;
                    break;
            }
        }

        // Returning → Completed
        if (m.status == MissionStatus.Returning && now >= m.returnTime)
        {
            // Resolve expedition rewards once
        if (m is ExpeditionMission exp && !exp.resolved)
        {
            ExpeditionResolver.Resolve(state, exp);

            if (string.IsNullOrEmpty(exp.resultText))
                exp.resultText = exp.outcomeSummary;

            AddExpeditionLog(state, exp);
        }

            // Return fleet ships to origin planet
            FleetReturner.ReturnFleetToOrigin(state, m.fleetId);

            // Finally mark mission complete
            m.status = MissionStatus.Completed;
            }
        }
    }

    private static void ApplyBattleLosses(Fleet attacker, Fleet defender, PlanetState defenderPlanet, BattleResult result)
    {
        if (attacker != null && result.attackerLosses != null)
        {
            foreach (var loss in result.attackerLosses.lostShips)
            {
                attacker.RemoveShips(loss.Key, loss.Value);
            }
        }

        if (defender != null && result.defenderLosses != null)
        {
            foreach (var loss in result.defenderLosses.lostShips)
            {
                defender.RemoveShips(loss.Key, loss.Value);

                if (defenderPlanet != null)
                {
                    defenderPlanet.RemoveStationed(loss.Key, loss.Value);
                }
            }
        }

        // Defense structure losses can be added later
    }

    private static void ApplyPlunder(Fleet attacker, PlanetState defenderPlanet)
    {
        if (attacker == null || defenderPlanet == null)
            return;

        int cargoCapacity = attacker.TotalCargo();
        if (cargoCapacity <= 0)
            return;

        double plunderMetal = defenderPlanet.metal * 0.5;
        double plunderCrystal = defenderPlanet.crystal * 0.5;
        double plunderGas = defenderPlanet.gas * 0.5;

        double totalAvailable = plunderMetal + plunderCrystal + plunderGas;
        if (totalAvailable <= 0)
            return;

        double totalTaken = Math.Min(cargoCapacity, totalAvailable);
        double ratio = totalTaken / totalAvailable;

        int metalTaken = Mathf.FloorToInt((float)(plunderMetal * ratio));
        int crystalTaken = Mathf.FloorToInt((float)(plunderCrystal * ratio));
        int gasTaken = Mathf.FloorToInt((float)(plunderGas * ratio));

        defenderPlanet.metal -= metalTaken;
        defenderPlanet.crystal -= crystalTaken;
        defenderPlanet.gas -= gasTaken;

        attacker.cargoMetal += metalTaken;
        attacker.cargoCrystal += crystalTaken;
        attacker.cargoGas += gasTaken;

        Debug.Log($"[PLUNDER] Took {metalTaken} metal, {crystalTaken} crystal, {gasTaken} gas.");
    }

    private static void ResolveAttack(GameState state, Mission mission)
    {
        if (state == null || mission == null)
            return;

        Fleet attacker = state.GetFleet(mission.fleetId);
        if (attacker == null)
        {
            Debug.LogWarning($"ResolveAttack: attacker fleet '{mission.fleetId}' not found.");
            return;
        }

        PlanetState defenderPlanet = state.GetPlanet(mission.targetId);
        if (defenderPlanet == null)
        {
            Debug.LogWarning($"ResolveAttack: target planet '{mission.targetId}' not found.");
            return;
        }

        Fleet defenderFleet = FleetFactory.CreateStationedFleet(state, mission.targetId);

        BattleResult result = BattleResolver.Resolve(attacker, defenderFleet, defenderPlanet);

        ApplyBattleLosses(attacker, defenderFleet, defenderPlanet, result);

        if (result.attackerWon)
        {
            ApplyPlunder(attacker, defenderPlanet);
        }

        Debug.Log($"[ATTACK] Mission {mission.missionId} resolved. AttackerWon={result.attackerWon}");
    }

       private static void AddExpeditionLog(GameState state, ExpeditionMission exp)
    {
        if (state.expeditionLog == null)
            state.expeditionLog = new List<ExpeditionLogEntry>();

        string summary = exp.resultText;
        if (string.IsNullOrEmpty(summary))
            summary = exp.outcomeSummary;
        if (string.IsNullOrEmpty(summary))
            summary = "Expedition complete.";

        var entry = new ExpeditionLogEntry
        {
            timestamp = state.gameTime,
            originPlanetId = exp.originPlanetId,
            missionId = exp.missionId,

            // Keep these for later stats/UI, but don't rely on them for text
            metalGained = exp.rewardMetal,
            crystalGained = exp.rewardCrystal,
            gasGained = exp.rewardGas,

            // ✅ This is what the console should display
            summaryText = summary
        };

        if (exp.itemsFound != null && exp.itemsFound.Count > 0)
            entry.itemsGained.AddRange(exp.itemsFound);

            state.expeditionLog.Add(entry);
    }
}