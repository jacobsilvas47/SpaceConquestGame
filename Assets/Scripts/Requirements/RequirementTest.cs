using UnityEngine;

public class RequirementTest : MonoBehaviour
{
    public GameStateHolder gameStateHolder;
    public string planetId = "home";

    private void Start()
    {
        if (gameStateHolder == null || gameStateHolder.state == null)
        {
            Debug.LogError("RequirementTest: GameStateHolder or state is missing.");
            return;
        }

        TestShipRequirement(ShipType.Probe);
        TestShipRequirement(ShipType.BasicFighter);
        TestShipRequirement(ShipType.Interceptor);
        TestShipRequirement(ShipType.Dreadnought);
    }

    private void TestShipRequirement(ShipType shipType)
    {
        var ship = ShipDatabase.Get(shipType);

        if (ship == null)
        {
            Debug.LogError($"RequirementTest: No ship data for {shipType}");
            return;
        }

        bool meets = RequirementUtility.MeetsRequirements(
            gameStateHolder.state,
            planetId,
            ship.requirements
        );

        int orbitalLevel = RequirementQueries.GetLevel(
            gameStateHolder.state,
            planetId,
            RequirementType.OrbitalShipworks
        );

        Debug.Log($"{ship.displayName} | Orbital Shipworks Level: {orbitalLevel} | Meets Requirements: {meets}");
    }
}