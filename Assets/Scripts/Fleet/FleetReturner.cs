using UnityEngine;

public static class FleetReturner
{
    public static void ReturnFleetToOrigin(GameState state, string fleetId)
    {
        if (state == null)
        {
            Debug.LogWarning("ReturnFleetToOrigin: state is null.");
            return;
        }

        if (string.IsNullOrEmpty(fleetId))
        {
            Debug.LogWarning("ReturnFleetToOrigin: fleetId is null or empty.");
            return;
        }

        Fleet fleet = state.GetFleet(fleetId);
        if (fleet == null)
        {
            Debug.LogWarning($"ReturnFleetToOrigin: fleet '{fleetId}' not found.");
            return;
        }

        PlanetState originPlanet = state.GetPlanet(fleet.originPlanetId);
        if (originPlanet == null)
        {
            Debug.LogWarning($"ReturnFleetToOrigin: origin planet '{fleet.originPlanetId}' not found.");
            return;
        }

        // Return ships to the origin planet
        foreach (var kvp in fleet.ships)
        {
            originPlanet.AddStationed(kvp.Key, kvp.Value);
        }

        // Deposit carried resources
        originPlanet.metal += fleet.cargoMetal;
        originPlanet.crystal += fleet.cargoCrystal;
        originPlanet.gas += fleet.cargoGas;

        // Clear fleet cargo
        fleet.cargoMetal = 0;
        fleet.cargoCrystal = 0;
        fleet.cargoGas = 0;

        // Remove the fleet from active fleets
        state.fleets.Remove(fleetId);

        Debug.Log($"[FLEET RETURN] Fleet {fleetId} returned to {fleet.originPlanetId}.");
    }
}