using System;
using UnityEngine;

[Serializable]
public class FleetComposition
{
    public int probes;
    public int smallCargo;
    public int largeCargo;

    // TODO: move these to ShipStats later
    public int probeStorage = 50;
    public int smallCargoStorage = 500;
    public int largeCargoStorage = 2000;

    public int TotalStorage()
    {
        int total = 0;
        total += probes * probeStorage;
        total += smallCargo * smallCargoStorage;
        total += largeCargo * largeCargoStorage;
        return total;
    }

    public int TotalShips()
    {
        return probes + smallCargo + largeCargo;
    }
}