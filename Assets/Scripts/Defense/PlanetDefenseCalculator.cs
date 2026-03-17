public static class PlanetDefenseCalculator
{
    public static int GetStationedFleetAttack(GameState state, string planetId)
    {
        if (state == null) return 0;

        int total = 0;

        foreach (ShipType type in System.Enum.GetValues(typeof(ShipType)))
        {
            int count = GameStateQueries.GetStationedShips(state, planetId, type);
            if (count <= 0) continue;

            var ship = ShipDatabase.Get(type);
            if (ship == null) continue;

            total += ship.attack * count;
        }

        return total;
    }

    public static int GetStationedFleetDefense(GameState state, string planetId)
    {
        if (state == null) return 0;

        int total = 0;

        foreach (ShipType type in System.Enum.GetValues(typeof(ShipType)))
        {
            int count = GameStateQueries.GetStationedShips(state, planetId, type);
            if (count <= 0) continue;

            var ship = ShipDatabase.Get(type);
            if (ship == null) continue;

            total += ship.maxHp * count;
        }

        return total;
    }
}