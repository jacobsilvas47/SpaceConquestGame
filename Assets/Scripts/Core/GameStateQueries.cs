using UnityEngine;

public static class GameStateQueries
{
    public static int GetStationedShips(GameState state, string planetId, ShipType type)
    {
        if (state == null) return 0;

        var planet = state.GetPlanet(planetId);
        if (planet == null) return 0;

        return planet.GetStationed(type);
    }

    public static int GetBusyShips(GameState state, ShipType type)
    {
        if (state == null) return 0;

        int busy = 0;

        for (int i = 0; i < state.missions.Count; i++)
        {
            var m = state.missions[i];
            if (m == null) continue;

            if (m.status == MissionStatus.Completed || m.status == MissionStatus.Failed)
                continue;

            var fleet = state.GetFleet(m.fleetId);
            if (fleet == null) continue;

            busy += fleet.GetCount(type);
        }

        return busy;
    }
}