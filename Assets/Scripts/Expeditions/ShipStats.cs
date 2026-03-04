using UnityEngine;

public static class ShipStats
{
    public static int CargoCapacity(ShipType type)
    {
        switch (type)
        {
            case ShipType.Probe:      return 50;
            case ShipType.SmallCargo: return 500;
            case ShipType.LargeCargo: return 2000;
            default: return 0;
        }
    }
}