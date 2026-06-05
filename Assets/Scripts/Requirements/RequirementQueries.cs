using UnityEngine;

public static class RequirementQueries
{
    public static int GetLevel(GameState state, string planetId, RequirementType type)
    {
        PlanetState planet = state.GetPlanet(planetId);

        if (planet == null)
        {
            Debug.LogWarning($"[RequirementQueries] Planet not found: {planetId}");
            return 0;
        }

        switch (type)
        {
            // Buildings
            case RequirementType.MetalRefinery:
                return planet.metalRefineryLevel;

            case RequirementType.CrystalMine:
                return planet.crystalMineLevel;

            case RequirementType.GasExtractor:
                return planet.gasExtractorLevel;

            case RequirementType.OrbitalShipworks:
                return planet.orbitalShipworksLevel;

            case RequirementType.Barracks:
                return 0;

            case RequirementType.ResearchLab:
                return 0;

            // Research
            case RequirementType.LaserTech:
                return state.research.GetLevel(ResearchType.WeaponSystems);

            case RequirementType.ArmorTech:
                return state.research.GetLevel(ResearchType.DefenseSystems);

            case RequirementType.EngineTech:
                return state.research.GetLevel(ResearchType.EngineTech);

            default:
                Debug.LogWarning($"[RequirementQueries] Unhandled requirement type: {type}");
                return 0;

            // Defense
            case RequirementType.Engineering:
            return state.research.GetLevel(ResearchType.Engineering);

            case RequirementType.WeaponSystems:
                return state.research.GetLevel(ResearchType.WeaponSystems);

            case RequirementType.DefenseSystems:
                return state.research.GetLevel(ResearchType.DefenseSystems);
        }
    }
}