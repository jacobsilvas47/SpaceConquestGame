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
        return probes * ShipDatabase.CargoCapacity(ShipType.Probe)
             + smallCargo * ShipDatabase.CargoCapacity(ShipType.SmallCargo)
             + largeCargo * ShipDatabase.CargoCapacity(ShipType.LargeCargo);
    }

    public int TotalShips()
    {
        return probes + smallCargo + largeCargo + basicFighters;
    }
}