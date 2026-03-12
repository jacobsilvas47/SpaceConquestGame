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

        Debug.Log("=== BATTLE RESULT ===");
        Debug.Log("Attacker Won: " + result.attackerWon);
        Debug.Log("Attacker Loss %: " + result.attackerLossPercent);
        Debug.Log("Defender Loss %: " + result.defenderLossPercent);

        Debug.Log("=== AFTER BATTLE ===");
        Debug.Log("Attacker Remaining Power: " + attacker.CombatPowerScore());
        Debug.Log("Defender Remaining Power: " + defender.CombatPowerScore());

        LogFleet("Attacker Remaining Fleet", attacker);
        LogFleet("Defender Remaining Fleet", defender);

        LogLosses("Attacker Losses", result.attackerLosses);
        LogLosses("Defender Losses", result.defenderLosses);
    }

    private void LogFleet(string label, Fleet fleet)
    {
        Debug.Log("---- " + label + " ----");

        foreach (var kvp in fleet.ships)
        {
            Debug.Log(kvp.Key + ": " + kvp.Value);
        }
    }

    private void LogLosses(string label, BattleLosses losses)
    {
        Debug.Log("---- " + label + " ----");

        foreach (var kvp in losses.lostShips)
        {
            Debug.Log(kvp.Key + ": " + kvp.Value);
        }
    }
}