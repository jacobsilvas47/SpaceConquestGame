using System;

[Serializable]
public class BuildingUpgradeFormula
{
    public double baseMetal;
    public double baseCrystal;
    public double baseGas;

    public float costMultiplier = 1.5f;

    public double baseBuildTimeSeconds = 10;
    public float timeMultiplier = 1.25f;
}