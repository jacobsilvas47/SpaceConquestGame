using System;
using System.Collections.Generic;

[Serializable]
public class BattleLosses
{
    public Dictionary<ShipType, int> lostShips = new Dictionary<ShipType, int>();

    public void AddLoss(ShipType type, int amount)
    {
        if (amount <= 0) return;

        if (!lostShips.ContainsKey(type))
            lostShips[type] = 0;

        lostShips[type] += amount;
    }

    public int GetLoss(ShipType type)
    {
        return lostShips.TryGetValue(type, out int amount) ? amount : 0;
    }
}