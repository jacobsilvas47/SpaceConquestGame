using UnityEngine;

public class PirateEncounterTest : MonoBehaviour
{
    void Start()
    {
        Fleet expeditionFleet = new Fleet("home");
        expeditionFleet.AddShips(ShipType.BasicFighter, 10);
        expeditionFleet.AddShips(ShipType.Interceptor, 4);
        expeditionFleet.AddShips(ShipType.SmallCargo, 3);

        string report = ExpeditionCombatResolver.ResolvePirateEncounter(expeditionFleet);

        Debug.Log(report);
    }
}