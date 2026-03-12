using System.Text;
using System.Collections.Generic;

public static class BattleReportFormatter
{
    public static string Format(BattleResult result, Fleet attackerRemaining, Fleet defenderRemaining)
    {
        if (result == null)
            return "Battle result was null.";

        StringBuilder sb = new StringBuilder();

        sb.AppendLine("=== BATTLE REPORT ===");
        sb.AppendLine(result.attackerWon ? "Attacker Victory" : "Defender Victory");
        sb.AppendLine();

        sb.AppendLine($"Attacker Power: {result.attackerPower}");
        sb.AppendLine($"Defender Power: {result.defenderPower}");
        sb.AppendLine();

        sb.AppendLine($"Attacker Loss %: {result.attackerLossPercent:P0}");
        sb.AppendLine($"Defender Loss %: {result.defenderLossPercent:P0}");
        sb.AppendLine();

        sb.AppendLine("Attacker Losses:");
        sb.AppendLine(FormatLosses(result.attackerLosses));
        sb.AppendLine();

        sb.AppendLine("Defender Losses:");
        sb.AppendLine(FormatLosses(result.defenderLosses));
        sb.AppendLine();

        sb.AppendLine("Attacker Remaining Fleet:");
        sb.AppendLine(FormatFleet(attackerRemaining));
        sb.AppendLine();

        sb.AppendLine("Defender Remaining Fleet:");
        sb.AppendLine(FormatFleet(defenderRemaining));

        return sb.ToString();
    }

    public static string FormatLosses(BattleLosses losses)
    {
        if (losses == null || losses.lostShips == null || losses.lostShips.Count == 0)
            return "None";

        List<string> parts = new List<string>();

        foreach (var kvp in losses.lostShips)
        {
            string shipName = ShipDatabase.DisplayName(kvp.Key);

            if (kvp.Value != 1)
                shipName += "s";

            parts.Add($"{kvp.Value} {shipName}");
        }

        return string.Join(", ", parts);
    }

    public static string FormatFleet(Fleet fleet)
    {
        if (fleet == null || fleet.ships == null || fleet.ships.Count == 0)
            return "None";

        List<string> parts = new List<string>();

        foreach (var kvp in fleet.ships)
        {
            if (kvp.Value <= 0) continue;

            string shipName = ShipDatabase.DisplayName(kvp.Key);

            if (kvp.Value != 1)
                shipName += "s";

            parts.Add($"{kvp.Value} {shipName}");
        }

        if (parts.Count == 0)
            return "None";

        return string.Join(", ", parts);
    }
}