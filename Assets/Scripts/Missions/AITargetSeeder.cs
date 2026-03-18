using UnityEngine;

public static class AITargetSeeder
{
    public static void Seed(GameState state)
    {
        if (state == null) return;
        if (state.aiTargets == null) return;
        if (state.aiTargets.Count > 0) return;

        Add(state, AIFactionType.Pirate, AITargetTier.Tier1, "Pirate Camp Alpha");
        Add(state, AIFactionType.Pirate, AITargetTier.Tier2, "Pirate Camp Beta");

        Add(state, AIFactionType.Alien, AITargetTier.Tier1, "Alien Nest K-12");
        Add(state, AIFactionType.Alien, AITargetTier.Tier3, "Alien Hive V-41");

        Add(state, AIFactionType.Computer, AITargetTier.Tier2, "Computer Outpost Sigma");
        Add(state, AIFactionType.Computer, AITargetTier.Tier4, "Computer Fortress Delta");
    }

    private static void Add(GameState state, AIFactionType factionType, AITargetTier tier, string name)
    {
        AttackTarget target = AITargetGenerator.CreateTarget(factionType, tier, name);
        state.aiTargets[target.targetId] = target;

        Debug.Log($"[AI TARGET] Seeded {target.displayName}, faction={target.factionType}, tier={target.tier}");
    }
}