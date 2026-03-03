using UnityEngine;
using System;
using System.Collections.Generic;

public static class MissionEngine
{
    // Creates an expedition mission for an existing fleet.
    // durationSeconds is one-way travel time. Total mission time = 2x duration.
    public static string TrySendExpedition(GameState state, string fleetId, double durationSeconds)
    {
        if (state == null) return null;
        if (string.IsNullOrEmpty(fleetId)) return null;
        if (durationSeconds <= 0) return null;

        Fleet fleet = state.GetFleet(fleetId);
        if (fleet == null) return null;

        // Optional sanity check: must have at least 1 ship
        if (fleet.IsEmpty()) return null;

        int seed = new System.Random().Next(1, int.MaxValue);

        var mission = new ExpeditionMission(fleet.originPlanetId, fleetId, seed);

        double now = state.gameTime;
        mission.departTime = now;
        mission.arriveTime = now + durationSeconds;
        mission.returnTime = now + (durationSeconds * 2.0);

        state.missions.Add(mission);
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

            // Completed missions don't change
            if (m.status == MissionStatus.Completed || m.status == MissionStatus.Failed)
                continue;

            if (m.status == MissionStatus.EnRoute && now >= m.arriveTime)
            {
                m.status = MissionStatus.Arrived;
            }

            // After arriving, we flip to Returning immediately (simple model for now)
            if (m.status == MissionStatus.Arrived)
            {
                m.status = MissionStatus.Returning;
            }

           if (m.status == MissionStatus.Returning && now >= m.returnTime)
            {
            m.status = MissionStatus.Completed;

            // Return the fleet to the origin planet
            FleetReturner.ReturnFleetToOrigin(state, m.fleetId);
            }
        }
    }
}