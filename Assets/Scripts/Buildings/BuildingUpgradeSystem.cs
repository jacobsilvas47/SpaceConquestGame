using UnityEngine;

public static class BuildingUpgradeSystem
{
    public static bool TryQueueUpgrade(
    GameState state,
    string planetId,
    BuildingType buildingType,
    out string error)
    {
        error = null;

        if (state == null)
        {
            Debug.LogError("TryQueueUpgrade failed: state is null.");
            return false;
        }

        PlanetState planet = state.GetPlanet(planetId);

        if (planet == null)
        {
            Debug.LogError($"TryQueueUpgrade failed: planet '{planetId}' not found.");
            return false;
        }

        if (planet.buildingUpgradeQueue == null)
        {
            planet.buildingUpgradeQueue = new System.Collections.Generic.List<BuildingUpgradeJob>();
        }

        int currentLevel = GetBuildingLevel(planet, buildingType);
        int queuedCount = GetQueuedCount(planet, buildingType);
        int targetLevel = currentLevel + queuedCount + 1;

        double metalCost = BuildingBalance.GetMetalCost(buildingType, targetLevel);
        double crystalCost = BuildingBalance.GetCrystalCost(buildingType, targetLevel);
        double gasCost = BuildingBalance.GetGasCost(buildingType, targetLevel);

        if (planet.metal < metalCost || planet.crystal < crystalCost || planet.gas < gasCost)
        {
            error = $"Not enough resources for {buildingType} Level {targetLevel}.";
            return false;
        }

        planet.metal -= metalCost;
        planet.crystal -= crystalCost;
        planet.gas -= gasCost;

        double durationSeconds = BuildingBalance.GetBuildTimeSeconds(buildingType, targetLevel);

        BuildingUpgradeJob job = new BuildingUpgradeJob
        {
            buildingType = buildingType,
            targetLevel = targetLevel,
            durationSeconds = durationSeconds,
            started = false,
            startTimeUtc = 0,
            completeTimeUtc = 0
        };

        planet.buildingUpgradeQueue.Add(job);

        TryStartNextUpgrade(state, planet);

        Debug.Log($"Queued {buildingType} upgrade to level {targetLevel}.");

        error = null;
        return true;
    }

        public static double GetRemainingTime(GameState state, PlanetState planet)
    {
        if (state == null || planet == null)
            return 0;

        BuildingUpgradeJob activeJob = GetActiveUpgrade(planet);

        if (activeJob == null)
            return 0;

        return System.Math.Max(0, activeJob.completeTimeUtc - state.gameTime);
    }

    public static void UpdateBuildingUpgrades(GameState state, string planetId)
    {
        if (state == null) return;

        PlanetState planet = state.GetPlanet(planetId);
        if (planet == null) return;

        if (planet.buildingUpgradeQueue == null || planet.buildingUpgradeQueue.Count == 0)
            return;

        BuildingUpgradeJob activeJob = planet.buildingUpgradeQueue[0];

        if (!activeJob.started)
        {
            StartJob(state, activeJob);
        }

        if (state.gameTime >= activeJob.completeTimeUtc)
        {
            CompleteJob(planet, activeJob);

            planet.buildingUpgradeQueue.RemoveAt(0);

            TryStartNextUpgrade(state, planet);
        }
    }

    public static BuildingUpgradeJob GetActiveUpgrade(PlanetState planet)
    {
        if (planet == null) return null;
        if (planet.buildingUpgradeQueue == null) return null;
        if (planet.buildingUpgradeQueue.Count == 0) return null;

        BuildingUpgradeJob job = planet.buildingUpgradeQueue[0];

        if (!job.started) return null;

        return job;
    }

    public static int GetQueuedCount(PlanetState planet, BuildingType buildingType)
    {
        if (planet == null || planet.buildingUpgradeQueue == null)
            return 0;

        int count = 0;

        foreach (BuildingUpgradeJob job in planet.buildingUpgradeQueue)
        {
            if (job.buildingType == buildingType)
                count++;
        }

        return count;
    }

    private static void TryStartNextUpgrade(GameState state, PlanetState planet)
    {
        if (planet == null) return;
        if (planet.buildingUpgradeQueue == null || planet.buildingUpgradeQueue.Count == 0) return;

        BuildingUpgradeJob nextJob = planet.buildingUpgradeQueue[0];

        if (!nextJob.started)
        {
            StartJob(state, nextJob);
        }
    }

    private static void StartJob(GameState state, BuildingUpgradeJob job)
    {
        job.started = true;
        job.startTimeUtc = state.gameTime;
        job.completeTimeUtc = state.gameTime + job.durationSeconds;

        Debug.Log($"Started {job.buildingType} upgrade to level {job.targetLevel}.");
    }

    private static void CompleteJob(PlanetState planet, BuildingUpgradeJob job)
    {
        SetBuildingLevel(planet, job.buildingType, job.targetLevel);

        Debug.Log($"Completed {job.buildingType} upgrade to level {job.targetLevel}.");
    }

    private static int GetBuildingLevel(PlanetState planet, BuildingType buildingType)
    {
        switch (buildingType)
        {
            case BuildingType.MetalRefinery:
                return planet.metalRefineryLevel;

            case BuildingType.CrystalMine:
                return planet.crystalMineLevel;

            case BuildingType.GasExtractor:
                return planet.gasExtractorLevel;

            case BuildingType.OrbitalShipworks:
                return planet.orbitalShipworksLevel;

            default:
                return 0;
        }
    }

    private static void SetBuildingLevel(PlanetState planet, BuildingType buildingType, int level)
    {
        switch (buildingType)
        {
            case BuildingType.MetalRefinery:
                planet.metalRefineryLevel = level;
                break;

            case BuildingType.CrystalMine:
                planet.crystalMineLevel = level;
                break;

            case BuildingType.GasExtractor:
                planet.gasExtractorLevel = level;
                break;

            case BuildingType.OrbitalShipworks:
                planet.orbitalShipworksLevel = level;
                break;
        }
    }
}