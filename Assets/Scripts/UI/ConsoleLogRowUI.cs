using System.Text;
using TMPro;
using UnityEngine;

public class ConsoleLogRowUI : MonoBehaviour
{
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI detailsText;

    public void Bind(ExpeditionLogEntry e)
    {
        if (e == null) return;

        if (timeText) timeText.text = FormatTime(e.timestamp);
        if (titleText) titleText.text = "Expedition Complete";

        if (!detailsText) return;

        // ✅ 1) Use the mission-provided summary if present (covers FOUND_SHIPS, EMPTY, JACKPOT, etc.)
        string details = e.summaryText;

        // ✅ 2) Fallback to numeric breakdown only if summaryText is missing
        if (string.IsNullOrWhiteSpace(details))
        {
            details = $"+{e.metalGained} Metal, +{e.crystalGained} Crystal, +{e.gasGained} Gas";
        }

        // ✅ 3) Append items if any
        if (e.itemsGained != null && e.itemsGained.Count > 0)
        {
            var sb = new StringBuilder();
            sb.Append(details);
            sb.Append(" | Items: ");

            for (int i = 0; i < e.itemsGained.Count; i++)
            {
                var it = e.itemsGained[i];
                sb.Append($"+{it.count} {it.itemId}");
                if (i < e.itemsGained.Count - 1) sb.Append(", ");
            }

            detailsText.text = sb.ToString();
        }
        else
        {
            detailsText.text = details;
        }
    }

    private string FormatTime(double t)
    {
        int total = Mathf.Max(0, Mathf.FloorToInt((float)t));
        int m = total / 60;
        int s = total % 60;
        return $"{m:00}:{s:00}";
    }
}