using System.Collections.Generic;

public static class FleetCombatCalculator
{
    public static int GetTotalAttack(Fleet fleet)
    {
        if (fleet == null) return 0;

        int total = 0;

        foreach (KeyValuePair<ShipType, int> kvp in fleet.ships)
        {
            ShipData data = ShipDatabase.Get(kvp.Key);
            if (data == null) continue;

            total += data.attack * kvp.Value;
        }

        return total;
    }

    public static int GetTotalDefense(Fleet fleet)
    {
        if (fleet == null) return 0;

        int total = 0;

        foreach (KeyValuePair<ShipType, int> kvp in fleet.ships)
        {
            ShipData data = ShipDatabase.Get(kvp.Key);
            if (data == null) continue;

            total += data.defense * kvp.Value;
        }

        return total;
    }

    public static int GetTotalHp(Fleet fleet)
    {
        if (fleet == null) return 0;

        int total = 0;

        foreach (KeyValuePair<ShipType, int> kvp in fleet.ships)
        {
            ShipData data = ShipDatabase.Get(kvp.Key);
            if (data == null) continue;

            total += data.maxHp * kvp.Value;
        }

        return total;
    }

    public static int GetTotalCargo(Fleet fleet)
    {
        if (fleet == null) return 0;

        int total = 0;

        foreach (KeyValuePair<ShipType, int> kvp in fleet.ships)
        {
            ShipData data = ShipDatabase.Get(kvp.Key);
            if (data == null) continue;

            total += data.cargoCapacity * kvp.Value;
        }

        return total;
    }

    public static int GetTotalShipCount(Fleet fleet)
    {
        if (fleet == null) return 0;

        int total = 0;

        foreach (KeyValuePair<ShipType, int> kvp in fleet.ships)
        {
            total += kvp.Value;
        }

        return total;
    }

    public static int GetCombatPowerScore(Fleet fleet)
    {
        if (fleet == null) return 0;

        int attack = GetTotalAttack(fleet);
        int defense = GetTotalDefense(fleet);
        int hp = GetTotalHp(fleet);

        return attack + defense + hp;
    }
}