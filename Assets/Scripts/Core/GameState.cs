using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class GameState
{
    [Header("Expeditions")]
    public int expeditionSlotsUnlocked = 1;
    public List<ExpeditionLogEntry> expeditionLog = new List<ExpeditionLogEntry>();

    public double gameTime; // your “clock”, you can increment from a MonoBehaviour

    public Dictionary<string, PlanetState> planets = new Dictionary<string, PlanetState>();
    public Dictionary<string, Fleet> fleets = new Dictionary<string, Fleet>();

    // keep missions as a list so order is stable
    public List<Mission> missions = new List<Mission>();

    // Inventory
    public InventoryState inventory = new InventoryState();

    public PlanetState GetPlanet(string planetId)
    {
        planets.TryGetValue(planetId, out var p);
        return p;
    }

    public Fleet GetFleet(string fleetId)
    {
        fleets.TryGetValue(fleetId, out var f);
        return f;
    }
}