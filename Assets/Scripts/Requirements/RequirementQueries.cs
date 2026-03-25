public static class RequirementQueries
{
    public static int GetLevel(GameState state, string planetId, RequirementType type)
    {
        PlanetState planet = state.GetPlanet(planetId);

        switch (type)
        {
            // 🏗 Buildings
            case RequirementType.MetalRefinery: return planet.metalRefineryLevel;
            case RequirementType.CrystalMine: return planet.crystalMineLevel;
            case RequirementType.GasExtractor: return planet.gasExtractorLevel;
            case RequirementType.OrbitalShipworks: return planet.orbitalShipworksLevel;
            case RequirementType.Barracks: return 0;
            case RequirementType.ResearchLab: return 0;

            // 🧠 Research (THIS IS NEW)
            case RequirementType.LaserTech: return state.research.GetLevel(ResearchType.WeaponSystems);
            case RequirementType.ArmorTech: return state.research.GetLevel(ResearchType.DefenseSystems);
            case RequirementType.EngineTech: return state.research.GetLevel(ResearchType.Engineering);
        }

        return 0;
    }
}