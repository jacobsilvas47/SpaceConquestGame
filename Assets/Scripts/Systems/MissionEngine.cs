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

    int seed = new System.Random().Next(1, int.MaxValue);

    var mission = new ExpeditionMission(fleet.originPlanetId, fleetId, seed);

    int cap = fleet.TotalCargoCapacity();
    
    double now = state.gameTime;
    mission.departTime = now;
    mission.arriveTime = now + durationSeconds;
    mission.returnTime = now + (durationSeconds * 2.0);

    state.missions.Add(mission);

    Debug.Log($"TrySendExpedition SUCCESS: mission {mission.missionId} created");

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
                exp.resolved = true;

                // Feed UI result text
                if (string.IsNullOrEmpty(exp.resultText))
                    exp.resultText = exp.outcomeSummary;
            }

            // Return fleet ships to origin planet
            FleetReturner.ReturnFleetToOrigin(state, m.fleetId);

            // Finally mark mission complete
            m.status = MissionStatus.Completed;
            }
        }
    }
}