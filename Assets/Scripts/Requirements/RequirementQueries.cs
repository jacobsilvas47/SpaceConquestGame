using UnityEngine;

public static class RequirementQueries
{
    public static int GetLevel(GameState state, string planetId, RequirementType type)
    {
        if (state == null)
        {
            Debug.LogError("[RequirementQueries] GameState is null.");
            return 0;
        }

        if (string.IsNullOrEmpty(planetId))
        {
            Debug.LogWarning($"[RequirementQueries] planetId is null or empty for requirement type {type}.");
            return 0;
        }

        PlanetState planet = state.GetPlanet(planetId);

        if (planet == null)
        {
            Debug.LogWarning($"[RequirementQueries] No planet found for planetId '{planetId}' and requirement type {type}.");
            return 0;
        }

        switch (type)
        {
            case RequirementType.MetalRefinery: return planet.metalRefineryLevel;
            case RequirementType.CrystalMine: return planet.crystalMineLevel;
            case RequirementType.GasExtractor: return planet.gasExtractorLevel;
            case RequirementType.OrbitalShipworks: return planet.orbitalShipworksLevel;

            // Replace these with real values if you support them
            case RequirementType.Barracks: return 0;
            case RequirementType.ResearchLab: return 0;

            default:
                Debug.LogWarning($"[RequirementQueries] Unhandled requirement type: {type}");
                return 0;
        }
    }
}