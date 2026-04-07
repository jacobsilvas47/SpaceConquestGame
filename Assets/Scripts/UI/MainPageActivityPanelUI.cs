using System;
using System.Collections.Generic;
using UnityEngine;

public class MainPageActivityPanelUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameStateHolder gameStateHolder;
    [SerializeField] private string currentPlanetId = "earth";

    [Header("Rows")]
    [SerializeField] private ActivityRowUI buildingRow;
    [SerializeField] private ActivityRowUI shipRow;
    [SerializeField] private ActivityRowUI researchRow;
    [SerializeField] private ActivityRowUI missionRow;

    private void Update()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (gameStateHolder == null)
        {
            Debug.LogError("ActivityPanel: gameStateHolder is NULL");
            SetAllUnavailable();
            return;
        }

        if (gameStateHolder.state == null)
        {
            Debug.LogError("ActivityPanel: gameStateHolder.state is NULL");
            SetAllUnavailable();
            return;
        }

        GameState state = gameStateHolder.state;

        Debug.Log($"ActivityPanel: currentPlanetId = {currentPlanetId}");

        PlanetState planet = state.GetPlanet(currentPlanetId);

        if (planet == null)
        {
            Debug.LogError($"ActivityPanel: No planet found for id '{currentPlanetId}'");
            SetAllUnavailable();
            return;
        }

        Debug.Log($"ActivityPanel: Found planet '{currentPlanetId}'");
        Debug.Log($"ActivityPanel: activeBuildingUpgrade null? {planet.activeBuildingUpgrade == null}");
        Debug.Log($"ActivityPanel: orbitalShipworksLevel = {planet.orbitalShipworksLevel}");

        double nowUtc = GetNowUtcSeconds();

        RefreshBuildingRow(planet, nowUtc);
        RefreshShipRow(planet, nowUtc);
        RefreshResearchRow(state, planet, nowUtc);
        RefreshMissionRow(state, planet, nowUtc);
    }

    private double GetNowUtcSeconds()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }

    private float CalculateProgress(double startUtc, double endUtc, double nowUtc)
    {
        double totalDuration = endUtc - startUtc;
        if (totalDuration <= 0)
            return 0f;

        double elapsed = nowUtc - startUtc;
        elapsed = Math.Max(0, elapsed);

        return Mathf.Clamp01((float)(elapsed / totalDuration));
    }

    private double CalculateRemaining(double endUtc, double nowUtc)
    {
        return Math.Max(0, endUtc - nowUtc);
    }

    private void RefreshBuildingRow(PlanetState planet, double nowUtc)
    {
        if (buildingRow == null)
            return;

        BuildingUpgradeJob job = planet.activeBuildingUpgrade;

        if (job == null)
        {
            Debug.Log("No active building upgrade found.");
            buildingRow.SetInactive("Building Upgrade", "No active upgrade");
            return;
        }

        Debug.Log($"Building job found: {job.buildingType}, start={job.startTimeUtc}, complete={job.completeTimeUtc}, nowUtc={nowUtc}");

        float progress = CalculateProgress(job.startTimeUtc, job.completeTimeUtc, nowUtc);
        double remaining = CalculateRemaining(job.completeTimeUtc, nowUtc);

        Debug.Log($"Building progress = {progress}, remaining = {remaining}");

        string detail = $"{job.buildingType} Lv {job.targetLevel - 1} → Lv {job.targetLevel}";
        string timeText = TimeFormatUtility.FormatDuration(remaining);

        buildingRow.SetRow("Building Upgrade", detail, progress, timeText);
    }

    private void RefreshShipRow(PlanetState planet, double nowUtc)
    {
        if (shipRow == null)
            return;

        if (planet.shipQueue == null || planet.shipQueue.Count == 0)
        {
            shipRow.SetInactive("Ship Production", "No ships queued");
            return;
        }

        ShipQueueItem item = planet.shipQueue[0];
        ShipData data = ShipDatabase.Get(item.shipType);

        string shipName = data != null ? data.displayName : item.shipType.ToString();

        double finishUtc = item.finishTime;
        double buildTime = data != null ? data.buildTimeSeconds : 1.0;
        double startUtc = finishUtc - buildTime;

        float progress = CalculateProgress(startUtc, finishUtc, nowUtc);
        double remaining = CalculateRemaining(finishUtc, nowUtc);

        Debug.Log($"Ship progress = {progress}, remaining = {remaining}, finishUtc = {finishUtc}, nowUtc = {nowUtc}");

        string detail = shipName;
        string timeText = TimeFormatUtility.FormatDuration(remaining);

        shipRow.SetRow("Ship Production", detail, progress, timeText);
    }

    private void RefreshResearchRow(GameState state, PlanetState planet, double nowUtc)
    {
        if (researchRow == null)
            return;

        researchRow.SetInactive("Research", "No active research");
    }

    private void RefreshMissionRow(GameState state, PlanetState planet, double nowUtc)
    {
        if (missionRow == null)
            return;

        if (state.missions == null || state.missions.Count == 0)
        {
            missionRow.SetInactive("Missions", "No active missions");
            return;
        }

        int activeCount = 0;

        foreach (Mission mission in state.missions)
        {
            if (mission == null)
                continue;

            if (mission.status != MissionStatus.Completed && mission.status != MissionStatus.Failed)
                activeCount++;
        }

        if (activeCount <= 0)
        {
            missionRow.SetInactive("Missions", "No active missions");
            return;
        }

        // Still text-only for now until mission timing fields are wired in.
        missionRow.SetRow("Missions", $"{activeCount} active", 0f, "In progress");
    }

    private void SetAllUnavailable()
    {
        if (buildingRow != null)
            buildingRow.SetInactive("Building Upgrade", "Unavailable");

        if (shipRow != null)
            shipRow.SetInactive("Ship Production", "Unavailable");

        if (researchRow != null)
            researchRow.SetInactive("Research", "Unavailable");

        if (missionRow != null)
            missionRow.SetInactive("Missions", "Unavailable");
    }
}