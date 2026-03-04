using UnityEngine;

public static class FleetReturner
{
    public static void ReturnFleetToOrigin(GameState state, string fleetId)
    {
        if (state == null) return;
        if (string.IsNullOrEmpty(fleetId)) return;

        Fleet fleet = state.GetFleet(fleetId);
        if (fleet == null) return;

        PlanetState planet = state.GetPlanet(fleet.originPlanetId);
        if (planet == null) return;

        // Move every ship in the fleet back onto the planet
        foreach (var kvp in fleet.ships)
        {
            planet.AddStationed(kvp.Key, kvp.Value);
        }

        // Remove the fleet from active fleets
        state.fleets.Remove(fleetId);
    }
}