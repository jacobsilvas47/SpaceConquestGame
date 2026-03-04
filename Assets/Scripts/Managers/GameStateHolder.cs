using UnityEngine;

public class GameStateHolder : MonoBehaviour
{
    public GameState state;
    private string debugMissionIdToWatch;
    private MissionStatus debugLastStatus;

    [Header("Defaults")]
    public string startingPlanetId = "home";

    void Awake()
    {
        state = new GameState();

        // create starting planet
        state.planets[startingPlanetId] = new PlanetState(startingPlanetId);

        // give starting stationed probes (your “indestructible 5” can be handled later)
        state.planets[startingPlanetId].AddStationed(ShipType.Probe, 5);
    }

    void Update()
{
    state.gameTime += Time.deltaTime;

    // mission engine tick
    MissionEngine.UpdateMissions(state);

    if (!string.IsNullOrEmpty(debugMissionIdToWatch))
{
    var m = state.missions.Find(x => x != null && x.missionId == debugMissionIdToWatch);
    if (m != null && m.status != debugLastStatus)
        {
            debugLastStatus = m.status;
            Debug.Log($"Mission {m.missionId} status -> {m.status} (t={state.gameTime:F1})");

    // Print expedition reward details when it completes
    if (m.status == MissionStatus.Completed && m is ExpeditionMission exp)
    {
        var p = state.GetPlanet(exp.originPlanetId);
        if (p != null)
            {
            Debug.Log($"Expedition Result: {exp.outcomeSummary} | Totals: M={p.metal:F0} C={p.crystal:F0} G={p.gas:F0}");

            // TODO: put this into UI
            // expeditionResultText.text = exp.outcomeSummary;  // if you have a TMP ref here
            // OR
            // lastExpeditionResult = exp.outcomeSummary;        // if you store it on the holder
                }
            }
        }
    }
}

    public void Debug_CreateFleet_1Probe()
{
    string fleetId = FleetFactory.TryCreateFleetFromPlanet(
        state,
        startingPlanetId,
        ShipType.Probe,
        1
    );

    if (fleetId == null)
    {
        Debug.Log("Failed to create fleet, not enough ships or missing state/planet.");
        return;
    }

    int remainingProbes = state.planets[startingPlanetId].GetStationed(ShipType.Probe);
    Debug.Log($"Created fleet {fleetId} with 1 Probe. Remaining stationed probes: {remainingProbes}");
    }

public void UI_SendExpedition()
{
    // For now, use the debug version, later we’ll pass a fleet composition
    Debug_Send10SecExpedition();
}

public string SendExpedition(FleetComposition comp, double oneWaySeconds)
{
    if (comp == null || comp.TotalShips() <= 0)
    {
        Debug.Log("Cannot send expedition, fleet is empty.");
        return null;
    }

    // ✅ Create ONE fleet containing all ship types in comp
    string fleetId = FleetFactory.TryCreateFleetFromPlanet(state, startingPlanetId, comp);
    if (fleetId == null)
    {
        Debug.Log("Failed to create fleet for expedition (not enough stationed ships?).");
        return null;
    }

    string missionId = MissionEngine.TrySendExpedition(state, fleetId, oneWaySeconds);
    if (missionId == null)
    {
        Debug.Log("Failed to send expedition mission.");
        return null;
    }

    debugMissionIdToWatch = missionId;
    debugLastStatus = MissionStatus.EnRoute;

    Debug.Log($"Sent Expedition mission {missionId} using fleet {fleetId}. One-way: {oneWaySeconds} seconds.");
    return missionId;
}

public void Debug_Send10SecExpedition()
{
    // Create a fleet with 1 probe (like before)
    string fleetId = FleetFactory.TryCreateFleetFromPlanet(
        state,
        startingPlanetId,
        ShipType.Probe,
        1
    );

    int stationedAfterLaunch = state.planets[startingPlanetId].GetStationed(ShipType.Probe);
    Debug.Log($"Stationed probes AFTER launch: {stationedAfterLaunch}");

    if (fleetId == null)
    {
        Debug.Log("Failed to create fleet for expedition.");
        return;
    }

    string missionId = MissionEngine.TrySendExpedition(state, fleetId, 10.0);

    if (missionId == null)
    {
        Debug.Log("Failed to send expedition mission.");
        return;
    }

    debugMissionIdToWatch = missionId;
    debugLastStatus = MissionStatus.EnRoute;
    Debug.Log($"Sent Expedition mission {missionId} using fleet {fleetId}. One-way: 10 seconds.");
    }

}