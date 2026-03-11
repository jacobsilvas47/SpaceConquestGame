using UnityEngine;

public enum BuildingType
{
    MetalRefinery,
    CrystalMine,
    GasExtractor
}

public static class BuildingBalance
{
    public static BuildingUpgradeFormula GetFormula(BuildingType type)
    {
        switch (type)
        {
            case BuildingType.MetalRefinery:
                return new BuildingUpgradeFormula
                {
                    baseMetal = 60,
                    baseCrystal = 15,
                    baseGas = 0,
                    costMultiplier = 1.5f,
                    baseBuildTimeSeconds = 8,
                    timeMultiplier = 1.3f
                };

            case BuildingType.CrystalMine:
                return new BuildingUpgradeFormula
                {
                    baseMetal = 48,
                    baseCrystal = 24,
                    baseGas = 0,
                    costMultiplier = 1.55f,
                    baseBuildTimeSeconds = 10,
                    timeMultiplier = 1.32f
                };

            case BuildingType.GasExtractor:
                return new BuildingUpgradeFormula
                {
                    baseMetal = 225,
                    baseCrystal = 75,
                    baseGas = 0,
                    costMultiplier = 1.6f,
                    baseBuildTimeSeconds = 14,
                    timeMultiplier = 1.35f
                };
        }

        return null;
    }

    public static double GetMetalCost(BuildingType type, int currentLevel)
    {
        var f = GetFormula(type);
        return Mathf.FloorToInt((float)(f.baseMetal * System.Math.Pow(f.costMultiplier, currentLevel)));
    }

    public static double GetCrystalCost(BuildingType type, int currentLevel)
    {
        var f = GetFormula(type);
        return Mathf.FloorToInt((float)(f.baseCrystal * System.Math.Pow(f.costMultiplier, currentLevel)));
    }

    public static double GetGasCost(BuildingType type, int currentLevel)
    {
        var f = GetFormula(type);
        return Mathf.FloorToInt((float)(f.baseGas * System.Math.Pow(f.costMultiplier, currentLevel)));
    }

    public static double GetBuildTimeSeconds(BuildingType type, int currentLevel)
    {
        var f = GetFormula(type);
        return System.Math.Floor(f.baseBuildTimeSeconds * System.Math.Pow(f.timeMultiplier, currentLevel));
    }
}