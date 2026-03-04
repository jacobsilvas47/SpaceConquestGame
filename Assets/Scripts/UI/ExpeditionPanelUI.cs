using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;

public class ExpeditionPanelUI : MonoBehaviour
{
    private string activeMissionId = null;
    private double activeMissionArriveTime = 0;
    private double activeMissionReturnTime = 0;
    private float resultDisplayUntilRealtime = 0f;

    [Header("Refs")]
    [SerializeField] private GameStateHolder gameStateHolder;

    [Header("UI")]
    [SerializeField] private Button sendButton;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI resultText;

    // Expedition Inputs
    [Header("Inputs")]
    [SerializeField] private TMP_InputField probesInput;
    [SerializeField] private TMP_InputField smallCargoInput;
    [SerializeField] private TMP_InputField largeCargoInput;
    [SerializeField] private TMP_InputField basicFightersInput;

    // Expedition Max Buttons
    [Header("Max Buttons")]
    [SerializeField] private Button maxProbesButton;
    [SerializeField] private Button maxSmallCargoButton;
    [SerializeField] private Button maxLargeCargoButton;
    [SerializeField] private Button maxBasicFightersButton;

    private int ReadIntOrZero(TMP_InputField f)
    {
        if (f == null) return 0;
        var s = f.text?.Trim();
        if (string.IsNullOrEmpty(s)) return 0;
        return int.TryParse(s, out int v) ? Mathf.Max(0, v) : 0;
    }

    // Create fleet if missing, then return it
    private Fleet GetOrCreateFleet(GameState state, string fleetId, string originPlanetId)
    {
        Fleet fleet = state.GetFleet(fleetId);
        if (fleet != null) return fleet;

        fleet = new Fleet(fleetId);
        fleet.originPlanetId = originPlanetId;

        // state.fleets is a Dictionary<string, Fleet>
        state.fleets[fleetId] = fleet;

        Debug.Log($"Created new fleet '{fleetId}' at planet '{originPlanetId}'");
        return fleet;
    }

    // Best-effort composition setter that works even if Fleet has different internals.
    private void ApplyFleetComposition(Fleet fleet, int probes, int smallCargo, int largeCargo)
    {
        TryClearFleetShips(fleet);

        TrySetShipCount(fleet, ShipType.Probe, probes);
        TrySetShipCount(fleet, ShipType.SmallCargo, smallCargo);
        TrySetShipCount(fleet, ShipType.LargeCargo, largeCargo);

        Debug.Log("Applied fleet composition to fleet object.");
    }

    private void TryClearFleetShips(Fleet fleet)
    {
        if (fleet == null) return;

        object dictObj = GetMemberValue(fleet, "ships") ?? GetMemberValue(fleet, "shipCounts");
        if (dictObj is IDictionary dict)
        {
            dict.Clear();
            return;
        }

        InvokeIfExists(fleet, "Clear");
        InvokeIfExists(fleet, "ClearShips");
        InvokeIfExists(fleet, "Reset");
    }

    private void TrySetShipCount(Fleet fleet, ShipType type, int count)
    {
        if (fleet == null) return;

        count = Mathf.Max(0, count);

        if (InvokeIfExists(fleet, "SetShipCount", type, count)) return;
        if (InvokeIfExists(fleet, "SetCount", type, count)) return;
        if (InvokeIfExists(fleet, "SetShip", type, count)) return;

        object dictObj = GetMemberValue(fleet, "ships") ?? GetMemberValue(fleet, "shipCounts");
        if (dictObj is IDictionary dict)
        {
            dict[type] = count;
            return;
        }

        // fallback if Fleet uses int fields
        if (type == ShipType.Probe) SetMemberValue(fleet, "probes", count);
        if (type == ShipType.SmallCargo) SetMemberValue(fleet, "smallCargo", count);
        if (type == ShipType.LargeCargo) SetMemberValue(fleet, "largeCargo", count);
    }

    private object GetMemberValue(object obj, string name)
    {
        if (obj == null) return null;

        var t = obj.GetType();
        var f = t.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (f != null) return f.GetValue(obj);

        var p = t.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (p != null) return p.GetValue(obj);

        return null;
    }

    private void SetMemberValue(object obj, string name, object value)
    {
        if (obj == null) return;

        var t = obj.GetType();
        var f = t.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (f != null)
        {
            f.SetValue(obj, value);
            return;
        }

        var p = t.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (p != null && p.CanWrite)
        {
            p.SetValue(obj, value);
        }
    }

    private bool InvokeIfExists(object obj, string methodName, params object[] args)
    {
        if (obj == null) return false;

        var t = obj.GetType();
        var m = t.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (m == null) return false;

        try
        {
            m.Invoke(obj, args);
            return true;
        }
        catch
        {
            return false;
        }
    }

    // Send Expedition Method
    public void OnSendPressed()
    {
        if (gameStateHolder == null || gameStateHolder.state == null)
        {
            Debug.LogError("ExpeditionPanelUI missing GameStateHolder/state.");
            return;
        }

        if (!string.IsNullOrEmpty(activeMissionId))
        {
            if (statusText) statusText.text = "Expedition already running.";
            return;
        }

        // Clear any stale result text on new send
        if (resultText) resultText.text = "";

        int probes = ReadIntOrZero(probesInput);
        int smallCargo = ReadIntOrZero(smallCargoInput);
        int largeCargo = ReadIntOrZero(largeCargoInput);
        int basicFighters = ReadIntOrZero(basicFightersInput);

        if (probes + smallCargo + largeCargo + basicFighters <= 0)
        {
            if (statusText) statusText.text = "Pick at least 1 ship.";
            return;
        }

        var comp = new FleetComposition
        {
            probes = probes,
            smallCargo = smallCargo,
            largeCargo = largeCargo,
            basicFighters = basicFighters
        };

        double oneWaySeconds = 5;

        // 🔎 DEBUG HERE
        var p = gameStateHolder.state.GetPlanet(gameStateHolder.startingPlanetId);
        Debug.Log($"Stationed BEFORE send: P={p.GetStationed(ShipType.Probe)} SC={p.GetStationed(ShipType.SmallCargo)} LC={p.GetStationed(ShipType.LargeCargo)}");
        Debug.Log($"Requested: P={probes} SC={smallCargo} LC={largeCargo}");

        string missionId = gameStateHolder.SendExpedition(comp, oneWaySeconds);

        if (string.IsNullOrEmpty(missionId))
        {
            if (statusText) statusText.text = "Failed to send expedition.";
            return;
        }

        // clear inputs after success
        if (probesInput) probesInput.text = "";
        if (smallCargoInput) smallCargoInput.text = "";
        if (largeCargoInput) largeCargoInput.text = "";
        if (basicFightersInput) basicFightersInput.text = "";

        activeMissionId = missionId;

        var mission = gameStateHolder.state.missions.Find(m => m != null && m.missionId == missionId);
        if (mission != null)
        {
            activeMissionArriveTime = mission.arriveTime;
            activeMissionReturnTime = mission.returnTime;
        }

        if (statusText) statusText.text = "Expedition sent!";
    }

    private int GetStationedCount(ShipType type)
{
    if (gameStateHolder == null || gameStateHolder.state == null) return 0;

    var p = gameStateHolder.state.GetPlanet(gameStateHolder.startingPlanetId);
    if (p == null) return 0;

    return Mathf.Max(0, p.GetStationed(type));
}

    // Start Max Button Methods

    public void OnMaxProbesPressed()
    {
        int max = GetStationedCount(ShipType.Probe);
        if (probesInput) probesInput.text = max.ToString();
    }

    public void OnMaxSmallCargoPressed()
    {
        int max = GetStationedCount(ShipType.SmallCargo);
        if (smallCargoInput) smallCargoInput.text = max.ToString();
    }

    public void OnMaxLargeCargoPressed()
    {
        int max = GetStationedCount(ShipType.LargeCargo);
        if (largeCargoInput) largeCargoInput.text = max.ToString();
    }

    public void OnMaxBasicFightersPressed()
    {
        int max = GetStationedCount(ShipType.BasicFighter);
        if (basicFightersInput) basicFightersInput.text = max.ToString();
    }

    // End Max Button Methods

    private void Update()
    {
        if (gameStateHolder == null || gameStateHolder.state == null) return;
        if (statusText == null) return;
        if (string.IsNullOrEmpty(activeMissionId)) return;

        double now = gameStateHolder.state.gameTime;

        // Try to locate the mission each frame
        ExpeditionMission active = null;
        var missions = gameStateHolder.state.missions;
        for (int i = 0; i < missions.Count; i++)
        {
            if (missions[i] != null && missions[i].missionId == activeMissionId)
            {
                active = missions[i] as ExpeditionMission;
                break;
            }
        }

        // If found, use authoritative timing + result
        if (active != null)
        {
            activeMissionArriveTime = active.arriveTime;
            activeMissionReturnTime = active.returnTime;

            // ✅ If mission completed and has a result, show it for 3 seconds
            if (now >= active.returnTime && !string.IsNullOrEmpty(active.resultText) && !active.resultShown)
            {
                active.resultShown = true;

                // Show the result in the dedicated result label
                if (resultText) resultText.text = active.resultText;

                // Optional: keep statusText too
                statusText.text = "Expedition complete!";

                resultDisplayUntilRealtime = Time.realtimeSinceStartup + 3f;
                return;
            }
        }

        // If we’re currently showing the result, keep it up until timer expires
        if (resultDisplayUntilRealtime > 0f)
        {
            if (Time.realtimeSinceStartup < resultDisplayUntilRealtime) return;

            // result display finished, clear it and reset mission tracking
            resultDisplayUntilRealtime = 0f;

            if (resultText) resultText.text = "";
            statusText.text = "Expedition complete!";

            activeMissionId = null;
            activeMissionArriveTime = 0;
            activeMissionReturnTime = 0;
            return;
        }

        // Completed but no result text available
        if (now >= activeMissionReturnTime)
        {
            statusText.text = "Expedition complete!";

            if (resultText) resultText.text = "";

            activeMissionId = null;
            activeMissionArriveTime = 0;
            activeMissionReturnTime = 0;
            return;
        }

        // Phase messaging
        if (now < activeMissionArriveTime)
        {
            double toArrive = activeMissionArriveTime - now;
            statusText.text = $"Expedition en route, {toArrive:0.0}s to arrival";
            return;
        }

        double toReturn = activeMissionReturnTime - now;
        statusText.text = $"Expedition returning, {toReturn:0.0}s to return";
    }
}