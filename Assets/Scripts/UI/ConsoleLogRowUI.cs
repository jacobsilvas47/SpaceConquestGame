using TMPro;
using UnityEngine;

public class ConsoleLogRowUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI detailsText;

    public void Bind(MissionReport report)
    {
        if (report == null) return;

        if (timeText)
            timeText.text = FormatTime(report.createdAtGameTime);

        if (titleText)
            titleText.text = report.title;

        if (detailsText)
            detailsText.text = report.details;
    }

    private string FormatTime(double time)
    {
        int totalSeconds = Mathf.FloorToInt((float)time);
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;
        return $"{minutes:00}:{seconds:00}";
    }
}