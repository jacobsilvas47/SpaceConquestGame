using System;

[Serializable]
public class BuildingUpgradeJob
{
    public BuildingType buildingType;
    public int targetLevel;

    public double durationSeconds;
    public double startTimeUtc;
    public double completeTimeUtc;

    public bool started;
}