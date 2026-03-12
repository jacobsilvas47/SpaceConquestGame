using UnityEngine;

public class FleetCombatTest : MonoBehaviour
{
    void Start()
    {
        Fleet testFleet = new Fleet("home");

        testFleet.AddShips(ShipType.BasicFighter, 10);
        testFleet.AddShips(ShipType.SmallCargo, 5);
        testFleet.AddShips(ShipType.Freighter, 2);

        Debug.Log("Fleet Attack: " + testFleet.TotalAttack());
        Debug.Log("Fleet Defense: " + testFleet.TotalDefense());
        Debug.Log("Fleet HP: " + testFleet.TotalHp());
        Debug.Log("Fleet Cargo: " + testFleet.TotalCargo());
        Debug.Log("Fleet Ships: " + testFleet.TotalShipCount());
        Debug.Log("Fleet Power Score: " + testFleet.CombatPowerScore());
    }
}