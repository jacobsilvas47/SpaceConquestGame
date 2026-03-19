public static class MissionUIUtility
{
    public static string GetTargetDisplayName(GameState state, Mission mission)
    {
        if (state == null || mission == null)
            return "Unknown Target";

        switch (mission.targetType)
        {
            case TargetType.PlayerPlanet:
            {
                PlanetState planet = state.GetPlanet(mission.targetId);
                if (planet != null)
                    return string.IsNullOrEmpty(planet.planetId) ? mission.targetId : planet.planetId;

                return mission.targetId;
            }

            case TargetType.AITarget:
            {
                AttackTarget aiTarget = state.GetAITarget(mission.targetId);
                if (aiTarget != null)
                    return aiTarget.displayName;

                return mission.targetId;
            }

            default:
                return mission.targetId;
        }
    }
}