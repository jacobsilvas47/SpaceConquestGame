using UnityEngine;

public static class ResearchSystem
{
    public static bool TryResearch(GameState state, ResearchType type, string planetId, out string error)
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

        int currentLevel = state.research.GetLevel(type);

        float baseMetal;
        float baseCrystal;
        float multiplier;

        GetResearchValues(type, out baseMetal, out baseCrystal, out multiplier);

        double metalCost = baseMetal * Mathf.Pow(multiplier, currentLevel);
        double crystalCost = baseCrystal * Mathf.Pow(multiplier, currentLevel);

        if (planet.metal < metalCost || planet.crystal < crystalCost)
        {
            error = "Not enough resources";
            return false;
        }

        planet.metal -= metalCost;
        planet.crystal -= crystalCost;

        state.research.IncreaseLevel(type);
        return true;
    }

    public static double GetMetalCost(GameState state, ResearchType type)
    {
        int currentLevel = state.research.GetLevel(type);

        float baseMetal;
        float baseCrystal;
        float multiplier;

        GetResearchValues(type, out baseMetal, out baseCrystal, out multiplier);

        return baseMetal * Mathf.Pow(multiplier, currentLevel);
    }

    public static double GetCrystalCost(GameState state, ResearchType type)
    {
        int currentLevel = state.research.GetLevel(type);

        float baseMetal;
        float baseCrystal;
        float multiplier;

        GetResearchValues(type, out baseMetal, out baseCrystal, out multiplier);

        return baseCrystal * Mathf.Pow(multiplier, currentLevel);
    }

    private static void GetResearchValues(ResearchType type, out float baseMetal, out float baseCrystal, out float multiplier)
    {
        switch (type)
        {
            case ResearchType.Engineering:
                baseMetal = 100f;
                baseCrystal = 50f;
                multiplier = 1.35f;
                return;

            case ResearchType.WeaponSystems:
                baseMetal = 150f;
                baseCrystal = 100f;
                multiplier = 1.4f;
                return;

            case ResearchType.DefenseSystems:
                baseMetal = 200f;
                baseCrystal = 150f;
                multiplier = 1.45f;
                return;

            default:
                baseMetal = 100f;
                baseCrystal = 50f;
                multiplier = 1.3f;
                return;
        }
    }
}