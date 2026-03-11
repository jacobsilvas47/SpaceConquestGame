using System;

[Serializable]
public class BuildingUpgradeJob
{
    public BuildingType buildingType;
    public int targetLevel;
    public double startTimeUtc;
    public double completeTimeUtc;
}