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
        if (gameStateHolder == null || gameStateHolder.state == null)
        {
            SetAllUnavailable();
            return;
        }

        GameState state = gameStateHolder.state;
        PlanetState planet = state.GetPlanet(currentPlanetId);

        if (planet == null)
        {
            SetAllUnavailable();
            return;
        }

        RefreshBuildingRow(planet, state.gameTime);
        RefreshShipRow(planet, state.gameTime);
        RefreshResearchRow(state);
        RefreshMissionRow(state, planet);
    }

    private void RefreshBuildingRow(PlanetState planet, double gameTime)
    {
        if (buildingRow == null)
            return;

        BuildingUpgradeJob job = planet.activeBuildingUpgrade;

        if (job == null)
        {
            buildingRow.SetInactive("Building Upgrade", "No active upgrade");
            return;
        }

        double totalDuration = job.completeTimeUtc - job.startTimeUtc;
        double elapsed = gameTime - job.startTimeUtc;
        double remaining = job.completeTimeUtc - gameTime;

        float progress = 0f;
        if (totalDuration > 0)
            progress = (float)(elapsed / totalDuration);

        progress = Mathf.Clamp01(progress);
        remaining = Math.Max(0, remaining);

        string detail = $"{job.buildingType} Lv {job.targetLevel - 1} → Lv {job.targetLevel}";
        string timeText = TimeFormatUtility.FormatDuration(remaining);

        buildingRow.SetRow("Building Upgrade", detail, progress, timeText);
    }

    private void RefreshShipRow(PlanetState planet, double gameTime)
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

        double finishTime = item.finishTime;
        double remaining = finishTime - gameTime;

        double buildTime = data != null ? data.buildTimeSeconds : 1.0;
        double startTime = finishTime - buildTime;
        double elapsed = gameTime - startTime;

        float progress = 0f;
        if (buildTime > 0)
            progress = (float)(elapsed / buildTime);

        string detail = shipName;
        string timeText = TimeFormatUtility.FormatDuration(remaining);

        shipRow.SetRow("Ship Production", detail, progress, timeText);
    }

    private void RefreshResearchRow(GameState state)
    {
        if (researchRow == null)
            return;

        researchRow.SetInactive("Research", "No active research");
    }

    private void RefreshMissionRow(GameState state, PlanetState planet)
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

        missionRow.SetRow("Missions", $"{activeCount} active", 1f, "In progress");
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