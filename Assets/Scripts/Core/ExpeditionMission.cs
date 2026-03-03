using UnityEngine;
using System;

[Serializable]
public class ExpeditionMission : Mission
{
    public int seed;                 // used later so outcomes are reproducible if needed
    public bool resolved;            // did we roll the outcome yet
    public string outcomeSummary;    // debug string for now
    public int cargoCapacity;
    public string resultText = "";
    public bool resultShown = false;


    public ExpeditionMission(string originPlanetId, string fleetId, int seed)
        : base(MissionType.Expedition, originPlanetId, targetPlanetId: "deep_space", fleetId: fleetId)
    {
        this.seed = seed;
        resolved = false;
        outcomeSummary = "";
    }
}