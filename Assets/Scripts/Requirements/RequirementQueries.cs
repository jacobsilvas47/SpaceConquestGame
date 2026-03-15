public static class RequirementQueries
{
    public static int GetLevel(GameState state, string planetId, RequirementType type)
    {
        var planet = state.GetPlanet(planetId);
        if (planet == null) return 0;

        switch (type)
        {
            case RequirementType.OrbitalShipworks:
                return planet.GetBuildingLevel(BuildingType.OrbitalShipworks);

            case RequirementType.MetalRefinery:
                return planet.GetBuildingLevel(BuildingType.MetalRefinery);

            case RequirementType.CrystalMine:
                return planet.GetBuildingLevel(BuildingType.CrystalMine);

            case RequirementType.GasExtractor:
                return planet.GetBuildingLevel(BuildingType.GasExtractor);
        }

        return 0;
    }
}