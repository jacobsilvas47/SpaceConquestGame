using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ExpeditionMission : Mission
{
    public int rewardMetal;
    public int rewardCrystal;
    public int rewardGas;
    public int seed;                 // used later so outcomes are reproducible if needed
    public bool resolved;            // did we roll the outcome yet
    public string outcomeSummary;    // debug string for now
    public int cargoCapacity;
    public string resultText = "";
    public bool resultShown = false;
    public List<ExpeditionItemReward> itemsFound = new List<ExpeditionItemReward>();


    public ExpeditionMission(string originPlanetId, string fleetId, int seed)
        : base(MissionType.Expedition, originPlanetId, targetPlanetId: "deep_space", fleetId: fleetId)
    {
        this.seed = seed;
        resolved = false;
        outcomeSummary = "";
    }
}