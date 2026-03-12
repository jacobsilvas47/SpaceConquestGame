using UnityEngine;

public static class ExpeditionCombatResolver
{
    public static string ResolvePirateEncounter(Fleet playerFleet)
    {
        if (playerFleet == null || playerFleet.IsEmpty())
            return "No fleet available for pirate encounter.";

        int playerPower = playerFleet.CombatPowerScore();
        Fleet pirateFleet = PirateFleetFactory.CreatePirateFleet(playerPower);

        BattleResult result = BattleResolver.Resolve(playerFleet, pirateFleet);

        string report = BattleReportFormatter.Format(result, playerFleet, pirateFleet);

        if (result.attackerWon)
        {
            return "Your fleet encountered pirates and won.\n\n" + report;
        }
        else
        {
            return "Your fleet encountered pirates and was defeated.\n\n" + report;
        }
    }
}