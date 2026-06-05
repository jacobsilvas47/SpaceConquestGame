using System;

[Serializable]
public class DefenseBuildJob
{
    public DefenseType defenseType;
    public int amount;

    public double durationSeconds;
    public double startTime;
    public double completeTime;

    public bool started;
}