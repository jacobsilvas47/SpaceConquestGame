using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class GameState
{
    [Header("Expeditions")]
    public int expeditionSlotsUnlocked = 1;
    public double gameTime; // your “clock”, you can increment from a MonoBehaviour

    public Dictionary<string, PlanetState> planets = new Dictionary<string, PlanetState>();
    public Dictionary<string, Fleet> fleets = new Dictionary<string, Fleet>();
    public Dictionary<string, AttackTarget> aiTargets = new Dictionary<string, AttackTarget>();

    // keep missions as a list so order is stable
    public List<ExpeditionLogEntry> expeditionLog = new List<ExpeditionLogEntry>();
    public List<Mission> missions = new List<Mission>();

    public List<MissionReport> missionReports = new List<MissionReport>();

    public void AddMissionReport(MissionReport report)
    {
        if (report == null) return;

        if (missionReports == null)
            missionReports = new List<MissionReport>();

        missionReports.Add(report);
    }

    public List<MissionReport> GetMissionReports()
    {
        if (missionReports == null)
            missionReports = new List<MissionReport>();

        return missionReports;
    }

    // Inventory
    public InventoryState inventory = new InventoryState();

    public PlanetState GetPlanet(string planetId)
    {
        planets.TryGetValue(planetId, out var p);
        return p;
    }

    public Fleet GetFleet(string fleetId)
    {
        fleets.TryGetValue(fleetId, out var f);
        return f;
    }

    public AttackTarget GetAITarget(string targetId)
    {
        if (string.IsNullOrEmpty(targetId)) return null;
        return aiTargets.TryGetValue(targetId, out var target) ? target : null;
    }
}