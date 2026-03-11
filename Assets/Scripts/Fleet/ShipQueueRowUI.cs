using TMPro;
using UnityEngine;

public class ShipQueueRowUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI shipNameText;
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private TextMeshProUGUI timerText;

    public void Bind(string shipName, int count, double secondsRemaining)
    {
        if (shipNameText) shipNameText.text = shipName;
        if (countText) countText.text = count.ToString();
        if (timerText) timerText.text = FormatTime(secondsRemaining);
    }

    private string FormatTime(double seconds)
    {
        int total = Mathf.Max(0, Mathf.CeilToInt((float)seconds));
        int minutes = total / 60;
        int secs = total % 60;
        return $"{minutes:00}:{secs:00}";
    }
}