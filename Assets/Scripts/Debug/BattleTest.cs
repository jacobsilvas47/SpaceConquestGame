using UnityEngine;

public class BattleTest : MonoBehaviour
{
    void Start()
    {
        Fleet attacker = new Fleet("home");
        attacker.AddShips(ShipType.BasicFighter, 10);
        attacker.AddShips(ShipType.Interceptor, 5);
        attacker.AddShips(ShipType.SmallCargo, 3);

        Fleet defender = new Fleet("enemy");
        defender.AddShips(ShipType.BasicFighter, 8);
        defender.AddShips(ShipType.SmallCargo, 4);
        defender.AddShips(ShipType.LargeCargo, 2);

        Debug.Log("=== BEFORE BATTLE ===");
        Debug.Log("Attacker Power: " + attacker.CombatPowerScore());
        Debug.Log("Defender Power: " + defender.CombatPowerScore());

        BattleResult result = BattleResolver.Resolve(attacker, defender);

        Debug.Log(BattleReportFormatter.Format(result, attacker, defender));

        Debug.Log("=== AFTER BATTLE ===");
        Debug.Log("Attacker Remaining Power: " + attacker.CombatPowerScore());
        Debug.Log("Defender Remaining Power: " + defender.CombatPowerScore());

        LogFleet("Attacker Remaining Fleet", attacker);
        LogFleet("Defender Remaining Fleet", defender);
    }

    private void LogFleet(string label, Fleet fleet)
    {
        Debug.Log("---- " + label + " ----");

        foreach (var kvp in fleet.ships)
        {
            Debug.Log(ShipDatabase.DisplayName(kvp.Key) + ": " + kvp.Value);
        }
    }
}