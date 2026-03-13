using System;
using System.Collections.Generic;
using UnityEngine;

public static class ShipyardQueueSystem
{
    public static bool TryEnqueueShip(GameState state, string planetId, ShipType shipType, out string error)
    {
        error = null;

        if (state == null)
        {
            error = "Game state is null.";
            return false;
        }

        PlanetState planet = state.GetPlanet(planetId);
        if (planet == null)
        {
            error = "Planet not found.";
            return false;
        }

        ShipData shipData = ShipDatabase.Get(shipType);
        if (shipData == null)
        {
            error = $"No ship data found for {shipType}.";
            return false;
        }

        if (planet.metal < shipData.metalCost)
        {
            error = $"Not enough Metal for {shipData.displayName}.";
            return false;
        }

        if (planet.crystal < shipData.crystalCost)
        {
            error = $"Not enough Crystal for {shipData.displayName}.";
            return false;
        }

        if (planet.gas < shipData.gasCost)
        {
            error = $"Not enough Gas for {shipData.displayName}.";
            return false;
        }

        planet.metal -= shipData.metalCost;
        planet.crystal -= shipData.crystalCost;
        planet.gas -= shipData.gasCost;

        if (planet.shipQueue == null)
            planet.shipQueue = new List<ShipQueueItem>();

        double startTime = Time.time;

        if (planet.shipQueue.Count > 0)
        {
            double lastFinish = planet.shipQueue[planet.shipQueue.Count - 1].finishTime;
            startTime = Math.Max(Time.time, lastFinish);
        }

        planet.shipQueue.Add(new ShipQueueItem
        {
            shipType = shipType,
            finishTime = startTime + shipData.buildTimeSeconds
        });

        return true;
    }
}