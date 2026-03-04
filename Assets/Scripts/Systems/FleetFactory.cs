using UnityEngine;
using System;

public static class FleetFactory
{
    // Single ship type version (keep this for debug / future utility)
    public static string TryCreateFleetFromPlanet(GameState state, string planetId, ShipType shipType, int amount)
    {
        if (state == null) return null;

        var planet = state.GetPlanet(planetId);
        if (planet == null) return null;

        if (amount <= 0) return null;

        if (!planet.RemoveStationed(shipType, amount))
            return null;

        var fleet = new Fleet(planetId);
        fleet.AddShips(shipType, amount);

        state.fleets[fleet.fleetId] = fleet;
        return fleet.fleetId;
    }

    // Composition version (used for expeditions)
    public static string TryCreateFleetFromPlanet(GameState state, string planetId, FleetComposition comp)
    {
        if (state == null) return null;

        var planet = state.GetPlanet(planetId);
        if (planet == null) return null;

        if (comp == null || comp.TotalShips() <= 0) return null;

        // Validate availability first (prevents partial removal)
        if (planet.GetStationed(ShipType.Probe) < comp.probes) return null;
        if (planet.GetStationed(ShipType.SmallCargo) < comp.smallCargo) return null;
        if (planet.GetStationed(ShipType.LargeCargo) < comp.largeCargo) return null;

        // ✅ ADD: validate BasicFighter availability
        if (planet.GetStationed(ShipType.BasicFighter) < comp.basicFighters) return null;

        // Remove stationed ships (fail-safe)
        if (comp.probes > 0 && !planet.RemoveStationed(ShipType.Probe, comp.probes)) return null;
        if (comp.smallCargo > 0 && !planet.RemoveStationed(ShipType.SmallCargo, comp.smallCargo)) return null;
        if (comp.largeCargo > 0 && !planet.RemoveStationed(ShipType.LargeCargo, comp.largeCargo)) return null;

        // ✅ ADD: remove stationed BasicFighter
        if (comp.basicFighters > 0 && !planet.RemoveStationed(ShipType.BasicFighter, comp.basicFighters)) return null;

        // Create fleet and add ships
        var fleet = new Fleet(planetId);
        if (comp.probes > 0) fleet.AddShips(ShipType.Probe, comp.probes);
        if (comp.smallCargo > 0) fleet.AddShips(ShipType.SmallCargo, comp.smallCargo);
        if (comp.largeCargo > 0) fleet.AddShips(ShipType.LargeCargo, comp.largeCargo);

        // ✅ ADD: add BasicFighter to fleet
        if (comp.basicFighters > 0) fleet.AddShips(ShipType.BasicFighter, comp.basicFighters);

        // Store fleet
        state.fleets[fleet.fleetId] = fleet;
        return fleet.fleetId;
    }
}