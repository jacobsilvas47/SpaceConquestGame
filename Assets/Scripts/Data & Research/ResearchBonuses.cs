using UnityEngine;

public static class ResearchBonuses
{
    // Engineering:
    // 5% faster build speed per level
    // Returns a time multiplier, so lower is better.
    public static float GetBuildTimeMultiplier(GameState state)
    {
        if (state == null) return 1f;

        int lvl = state.research.GetLevel(ResearchType.Engineering);
        return Mathf.Pow(0.95f, lvl);
    }

    // Engine Tech:
    // 10% faster fleet travel speed per level
    // Returns a speed multiplier, so higher is better.
    public static float GetFleetSpeedMultiplier(GameState state)
    {
        if (state == null) return 1f;

        int lvl = state.research.GetLevel(ResearchType.EngineTech);
        return Mathf.Pow(1.10f, lvl);
    }

    // Weapon Systems:
    // 10% more attack per level
    public static float GetAttackMultiplier(GameState state)
    {
        if (state == null) return 1f;

        int lvl = state.research.GetLevel(ResearchType.WeaponSystems);
        return Mathf.Pow(1.10f, lvl);
    }

    // Defense Systems:
    // 10% more defense per level
    public static float GetDefenseMultiplier(GameState state)
    {
        if (state == null) return 1f;

        int lvl = state.research.GetLevel(ResearchType.DefenseSystems);
        return Mathf.Pow(1.10f, lvl);
    }

    // Optional:
    // Defense Systems can also improve HP if you want ships to feel tankier.
    public static float GetHpMultiplier(GameState state)
    {
        if (state == null) return 1f;

        int lvl = state.research.GetLevel(ResearchType.DefenseSystems);
        return Mathf.Pow(1.05f, lvl);
    }
}