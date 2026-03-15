using System.Collections.Generic;

public static class RequirementUtility
{
    public static bool MeetsRequirements(GameState state, string planetId, List<Requirement> requirements)
    {
        if (requirements == null || requirements.Count == 0)
            return true;

        foreach (var req in requirements)
        {
            int currentLevel = RequirementQueries.GetLevel(state, planetId, req.type);

            if (currentLevel < req.level)
                return false;
        }

        return true;
    }
}