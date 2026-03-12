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

        // Arrived → Returning (simple instant turnaround model)
        if (m.status == MissionStatus.Arrived)
        {
            m.status = MissionStatus.Returning;
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
    }
}