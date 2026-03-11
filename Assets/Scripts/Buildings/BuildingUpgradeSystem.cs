using System;

public static class BuildingUpgradeSystem
{
    public static bool IsUpgradeInProgress(PlanetState planet)
    {
        return planet != null && planet.activeBuildingUpgrade != null;
    }

    public static bool TryStartUpgrade(GameState state, string planetId, BuildingType type, out string error)
    {
        error = null;

        if (state == null)
        {
            error = "State is null.";
            return false;
        }

        var planet = state.GetPlanet(planetId);
        if (planet == null)
        {
            error = "Planet not found.";
            return false;
        }

        if (planet.activeBuildingUpgrade != null)
        {
            error = "Another building upgrade is already in progress.";
            return false;
        }

        int currentLevel = GetBuildingLevel(planet, type);

        double metalCost = BuildingBalance.GetMetalCost(type, currentLevel);
        double crystalCost = BuildingBalance.GetCrystalCost(type, currentLevel);
        double gasCost = BuildingBalance.GetGasCost(type, currentLevel);
        double buildTime = BuildingBalance.GetBuildTimeSeconds(type, currentLevel);

        if (planet.metal < metalCost || planet.crystal < crystalCost || planet.gas < gasCost)
        {
            error = "Not enough resources.";
            return false;
        }

        planet.metal -= metalCost;
        planet.crystal -= crystalCost;
        planet.gas -= gasCost;

        double now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        planet.activeBuildingUpgrade = new BuildingUpgradeJob
        {
            buildingType = type,
            targetLevel = currentLevel + 1,
            startTimeUtc = now,
            completeTimeUtc = now + buildTime
        };

        return true;
    }

    public static void TickPlanet(PlanetState planet)
    {
        if (planet == null || planet.activeBuildingUpgrade == null) return;

        double now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        if (now < planet.activeBuildingUpgrade.completeTimeUtc) return;

        var job = planet.activeBuildingUpgrade;

        SetBuildingLevel(planet, job.buildingType, job.targetLevel);

        planet.activeBuildingUpgrade = null;
    }

    public static double GetRemainingTime(PlanetState planet)
    {
        if (planet == null || planet.activeBuildingUpgrade == null) return 0;

        double now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        return System.Math.Max(0, planet.activeBuildingUpgrade.completeTimeUtc - now);
    }

    private static int GetBuildingLevel(PlanetState planet, BuildingType type)
    {
        switch (type)
        {
            case BuildingType.MetalRefinery: return planet.metalRefineryLevel;
            case BuildingType.CrystalMine: return planet.crystalMineLevel;
            case BuildingType.GasExtractor: return planet.gasExtractorLevel;
            default: return 0;
        }
    }

    private static void SetBuildingLevel(PlanetState planet, BuildingType type, int level)
    {
        switch (type)
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
        }
    }
}