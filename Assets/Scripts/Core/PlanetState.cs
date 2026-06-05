using UnityEngine;
using System;
using System.Collections.Generic;

    [Serializable]
    public class ShipQueueItem
    {
        public ShipType shipType;
        public double finishTime;
    }

    [Serializable]
    public class PlanetState
{
    public string planetId;

    // resources on this planet (later you can move these out of ResourceManager)
    public double metal;
    public double crystal;
    public double gas;
   

    // Building levels
    public int metalRefineryLevel = 0;
    public int crystalMineLevel = 0;
    public int gasExtractorLevel = 0;
    public int orbitalShipworksLevel;

    // Dictionaries
    public Dictionary<ShipType, int> stationedShips = new Dictionary<ShipType, int>();
    public Dictionary<DefenseType, int> defenses = new();
    public Dictionary<DefenseType, int> ApplyDefenseLossesByPercent(float lossPercent)
    {
        Dictionary<DefenseType, int> losses = new Dictionary<DefenseType, int>();

        if (defenses == null || defenses.Count == 0)
            return losses;

        List<DefenseType> keys = new List<DefenseType>(defenses.Keys);

        foreach (DefenseType type in keys)
        {
            int current = GetDefenseCount(type);
            if (current <= 0) continue;

            int lost = UnityEngine.Mathf.CeilToInt(current * lossPercent);
            if (lost > current) lost = current;

            if (lost > 0)
            {
                RemoveDefense(type, lost);
                losses[type] = lost;
            }
        }

        return losses;
    }

    // Ship Queue
    public List<ShipQueueItem> shipQueue = new List<ShipQueueItem>();

    // Buildings
    public BuildingUpgradeJob activeBuildingUpgrade;
    public List<BuildingUpgradeJob> buildingUpgradeQueue = new List<BuildingUpgradeJob>();

    // Defenses
    public List<DefenseBuildJob> defenseBuildQueue = new List<DefenseBuildJob>();

    public PlanetState(string planetId)
    {
        this.planetId = planetId;
    }

    public int GetStationed(ShipType type)
    {
        return stationedShips.TryGetValue(type, out int count) ? count : 0;
    }

    public void AddStationed(ShipType type, int amount)
    {
        if (amount <= 0) return;

        if (!stationedShips.ContainsKey(type)) stationedShips[type] = 0;
        stationedShips[type] += amount;
    }

    public bool RemoveStationed(ShipType type, int amount)
    {
        if (amount <= 0) return true;

        int current = GetStationed(type);
        if (current < amount) return false;

        stationedShips[type] = current - amount;
        if (stationedShips[type] <= 0) stationedShips.Remove(type);

        return true;
    }

    public int GetBuildingLevel(BuildingType type)
    {
        switch (type)
        {
            case BuildingType.MetalRefinery:
                return metalRefineryLevel;

            case BuildingType.CrystalMine:
                return crystalMineLevel;

            case BuildingType.GasExtractor:
                return gasExtractorLevel;

            case BuildingType.OrbitalShipworks:
                return orbitalShipworksLevel;
        }

        return 0;
    }

    // Defense Structures
    public int GetDefenseCount(DefenseType type)
    {
        if (defenses == null)
            defenses = new Dictionary<DefenseType, int>();

        return defenses.TryGetValue(type, out int count) ? count : 0;
    }

    public void AddDefense(DefenseType type, int amount)
    {
        if (defenses == null)
            defenses = new Dictionary<DefenseType, int>();

        if (!defenses.ContainsKey(type))
            defenses[type] = 0;

        defenses[type] += amount;
    }

    public void RemoveDefense(DefenseType type, int amount)
    {
        if (defenses == null)
            defenses = new Dictionary<DefenseType, int>();

        if (!defenses.ContainsKey(type))
            return;

        defenses[type] -= amount;

        if (defenses[type] <= 0)
            defenses.Remove(type);
    }

    public int GetTotalDefenseAttack()
    {
        if (defenses == null) return 0;

        int total = 0;

        foreach (var kvp in defenses)
        {
            var data = DefenseDatabase.Get(kvp.Key);
            if (data == null) continue;

            total += data.attack * kvp.Value;
        }

        return total;
    }

    public int GetTotalDefenseHP()
    {
        if (defenses == null) return 0;

        int total = 0;

        foreach (var kvp in defenses)
        {
            var data = DefenseDatabase.Get(kvp.Key);
            if (data == null) continue;

            total += data.hp * kvp.Value;
        }

        return total;
    }

    public int GetTotalDefenseShield()
    {
        if (defenses == null) return 0;

        int total = 0;

        foreach (var kvp in defenses)
        {
            var data = DefenseDatabase.Get(kvp.Key);
            if (data == null) continue;

            total += data.shield * kvp.Value;
        }

        return total;
    }
}