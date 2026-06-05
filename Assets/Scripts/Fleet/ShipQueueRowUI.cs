using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShipQueueRowUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI percentText;
    [SerializeField] private Image progressFill;
    [SerializeField] private Button cancelButton;

    private int queueIndex;
    private System.Action<int> onCancelClicked;

    public void Bind(ShipQueueItem item, int index, System.Action<int> cancelCallback)
    {
        queueIndex = index;
        onCancelClicked = cancelCallback;

        UpdateDisplay(item);

        if (cancelButton)
        {
            cancelButton.onClick.RemoveAllListeners();
            cancelButton.onClick.AddListener(() => onCancelClicked?.Invoke(queueIndex));
        }
    }

    public void UpdateDisplay(ShipQueueItem item)
    {
        if (item == null) return;

        string shipName = ShipDatabase.DisplayName(item.shipType);

        if (nameText)
            nameText.text = $"{ShipDatabase.DisplayName(item.shipType)} x{item.amount}";

        double remaining = System.Math.Max(0, item.finishTime - Time.time);

        float progress = 0f;

        if (item.durationSeconds > 0)
        {
            double elapsed = Time.time - item.startTime;
            progress = Mathf.Clamp01((float)(elapsed / item.durationSeconds));
        }

        if (timeText)
            timeText.text = $"Time: {TimeFormatUtility.FormatDuration(remaining)}";

        if (progressFill)
            progressFill.fillAmount = progress;

        if (percentText)
            percentText.text = $"{Mathf.RoundToInt(progress * 100f)}%";
    }
}