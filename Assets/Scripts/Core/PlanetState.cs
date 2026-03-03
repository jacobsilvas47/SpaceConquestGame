using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class PlanetState
{
    public string planetId;

    // resources on this planet (later you can move these out of ResourceManager)
    public double metal;
    public double crystal;
    public double gas;

    // ships stationed on the planet (not traveling)
    public Dictionary<ShipType, int> stationedShips = new Dictionary<ShipType, int>();

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
}