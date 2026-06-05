using UnityEngine;

public static class DefenseQueueSystem
{
    public static bool TryQueueDefense(
        GameState state,
        string planetId,
        DefenseType defenseType,
        int amount,
        out string error)
    {
        error = null;

        if (state == null)
        {
            error = "Game state missing.";
            return false;
        }

        if (amount <= 0)
        {
            error = "Enter an amount greater than 0.";
            return false;
        }

        PlanetState planet = state.GetPlanet(planetId);

        if (planet == null)
        {
            error = "Planet not found.";
            return false;
        }

        DefenseData data = DefenseDatabase.Get(defenseType);

        if (data == null)
        {
            error = $"No defense data found for {defenseType}.";
            return false;
        }

        if (!RequirementUtility.MeetsRequirements(state, planetId, data.requirements))
        {
            error = GetRequirementText(data);
            return false;
        }

        int totalMetal = data.metalCost * amount;
        int totalCrystal = data.crystalCost * amount;
        int totalGas = data.gasCost * amount;

        if (planet.metal < totalMetal || planet.crystal < totalCrystal || planet.gas < totalGas)
        {
            error =
                $"Not enough resources. Need " +
                $"{NumberFormatter.Format(totalMetal)}M / " +
                $"{NumberFormatter.Format(totalCrystal)}C / " +
                $"{NumberFormatter.Format(totalGas)}G";
            return false;
        }

        planet.metal -= totalMetal;
        planet.crystal -= totalCrystal;
        planet.gas -= totalGas;

        if (planet.defenseBuildQueue == null)
            planet.defenseBuildQueue = new System.Collections.Generic.List<DefenseBuildJob>();

        DefenseBuildJob job = new DefenseBuildJob
        {
            defenseType = defenseType,
            amount = amount,
            durationSeconds = GetBuildTimeSeconds(defenseType, amount),
            started = false,
            startTime = 0,
            completeTime = 0
        };

        planet.defenseBuildQueue.Add(job);

        TryStartNextDefense(state, planet);

        Debug.Log($"Queued {amount} {defenseType}.");

        return true;
    }

    public static void UpdateDefenseQueue(GameState state, string planetId)
    {
        if (state == null) return;

        PlanetState planet = state.GetPlanet(planetId);
        if (planet == null) return;

        if (planet.defenseBuildQueue == null || planet.defenseBuildQueue.Count == 0)
            return;

        DefenseBuildJob activeJob = planet.defenseBuildQueue[0];

        if (!activeJob.started)
            StartJob(state, activeJob);

        if (state.gameTime >= activeJob.completeTime)
        {
            CompleteJob(planet, activeJob);

            planet.defenseBuildQueue.RemoveAt(0);

            TryStartNextDefense(state, planet);
        }
    }

    public static DefenseBuildJob GetActiveDefenseJob(PlanetState planet)
    {
        if (planet == null || planet.defenseBuildQueue == null || planet.defenseBuildQueue.Count == 0)
            return null;

        DefenseBuildJob job = planet.defenseBuildQueue[0];

        return job.started ? job : null;
    }

    private static void TryStartNextDefense(GameState state, PlanetState planet)
    {
        if (state == null || planet == null) return;
        if (planet.defenseBuildQueue == null || planet.defenseBuildQueue.Count == 0) return;

        DefenseBuildJob nextJob = planet.defenseBuildQueue[0];

        if (!nextJob.started)
            StartJob(state, nextJob);
    }

    private static void StartJob(GameState state, DefenseBuildJob job)
    {
        job.started = true;
        job.startTime = state.gameTime;
        job.completeTime = state.gameTime + job.durationSeconds;

        Debug.Log($"Started defense build: {job.amount} {job.defenseType}");
    }

    private static void CompleteJob(PlanetState planet, DefenseBuildJob job)
    {
        planet.AddDefense(job.defenseType, job.amount);

        Debug.Log($"Completed defense build: {job.amount} {job.defenseType}");
    }

    public static double GetBuildTimeSeconds(DefenseType defenseType, int amount)
    {
        double baseTime;

        switch (defenseType)
        {
            case DefenseType.LaserTurret:
                baseTime = 8;
                break;

            case DefenseType.MissileBattery:
                baseTime = 12;
                break;

            case DefenseType.ShieldGenerator:
                baseTime = 18;
                break;

            case DefenseType.PlanetaryCannon:
                baseTime = 30;
                break;

            default:
                baseTime = 10;
                break;
        }

        return baseTime * Mathf.Max(1, amount);
    }

    private static string GetRequirementText(DefenseData defense)
    {
        if (defense == null || defense.requirements == null || defense.requirements.Count == 0)
            return "Requirements not met.";

        var parts = new System.Collections.Generic.List<string>();

        foreach (Requirement req in defense.requirements)
        {
            parts.Add($"{req.type} Lv {req.level}");
        }

        return $"{defense.displayName} requires: " + string.Join(", ", parts);
    }
}