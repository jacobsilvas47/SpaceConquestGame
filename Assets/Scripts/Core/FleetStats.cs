using UnityEngine;
using System;

public static class FleetStats
{
    public static int GetCargoCapacity(GameState state, string fleetId)
    {
        if (state == null || string.IsNullOrEmpty(fleetId)) return 0;

        var fleet = state.GetFleet(fleetId);
        if (fleet == null) return 0;

        int cap = 0;

        cap += fleet.GetCount(ShipType.Probe) * 50;
        cap += fleet.GetCount(ShipType.SmallCargo) * 500;
        cap += fleet.GetCount(ShipType.LargeCargo) * 2000;

        return cap;
    }
}