using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DefenseQueueRowUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI percentText;
    [SerializeField] private Image progressFill;
    [SerializeField] private Button cancelButton;

    private int queueIndex;
    private System.Action<int> onCancelClicked;

    public void Bind(DefenseBuildJob job, GameState state, int index, double totalRemainingUntilThisJob, System.Action<int> cancelCallback)
    {
        queueIndex = index;
        onCancelClicked = cancelCallback;

        UpdateDisplay(job, state, totalRemainingUntilThisJob);

        if (cancelButton)
        {
            cancelButton.onClick.RemoveAllListeners();
            cancelButton.onClick.AddListener(() => onCancelClicked?.Invoke(queueIndex));
        }
    }

    public void UpdateDisplay(DefenseBuildJob job, GameState state, double totalRemainingUntilThisJob)
    {
        if (job == null || state == null) return;

        if (nameText)
            nameText.text = $"{GetDefenseDisplayName(job.defenseType)} x{job.amount}";

        if (timeText)
            timeText.text = $"Time: {TimeFormatUtility.FormatDuration(totalRemainingUntilThisJob)}";

        float progress = 0f;

        if (job.started && job.durationSeconds > 0)
        {
            double elapsed = state.gameTime - job.startTime;
            progress = Mathf.Clamp01((float)(elapsed / job.durationSeconds));
        }

        if (progressFill)
            progressFill.fillAmount = progress;

        if (percentText)
            percentText.text = $"{Mathf.RoundToInt(progress * 100f)}%";
    }

    private string GetDefenseDisplayName(DefenseType type)
    {
        DefenseData data = DefenseDatabase.Get(type);
        return data != null ? data.displayName : type.ToString();
    }
}