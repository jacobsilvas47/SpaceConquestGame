using UnityEngine;

public static class ResearchSystem
{
    public static bool TryQueueResearch(GameState state, ResearchType type, string planetId, out string error)
    {
        error = null;

        if (state == null)
        {
            error = "State is null";
            return false;
        }

        PlanetState planet = state.GetPlanet(planetId);
        if (planet == null)
        {
            error = "Planet not found";
            return false;
        }

        if (state.research.researchQueue == null)
            state.research.researchQueue = new System.Collections.Generic.List<ResearchJob>();

        int currentLevel = state.research.GetLevel(type);
        int queuedCount = GetQueuedCount(state, type);
        int targetLevel = currentLevel + queuedCount + 1;

        double metalCost = GetMetalCostForLevel(type, targetLevel);
        double crystalCost = GetCrystalCostForLevel(type, targetLevel);

        if (planet.metal < metalCost || planet.crystal < crystalCost)
        {
            error = "Not enough resources";
            return false;
        }

        planet.metal -= metalCost;
        planet.crystal -= crystalCost;

        ResearchJob job = new ResearchJob
        {
            researchType = type,
            targetLevel = targetLevel,
            durationSeconds = GetResearchTimeSeconds(type, targetLevel),
            started = false,
            startTime = 0,
            completeTime = 0
        };

        state.research.researchQueue.Add(job);

        TryStartNextResearch(state);

        return true;
    }

    public static void UpdateResearch(GameState state)
    {
        if (state == null || state.research == null || state.research.researchQueue == null)
            return;

        if (state.research.researchQueue.Count == 0)
            return;

        ResearchJob activeJob = state.research.researchQueue[0];

        if (!activeJob.started)
            StartJob(state, activeJob);

        if (state.gameTime >= activeJob.completeTime)
        {
            CompleteJob(state, activeJob);
            state.research.researchQueue.RemoveAt(0);
            TryStartNextResearch(state);
        }
    }

    public static ResearchJob GetActiveResearch(GameState state)
    {
        if (state == null || state.research == null || state.research.researchQueue == null)
            return null;

        if (state.research.researchQueue.Count == 0)
            return null;

        ResearchJob job = state.research.researchQueue[0];
        return job.started ? job : null;
    }

    public static int GetQueuedCount(GameState state, ResearchType type)
    {
        if (state == null || state.research == null || state.research.researchQueue == null)
            return 0;

        int count = 0;

        foreach (ResearchJob job in state.research.researchQueue)
        {
            if (job.researchType == type)
                count++;
        }

        return count;
    }

    private static void TryStartNextResearch(GameState state)
    {
        if (state == null || state.research.researchQueue.Count == 0)
            return;

        ResearchJob nextJob = state.research.researchQueue[0];

        if (!nextJob.started)
            StartJob(state, nextJob);
    }

    private static void StartJob(GameState state, ResearchJob job)
    {
        job.started = true;
        job.startTime = state.gameTime;
        job.completeTime = state.gameTime + job.durationSeconds;
    }

    private static void CompleteJob(GameState state, ResearchJob job)
    {
        state.research.IncreaseLevel(job.researchType);
        Debug.Log($"Completed {job.researchType} research to level {job.targetLevel}");
    }

    public static double GetMetalCost(GameState state, ResearchType type)
    {
        int nextLevel = state.research.GetLevel(type) + GetQueuedCount(state, type) + 1;
        return GetMetalCostForLevel(type, nextLevel);
    }

    public static double GetCrystalCost(GameState state, ResearchType type)
    {
        int nextLevel = state.research.GetLevel(type) + GetQueuedCount(state, type) + 1;
        return GetCrystalCostForLevel(type, nextLevel);
    }

    public static double GetResearchTimeSeconds(ResearchType type, int level)
    {
        GetResearchValues(type, out float baseMetal, out float baseCrystal, out float multiplier, out float baseTime, out float timeMultiplier);
        return System.Math.Floor(baseTime * System.Math.Pow(timeMultiplier, level));
    }

    private static double GetMetalCostForLevel(ResearchType type, int level)
    {
        GetResearchValues(type, out float baseMetal, out float baseCrystal, out float multiplier, out float baseTime, out float timeMultiplier);
        return System.Math.Floor(baseMetal * System.Math.Pow(multiplier, level));
    }

    private static double GetCrystalCostForLevel(ResearchType type, int level)
    {
        GetResearchValues(type, out float baseMetal, out float baseCrystal, out float multiplier, out float baseTime, out float timeMultiplier);
        return System.Math.Floor(baseCrystal * System.Math.Pow(multiplier, level));
    }

    private static void GetResearchValues(
        ResearchType type,
        out float baseMetal,
        out float baseCrystal,
        out float multiplier,
        out float baseTime,
        out float timeMultiplier)
    {
        switch (type)
        {
            case ResearchType.Engineering:
                baseMetal = 100f;
                baseCrystal = 50f;
                multiplier = 1.35f;
                baseTime = 10f;
                timeMultiplier = 1.35f;
                return;

            case ResearchType.EngineTech:
                baseMetal = 125f;
                baseCrystal = 75f;
                multiplier = 1.38f;
                baseTime = 12f;
                timeMultiplier = 1.38f;
                return;

            case ResearchType.WeaponSystems:
                baseMetal = 150f;
                baseCrystal = 100f;
                multiplier = 1.4f;
                baseTime = 14f;
                timeMultiplier = 1.4f;
                return;

            case ResearchType.DefenseSystems:
                baseMetal = 200f;
                baseCrystal = 150f;
                multiplier = 1.45f;
                baseTime = 16f;
                timeMultiplier = 1.45f;
                return;

            default:
                baseMetal = 100f;
                baseCrystal = 50f;
                multiplier = 1.3f;
                baseTime = 10f;
                timeMultiplier = 1.3f;
                return;
        }
    }
}