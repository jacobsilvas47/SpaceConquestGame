using System;

[Serializable]
public class ResearchJob
{
    public ResearchType researchType;
    public int targetLevel;

    public double durationSeconds;
    public double startTime;
    public double completeTime;

    public bool started;
}