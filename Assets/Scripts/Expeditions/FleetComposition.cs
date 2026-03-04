using System;
using UnityEngine;

[Serializable]
public class FleetComposition
{

    public int basicFighters = 0;
    public int probes;
    public int smallCargo;
    public int largeCargo;

public int TotalStorage()
    {
    return probes * ShipStats.CargoCapacity(ShipType.Probe)
         + smallCargo * ShipStats.CargoCapacity(ShipType.SmallCargo)
         + largeCargo * ShipStats.CargoCapacity(ShipType.LargeCargo);
    }

public int TotalShips()
    {
    return probes + smallCargo + largeCargo + basicFighters;
    }
}