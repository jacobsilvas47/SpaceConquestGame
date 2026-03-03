using UnityEngine;

using System;
using System.Collections.Generic;

[Serializable]
public class Fleet
{
    public string fleetId;

    // which planet it launched from (later, can be a real planet id)
    public string originPlanetId;

    // ship counts by type
    public Dictionary<ShipType, int> ships = new Dictionary<ShipType, int>();

    public Fleet(string originPlanetId)
    {
        this.fleetId = IdUtil.NewId("fleet");
        this.originPlanetId = originPlanetId;
    }

    public int GetCount(ShipType type)
    {
        return ships.TryGetValue(type, out int count) ? count : 0;
    }

    public void AddShips(ShipType type, int amount)
    {
        if (amount <= 0) return;

        if (!ships.ContainsKey(type)) ships[type] = 0;
        ships[type] += amount;
    }

    public bool RemoveShips(ShipType type, int amount)
    {
        if (amount <= 0) return true;

        int current = GetCount(type);
        if (current < amount) return false;

        ships[type] = current - amount;
        if (ships[type] <= 0) ships.Remove(type);

        return true;
    }

    public bool IsEmpty()
    {
        foreach (var kvp in ships)
        {
            if (kvp.Value > 0) return false;
        }
        return true;
    }
}