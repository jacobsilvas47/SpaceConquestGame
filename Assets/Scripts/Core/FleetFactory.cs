using UnityEngine;
using System;

public static class FleetFactory
{
    // Creates a fleet by removing ships from the planet's stationedShips
    // Returns the created fleetId if successful, otherwise null.
    public static string TryCreateFleetFromPlanet(GameState state, string planetId, ShipType shipType, int amount)
    {
        if (state == null) return null;

        var planet = state.GetPlanet(planetId);
        if (planet == null) return null;

        if (amount <= 0) return null;

        // do we have enough stationed ships?
        if (!planet.RemoveStationed(shipType, amount))
            return null;

        // create fleet
        var fleet = new Fleet(planetId);
        fleet.AddShips(shipType, amount);

        // store it in GameState
        state.fleets[fleet.fleetId] = fleet;

        return fleet.fleetId;
    }
}